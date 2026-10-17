using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewMod.Conversations;

// JSON metadata in a fenced Markdown block preserves exact text, including hostile delimiters.
// Only files produced by this store are eligible for eviction. Unknown/future files are preserved.
public sealed class MarkdownMemoryStore
{
    private const string Header = "# NewMod NPC memory v1\n\n```json\n";
    private const int MaxFileBytes = 50000;
    private readonly string root;
    private readonly int maxConversations, maxRelationships;
    private readonly SemaphoreSlim gate = new(1, 1);
    private sealed record Document(int Version, MemoryRecord Memory);
    public MarkdownMemoryStore(string root, int maxConversations = 128, int maxRelationships = 512)
    {
        if (maxConversations < 1 || maxRelationships < maxConversations) throw new ArgumentOutOfRangeException(nameof(maxConversations));
        this.root = Path.GetFullPath(root);
        this.maxConversations = maxConversations;
        this.maxRelationships = maxRelationships;
    }
    public string RootDirectory => root;
    private string FileName(ConversationIdentity identity) { identity.Validate(); return Path.Combine(root, identity.Key + ".md"); }
    private static void Validate(MemoryRecord record)
    {
        if (record.Identity is null) throw new InvalidDataException("Missing memory identity.");
        try { record.Identity.Validate(); }
        catch (ArgumentException ex) { throw new InvalidDataException("Invalid memory identity.", ex); }
        if (record.Summary is null || record.Summary.Length > 1200 || record.Recent is null || record.Recent.Count > 4 ||
            record.Recent.Any(x => x is null || string.IsNullOrWhiteSpace(x.Question) || x.Question.Length > 300 || string.IsNullOrWhiteSpace(x.Reply) || x.Reply.Length > 600) ||
            record.Affinity < -100 || record.Affinity > 100 || record.DailyMovement < 0 || record.DailyMovement > 5 ||
            record.AskedQuestions is null || record.AskedQuestions.Count > 64 || record.AskedQuestions.Any(x => x is null || x.Length > 300) || record.LastTouchedUtc == default)
            throw new InvalidDataException("Memory violates bounded format.");
    }
    private static async Task<MemoryRecord> Read(string path, CancellationToken ct)
    {
        if (new FileInfo(path).Length > MaxFileBytes) throw new InvalidDataException("Memory file is oversized.");
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        var buffer = new byte[MaxFileBytes + 1];
        var count = 0;
        while (count < buffer.Length) { var n = await stream.ReadAsync(buffer.AsMemory(count), ct); if (n == 0) break; count += n; }
        if (count > MaxFileBytes) throw new InvalidDataException("Memory file is oversized.");
        var text = Encoding.UTF8.GetString(buffer, 0, count);
        if (!text.StartsWith(Header, StringComparison.Ordinal) || !text.EndsWith("\n```\n", StringComparison.Ordinal)) throw new InvalidDataException("Unsupported memory format; preserve file and reset explicitly.");
        Document? document;
        try { document = JsonSerializer.Deserialize<Document>(text.Substring(Header.Length, text.Length - Header.Length - 5)); }
        catch (JsonException ex) { throw new InvalidDataException("Malformed memory.", ex); }
        if (document is null || document.Version != 1 || document.Memory is null) throw new InvalidDataException("Unsupported memory version.");
        Validate(document.Memory);
        return document.Memory;
    }
    public async Task<MemoryRecord> LoadAsync(ConversationIdentity identity, CancellationToken cancellationToken = default)
    {
        var path = FileName(identity);
        await gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(path)) return new MemoryRecord(identity);
            var memory = await Read(path, cancellationToken);
            if (memory.Identity != identity) throw new InvalidDataException("Memory identity mismatch.");
            return memory;
        }
        finally { gate.Release(); }
    }
    private static async Task<string> Stage(string path, MemoryRecord memory, CancellationToken ct)
    {
        var content = Header + JsonSerializer.Serialize(new Document(1, memory), new JsonSerializerOptions { WriteIndented = true }) + "\n```\n";
        if (Encoding.UTF8.GetByteCount(content) > MaxFileBytes) throw new InvalidDataException("Encoded memory exceeds file budget.");
        var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try { await File.WriteAllTextAsync(temp, content, new UTF8Encoding(false), ct); return temp; }
        catch { if (File.Exists(temp)) File.Delete(temp); throw; }
    }
    public async Task SaveAsync(MemoryRecord memory, CancellationToken cancellationToken = default)
    {
        Validate(memory);
        var path = FileName(memory.Identity);
        await gate.WaitAsync(cancellationToken);
        try
        {
            Directory.CreateDirectory(root);
            if (File.Exists(path)) { var old = await Read(path, cancellationToken); if (old.Identity != memory.Identity) throw new InvalidDataException("Memory identity mismatch."); }
            var entries = new List<(string Path, MemoryRecord Memory)>();
            // Bounded directory metadata scan. Unknown files count against capacity rather than being deleted.
            var files = Directory.EnumerateFiles(root).Take(maxRelationships + 1).ToList();
            if (files.Count > maxRelationships) throw new IOException("Memory directory exceeds capacity; inspect it before saving.");
            foreach (var file in files)
            {
                if (!file.EndsWith(".md", StringComparison.Ordinal)) continue;
                try { var found = await Read(file, cancellationToken); if (Path.GetFileName(file) == found.Identity.Key + ".md") entries.Add((file, found)); }
                catch (Exception ex) when (ex is InvalidDataException || ex is JsonException || ex is ArgumentException) { /* preserve unknown files */ }
            }
            entries = entries.Where(x => x.Path != path).OrderBy(x => x.Memory.LastTouchedUtc).ThenBy(x => x.Path, StringComparer.Ordinal).ToList();
            int textCount = entries.Count(x => x.Memory.Summary.Length != 0 || x.Memory.Recent.Count != 0) + (memory.Summary.Length != 0 || memory.Recent.Count != 0 ? 1 : 0);
            string? remove = null;
            if (!File.Exists(path) && files.Count >= maxRelationships)
            {
                if (entries.Count == 0) throw new IOException("Memory capacity occupied by unrecognized files.");
                remove = entries[0].Path;
            }
            var staged = new List<(string Temp, string Path)>();
            try
            {
                staged.Add((await Stage(path, memory, cancellationToken), path));
                foreach (var entry in entries)
                {
                    if (textCount <= maxConversations) break;
                    if (entry.Memory.Summary.Length == 0 && entry.Memory.Recent.Count == 0) continue;
                    if (entry.Path != remove) staged.Add((await Stage(entry.Path, entry.Memory with { Summary = "", Recent = new() }, cancellationToken), entry.Path));
                    textCount--;
                }
                cancellationToken.ThrowIfCancellationRequested();
                // Commit is noncancellable after all files stage successfully. Each rename is atomic;
                // a disk failure/crash between renames can leave a partial aggregate eviction.
                foreach (var entry in staged) File.Move(entry.Temp, entry.Path, true);
                if (remove is not null) File.Delete(remove);
            }
            finally { foreach (var entry in staged) if (File.Exists(entry.Temp)) File.Delete(entry.Temp); }
        }
        finally { gate.Release(); }
    }
    public async Task ResetAsync(ConversationIdentity identity, CancellationToken cancellationToken = default)
    {
        var path = FileName(identity);
        await gate.WaitAsync(cancellationToken);
        try { if (File.Exists(path)) File.Delete(path); }
        finally { gate.Release(); }
    }
}
