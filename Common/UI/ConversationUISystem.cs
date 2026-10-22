using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NewMod.Common.UI;

// Based on ExampleMod's ExampleCoinsUISystem: one client interface, drawn at UI scale.
[Autoload(Side = ModSide.Client)]
public sealed class ConversationUISystem : ModSystem
{
    private UserInterface interfaceLayer;
    private ConversationUI state;
    private GameTime lastUpdate;

    internal bool ConversationVisible => state?.Visible == true;

    public void ShowConversation()
    {
        if (!Main.dedServ && !Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer)
        {
            ConversationRuntime.Current?.Update();
            state?.ShowConversation();
        }
    }

    public override void PostSetupContent()
    {
        interfaceLayer = new UserInterface();
        state = new ConversationUI();
        state.Activate();
        interfaceLayer.SetState(state);
    }

    public void ShowSettings()
    {
        if (!Main.dedServ && !Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer)
            state?.ShowSettings();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdate = gameTime;
        if (Main.gameMenu || Main.netMode != NetmodeID.SinglePlayer)
        {
            state?.Close();
            return;
        }
        ConversationRuntime.Current?.Update();
        if (Main.LocalPlayer.dead)
        {
            state?.Close();
            return;
        }
        interfaceLayer?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (index < 0)
            return;
        layers.Insert(index, new LegacyGameInterfaceLayer("NewMod: NPC Conversations", () =>
        {
            if (!Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer && !Main.LocalPlayer.dead)
                interfaceLayer?.Draw(Main.spriteBatch, lastUpdate ?? new GameTime());
            return true;
        }, InterfaceScaleType.UI));
    }

    public override void OnWorldUnload() => state?.Close();

    public override void Unload()
    {
        state?.ReleaseInput();
        interfaceLayer?.SetState(null);
        state = null;
        interfaceLayer = null;
    }
}
