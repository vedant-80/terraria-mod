using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewMod.Conversations;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace NewMod.Common;

// All Terraria state is read/applied on the game thread. Background work only receives snapshots.
public sealed class ConversationRuntime : IDisposable
{
    public static ConversationRuntime Current { get; internal set; }
    public string Status { get; private set; } = Text("SetupNeeded");
    public string Reply { get; private set; } = "";
    public string HardwareDescription { get; } = WindowsMemory.Describe();
    public int Affinity => memory?.Affinity ?? 0;
    public int MerchantAffinity { get; private set; }
    public bool IsBusy => pending != null || saveTask is { IsCompleted: false };
    public IReadOnlyList<string> Models { get; private set; } = Array.Empty<string>();
    public string SelectedModel { get; private set; } = "qwen2.5:1.5b";
    public string NpcKey { get; private set; } = "";
    private readonly MarkdownMemoryStore store;
    private readonly OllamaModelClient client = new();
    private readonly Dictionary<string, string> bundled = new();
    private readonly string personalities;
    private readonly string settingsPath;
    private MemoryRecord memory;
    private int npcIndex = -1;
    private CancellationTokenSource cancellation;
    private Task<Action> pending;
    private readonly List<Task> abandoned = new();
    private int generation;
    private bool disposed;
    private bool loadAttempted;
    private bool merchantLoaded;
    private Task saveTask;
    private static Task persistence = Task.CompletedTask;

    public ConversationRuntime(Mod mod)
    {
        string root = Path.Combine(Main.SavePath, "NewMod", "Conversations");
        store = new MarkdownMemoryStore(Path.Combine(root, "Memory"));
        personalities = Path.Combine(root, "Personalities");
        settingsPath = Path.Combine(root, "settings.json");
        foreach (string key in new[] { "Guide", "Merchant", "Nurse" })
            bundled[key] = Encoding.UTF8.GetString(mod.GetFileBytes($"Personalities/{key}.md"));
        Start(async token =>
        {
            Directory.CreateDirectory(personalities);
            foreach (var pair in bundled)
            {
                string path = Path.Combine(personalities, pair.Key + ".md");
                if (!File.Exists(path)) await File.WriteAllTextAsync(path, pair.Value, token);
            }
            string model = SelectedModel;
            if (File.Exists(settingsPath) && new FileInfo(settingsPath).Length < 4096)
            {
                try { model = JsonSerializer.Deserialize<string>(await File.ReadAllTextAsync(settingsPath, token)) ?? model; }
                catch (JsonException) { }
            }
            return () => { SelectedModel = model; Status = Text("SetupNeeded"); };
        });
    }

    public static string Text(string key, params object[] args) => Language.GetTextValue("Mods.NewMod.Chat." + key, args);
    private static string Key(int type) => type switch { NPCID.Guide => "Guide", NPCID.Merchant => "Merchant", NPCID.Nurse => "Nurse", _ => "" };

    public void Update()
    {
        abandoned.RemoveAll(task => task.IsCompleted);
        if (saveTask?.IsCompleted == true)
        {
            if (saveTask.IsFaulted) { _ = saveTask.Exception; Status = Text("SaveFailed"); }
            saveTask = null;
        }
        int index = Main.LocalPlayer.dead ? -1 : Main.LocalPlayer.talkNPC;
        if (index < 0 || index >= Main.maxNPCs || !Main.npc[index].active || Main.npc[index].Distance(Main.LocalPlayer.Center) > 600f)
            index = -1;
        string key = index >= 0 ? Key(Main.npc[index].type) : "";
        if (key.Length == 0) index = -1;
        // Invalidate before consuming results, including the frame an NPC closes or changes.
        if (index != npcIndex) Cancel();
        if (pending?.IsCompleted == true)
        {
            var completed = pending;
            pending = null;
            cancellation?.Dispose();
            cancellation = null;
            try { completed.GetAwaiter().GetResult()?.Invoke(); }
            catch (OperationCanceledException) { Status = Text("TimedOut"); }
            catch (System.Net.Http.HttpRequestException) { Status = Text("RuntimeUnavailable"); }
            catch (InvalidDataException) { Status = Text("InvalidData"); }
            catch (JsonException) { Status = Text("InvalidData"); }
            catch (IOException) { Status = Text("FileFailure"); }
            catch (UnauthorizedAccessException) { Status = Text("FileFailure"); }
            catch (InvalidOperationException) { Status = Text("ModelUnavailable"); }
            catch (Exception ex) when (ex is ArgumentException or System.ComponentModel.Win32Exception)
            { Status = Text("OperationFailed"); }
        }
        if (index == npcIndex)
        {
            if (key.Length > 0 && memory == null && !IsBusy && !loadAttempted) LoadCurrent(key);
            else if (index < 0 && !IsBusy && !merchantLoaded) LoadMerchantCache();
            return;
        }
        npcIndex = index;
        NpcKey = key;
        memory = null;
        Reply = "";
        if (index < 0) return;
        LoadCurrent(key);
    }

