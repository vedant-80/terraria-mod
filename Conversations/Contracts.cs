using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewMod.Conversations;

public sealed record ConversationIdentity(Guid WorldId, Guid PlayerId, string Npc)
{
    public string Key => WorldId.ToString("N") + "_" + PlayerId.ToString("N") + "_" + Npc;
    public void Validate()
    {
        if (WorldId == Guid.Empty || PlayerId == Guid.Empty || Npc is not ("Guide" or "Merchant" or "Nurse"))
            throw new ArgumentException("Invalid conversation identity.");
    }
}
public sealed record Exchange(string Question, string Reply);
public sealed record MemoryRecord(ConversationIdentity Identity)
{
    public string Summary { get; init; } = "";
    public List<Exchange> Recent { get; init; } = new();
    public int Affinity { get; init; }
    public long Day { get; init; } = -1;
    public int DailyMovement { get; init; }
    public List<string> AskedQuestions { get; init; } = new();
    public DateTime LastTouchedUtc { get; init; } = DateTime.UtcNow;
}
public sealed record ConversationRequest(string Model, string Personality, string WorldContext, MemoryRecord Memory, string Question);
public sealed record ConversationReply(string Dialogue, string? Summary, int? AffinityDelta);
public sealed record LocalModel(string Name, long SizeBytes);
public interface ILocalModelClient
{
    Task<IReadOnlyList<LocalModel>> ListModelsAsync(CancellationToken cancellationToken = default);
    Task<ConversationReply> ChatAsync(ConversationRequest request, CancellationToken cancellationToken = default);
}
public static class RelationshipRules
{
    public const int MaxQuestion = 300, MaxReply = 600, MaxSummary = 1200, MaxRecent = 4;
    public static string Normalize(string value) => string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    public static MemoryRecord Apply(MemoryRecord memory, string question, int? delta, long day)
    {
        if (string.IsNullOrWhiteSpace(question) || question.Length > MaxQuestion) throw new ArgumentException("Question must contain 1–300 characters.");
        var asked = day == memory.Day ? new List<string>(memory.AskedQuestions) : new List<string>();
        var used = day == memory.Day ? memory.DailyMovement : 0;
        var normalized = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Normalize(question))));
        // Once the bounded duplicate set is full, no further movement is permitted this day.
        var movement = asked.Contains(normalized) || asked.Count >= 64 ? 0 : Math.Clamp(delta ?? 0, -2, 2);
        movement = Math.Sign(movement) * Math.Min(Math.Abs(movement), Math.Max(0, 5 - used));
        var affinity = Math.Clamp(memory.Affinity + movement, -100, 100);
        used += Math.Abs(affinity - memory.Affinity);
        if (!asked.Contains(normalized) && asked.Count < 64) asked.Add(normalized);
        return memory with { Affinity = affinity, Day = day, DailyMovement = used, AskedQuestions = asked, LastTouchedUtc = DateTime.UtcNow };
    }
    public static MemoryRecord Complete(MemoryRecord memory, string question, ConversationReply reply, long day)
    {
        if (string.IsNullOrWhiteSpace(reply.Dialogue) || reply.Dialogue.Length > MaxReply) throw new ArgumentException("Invalid dialogue.");
        var result = Apply(memory, question, reply.AffinityDelta, day);
        return result with { Summary = reply.Summary ?? memory.Summary, Recent = memory.Recent.Append(new Exchange(question, reply.Dialogue)).TakeLast(MaxRecent).ToList() };
    }
    public static double PurchaseMultiplier(int affinity) => 1d - Math.Clamp(affinity, -100, 100) / 1000d;
}
