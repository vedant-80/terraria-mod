using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using NewMod.Common.UI;

namespace NewMod.Common;

public sealed class ConversationCommand : ModCommand
{
    public override string Command => "npcchat";
    public override CommandType Type => CommandType.Chat;
    public override string Usage => "/npcchat";
    public override string Description => Language.GetTextValue("Mods.NewMod.Chat.CommandDescription");
    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (Main.netMode != NetmodeID.SinglePlayer || Main.dedServ || ConversationRuntime.Current == null)
        {
            caller.Reply(Language.GetTextValue("Mods.NewMod.Chat.SinglePlayerOnly"));
            return;
        }
        ModContent.GetInstance<ConversationUISystem>().ShowSettings();
    }
}
