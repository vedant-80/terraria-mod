using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace NewMod.Common.UI;

// Draw in vanilla's own NPC dialogue pass, in its coordinate system and button row.
[Autoload(Side = ModSide.Client)]
public sealed class ConversationChatButton : ModSystem
{
    private const float TextScale = 0.9f;
    private const float ButtonGap = 30f;
    private string lastFirst, lastSecond, lastClose, lastHappiness, lastTalk;
    private float rowWidth;
    private Vector2 talkSize;
    private bool hovered;

    public override void Load() => On_Main.DrawNPCChatButtons += DrawChatButtons;
    public override void Unload() => On_Main.DrawNPCChatButtons -= DrawChatButtons;

    private void DrawChatButtons(On_Main.orig_DrawNPCChatButtons orig, int mouseTextColor,
        Color chatColor, int amountOfLines, string first, string second)
    {
        orig(mouseTextColor, chatColor, amountOfLines, first, second);
        if (ModContent.GetInstance<ConversationUISystem>().ConversationVisible)
        {
            // GUIChatDrawInner consumes these flags after this draw call.
            // Keep vanilla actions from handling clicks through the overlay.
            Main.npcChatFocus1 = Main.npcChatFocus2 = Main.npcChatFocus3 = Main.npcChatFocus4 = false;
            hovered = false;
            return;
        }
        int npc = Main.LocalPlayer.talkNPC;
        if (Main.gameMenu || Main.netMode != NetmodeID.SinglePlayer || Main.LocalPlayer.dead ||
            npc < 0 || npc >= Main.maxNPCs || !Main.npc[npc].active ||
            Main.npc[npc].type is not (NPCID.Guide or NPCID.Merchant or NPCID.Nurse))
        {
            hovered = false;
            return;
        }

        string close = Lang.inter[52].Value;
        string happiness = !Main.remixWorld && !string.IsNullOrEmpty(Main.LocalPlayer.currentShoppingSettings.HappinessReport)
            ? Language.GetTextValue("UI.NPCCheckHappiness") : "";
        string talk = ConversationUI.Text("Talk");
        if (lastFirst != first || lastSecond != second || lastClose != close || lastHappiness != happiness || lastTalk != talk)
        {
            lastFirst = first; lastSecond = second; lastClose = close; lastHappiness = happiness; lastTalk = talk;
            // Installed 2026.08 vanilla measures at 0.9, caps the first action at
            // 260 pixels, and advances by measured width + 30 between actions.
            rowWidth = Math.Min(260f, Measure(first).X) + ButtonGap + Measure(close).X + ButtonGap;
            if (!string.IsNullOrWhiteSpace(second)) rowWidth += Measure(second).X + ButtonGap;
            if (happiness.Length > 0) rowWidth += Measure(happiness).X + ButtonGap;
            talkSize = Measure(talk);
        }
        var position = new Vector2(180 + (Main.screenWidth - 800) / 2 + rowWidth, 130 + amountOfLines * 30);
        bool over = !PlayerInput.IgnoreMouseInterface && new Rectangle((int)position.X, (int)position.Y,
            (int)Math.Ceiling(talkSize.X), (int)Math.Ceiling(talkSize.Y)).Contains(Main.mouseX, Main.mouseY);
        if (over && !hovered) SoundEngine.PlaySound(SoundID.MenuTick);
        hovered = over;
        float hoverScale = over ? 1.2f : 1f;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, talk,
            position + talkSize * 0.5f, over ? new Color(mouseTextColor, mouseTextColor, mouseTextColor) : chatColor,
            0f, talkSize * 0.5f, new Vector2(TextScale * hoverScale));
        if (!over) return;
        Main.LocalPlayer.mouseInterface = true;
        Main.LocalPlayer.releaseUseItem = false;
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            Main.mouseLeftRelease = false;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            ModContent.GetInstance<ConversationUISystem>().ShowConversation();
        }
    }

    private static Vector2 Measure(string text) => ChatManager.GetStringSize(FontAssets.MouseText.Value,
        text ?? "", new Vector2(TextScale));
}