    private ConversationIdentity Identity(string key) => new(ModContent.GetInstance<ConversationWorld>().Id,
        Main.LocalPlayer.GetModPlayer<ConversationPlayer>().Id, key);

    private void LoadCurrent(string key)
    {
        loadAttempted = true;
        var identity = Identity(key);
        Task precedingSave = persistence;
        Start(async token =>
        {
            try { await precedingSave; } catch (IOException) { }
            var loaded = await store.LoadAsync(identity, token);
            return () =>
            {
                memory = loaded;
                if (key == "Merchant") { MerchantAffinity = loaded.Affinity; merchantLoaded = true; }
                Status = Text("ReadyToTalk");
            };
        });
    }

    private void LoadMerchantCache()
    {
        merchantLoaded = true;
        var identity = Identity("Merchant");
        Task precedingSave = persistence;
        Start(async token =>
        {
            try { await precedingSave; } catch (IOException) { }
            var loaded = await store.LoadAsync(identity, token);
            return () => MerchantAffinity = loaded.Affinity;
        });
    }

    private void PersistAccepted(MemoryRecord accepted)
    {
        // Acceptance happens on the game thread. Closing UI later cannot cancel an accepted turn's save.
        // Serialize across world exits/re-entry so an earlier save cannot overwrite a newer relationship.
        Task precedingSave = persistence;
        saveTask = persistence = Task.Run(async () =>
        {
            try { await precedingSave; } catch (Exception) { /* the previous failure is observed separately */ }
            await store.SaveAsync(accepted, CancellationToken.None);
        });
        _ = saveTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
    }

