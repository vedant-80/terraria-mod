using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NewMod.Common;

// Explicit identities are persisted independently of display names and local player indexes.
public sealed class ConversationPlayer : ModPlayer
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public override void SaveData(TagCompound tag) => tag["conversationPlayerId"] = Id.ToString("N");
    public override void LoadData(TagCompound tag)
    {
        if (Guid.TryParse(tag.GetString("conversationPlayerId"), out var id) && id != Guid.Empty) Id = id;
    }
}

public sealed class ConversationWorld : ModSystem
{
    public Guid Id { get; private set; }
    public long Day { get; private set; }
    private bool wasDay;

    public override void OnWorldLoad()
    {
        Id = Guid.NewGuid();
        Day = 0;
        wasDay = Main.dayTime;
        if (!Main.dedServ && Main.netMode == NetmodeID.SinglePlayer)
            ConversationRuntime.Current = new ConversationRuntime(Mod);
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["conversationWorldId"] = Id.ToString("N");
        tag["conversationDay"] = Day;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (Guid.TryParse(tag.GetString("conversationWorldId"), out var id) && id != Guid.Empty) Id = id;
        Day = Math.Max(0, tag.GetLong("conversationDay"));
        wasDay = Main.dayTime;
    }

    public override void PostUpdateWorld()
    {
        if (Main.netMode != NetmodeID.SinglePlayer) return;
        if (Main.dayTime && !wasDay && Day < long.MaxValue) Day++;
        wasDay = Main.dayTime;
    }

    public override void OnWorldUnload()
    {
        ConversationRuntime.Current?.Dispose();
        ConversationRuntime.Current = null;
    }

    public override void Unload() => OnWorldUnload();
}

public sealed class ConversationShop : GlobalNPC
{
    public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
    {
        if (Main.netMode != NetmodeID.SinglePlayer || npc.type != NPCID.Merchant) return;
        int affinity = ConversationRuntime.Current?.MerchantAffinity ?? 0;
        if (affinity == 0) return;
        decimal multiplier = 1m - Math.Clamp(affinity, -100, 100) / 1000m;
        foreach (var item in items)
        {
            // Buyback inventory is supplied separately by vanilla; only alter freshly built coin-priced stock.
            if (item == null || item.IsAir || item.shopSpecialCurrency >= 0) continue;
            int price = item.shopCustomPrice ?? item.value;
            if (price <= 0) continue;
            decimal lower = decimal.Ceiling(price * 0.9m);
            decimal upper = decimal.Floor(price * 1.1m);
            item.shopCustomPrice = (int)Math.Clamp(decimal.Ceiling(price * multiplier), Math.Max(1m, lower), Math.Min(int.MaxValue, upper));
        }
    }
}