    // Start invoked by input events or lifecycle transitions, never performs disk/network work inline.
    private void Start(Func<CancellationToken, Task<Action>> operation)
    {
        if (disposed || pending != null) return;
        cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(45));
        var token = cancellation.Token;
        int version = generation;
        pending = Task.Run(async () =>
        {
            var apply = await operation(token);
            token.ThrowIfCancellationRequested();
            return (Action)(() => { if (!disposed && version == generation) apply(); });
        }, token);
    }

    public void Cancel()
    {
        generation++;
        loadAttempted = false;
        cancellation?.Cancel();
        cancellation?.Dispose();
        cancellation = null;
        if (pending != null)
        {
            // Observe abandoned failures; a late result can never mutate game state.
            abandoned.Add(pending.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted));
            pending = null;
        }
        Status = Text("Cancelled");
    }

    public void RefreshModels()
    {
        if (IsBusy) return;
        Status = Text("Checking");
        Start(async token =>
        {
            var models = await client.ListModelsAsync(token);
            var names = models.Select(m => m.Name).ToArray();
            return () => { Models = names; Status = Text(names.Length == 0 ? "NoModels" : "ModelsFound", names.Length); };
        });
    }

    public void SelectModel(string name)
    {
        if (IsBusy || !Models.Contains(name)) return;
        SelectedModel = name;
        Start(async token =>
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
            await File.WriteAllTextAsync(settingsPath, JsonSerializer.Serialize(name), token);
            return () => Status = Text("ModelSelected", name);
        });
    }

    public void TestModel()
    {
        if (IsBusy) return;
        string model = SelectedModel;
        var id = new ConversationIdentity(Guid.NewGuid(), Guid.NewGuid(), "Guide");
        Status = Text("Testing");
        Start(async token =>
        {
            var clock = Stopwatch.StartNew();
            await client.ChatAsync(new ConversationRequest(model, "You are a friendly Terraria guide. Answer briefly.", "Readiness test only.", new MemoryRecord(id), "Hello."), token);
            return () => Status = Text("TestPassed", clock.Elapsed.TotalSeconds.ToString("0.0"));
        });
    }

    public void Send(string question)
    {
        if (IsBusy || memory == null || string.IsNullOrWhiteSpace(question) || question.Length > 300) return;
        string key = NpcKey;
        var original = memory;
        string model = SelectedModel;
        long day = ModContent.GetInstance<ConversationWorld>().Day;
        string snapshot = Snapshot();
        string path = Path.Combine(personalities, key + ".md");
        Status = Text("Thinking");
        Start(async token =>
        {
            string persona = bundled[key];
            if (File.Exists(path))
            {
                if (new FileInfo(path).Length > 16384) throw new InvalidDataException("Personality exceeds limit.");
                persona = await File.ReadAllTextAsync(path, token);
            }
            var response = await client.ChatAsync(new ConversationRequest(model, persona, snapshot, original, question), token);
            var updated = RelationshipRules.Complete(original, question, response, day);
            return () =>
            {
                memory = updated;
                Reply = response.Dialogue;
                if (key == "Merchant") MerchantAffinity = updated.Affinity;
                Status = Text("ReadyToTalk");
                PersistAccepted(updated);
            };
        });
    }

    private static string Snapshot()
    {
        var p = Main.LocalPlayer;
        string biome = p.ZoneJungle ? "jungle" : p.ZoneSnow ? "snow" : p.ZoneDesert ? "desert" : p.ZoneDungeon ? "dungeon" : p.ZoneUnderworldHeight ? "underworld" : "forest or underground";
        var defeated = new List<string>();
        if (NPC.downedBoss1) defeated.Add("Eye of Cthulhu");
        if (NPC.downedBoss2) defeated.Add("world evil boss");
        if (NPC.downedBoss3) defeated.Add("Skeletron");
        if (NPC.downedMechBossAny) defeated.Add("a mechanical boss");
        if (NPC.downedPlantBoss) defeated.Add("Plantera");
        if (NPC.downedGolemBoss) defeated.Add("Golem");
        if (NPC.downedMoonlord) defeated.Add("Moon Lord");
        return $"Time: {(Main.dayTime ? "day" : "night")}. Biome: {biome}. Hardmode: {Main.hardMode}. Defeated bosses: {string.Join(", ", defeated)}. Do not reveal events or enemies beyond this progression.";
    }

    public void ResetCurrent()
    {
        if (IsBusy || NpcKey.Length == 0) return;
        var identity = new ConversationIdentity(ModContent.GetInstance<ConversationWorld>().Id, Main.LocalPlayer.GetModPlayer<ConversationPlayer>().Id, NpcKey);
        string key = NpcKey;
        Task precedingSave = persistence;
        Start(async token =>
        {
            try { await precedingSave; } catch (Exception) { }
            await store.ResetAsync(identity, token);
            return () =>
            {
                memory = new MemoryRecord(identity);
                if (key == "Merchant") MerchantAffinity = 0;
                Reply = "";
                Status = Text("ResetDone");
            };
        });
    }

    public void OpenFolder(bool personalityFolder)
    {
        if (IsBusy) return;
        string path = personalityFolder ? personalities : store.RootDirectory;
        Start(token =>
        {
            Directory.CreateDirectory(path);
            Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            return Task.FromResult<Action>(() => Status = Text("FolderOpened"));
        });
    }

    public void Dispose()
    {
        disposed = true;
        Cancel();
        client.Dispose();
    }
}
