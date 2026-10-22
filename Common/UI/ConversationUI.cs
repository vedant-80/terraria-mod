using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;

namespace NewMod.Common.UI;

internal sealed class ConversationUI : UIState
{
    private UIPanel panel;
    private UITextPanel<string> launcher;
    private UIList contents;
    private UIText status;
    private UIText reply;
    private UIText affinity;
    private UIText model;
    private ConversationInput input;
    private bool visible;
    private bool settings;
    private bool questionsOpen;
    private bool modelsOpen;
    private bool confirmReset;
    private int conversationNpc = -1;
    private int settingsNpc = -1;
    private string draft = "";
    private string lastStatus;
    private string lastReply;
    private string lastModel;
    private int lastAffinity = int.MinValue;
    private float screenWidth;
    private float screenHeight;
    private object lastModels;
    private int lastModelCount;

    internal static string Text(string key) => Language.GetTextValue("Mods.NewMod.Chat." + key);

    public override void OnInitialize()
    {
        launcher = new UITextPanel<string>(Text("Talk"), 0.85f);
        launcher.Width.Set(150, 0);
        launcher.Height.Set(36, 0);
        launcher.HAlign = 1;
        launcher.Left.Set(-20, 0);
        launcher.Top.Set(260, 0);
        launcher.OnLeftClick += (_, _) => ShowConversation();
        Append(launcher);
    }

    public void ShowSettings()
    {
        settings = true;
        settingsNpc = Main.LocalPlayer.talkNPC;
        visible = true;
        confirmReset = false;
        BuildPanel();
        ConversationRuntime.Current?.RefreshModels();
    }

    private void ShowConversation()
    {
        if (ConversationRuntime.Current is not { NpcKey.Length: > 0 })
            return;
        conversationNpc = Main.LocalPlayer.talkNPC;
        settings = false;
        visible = true;
        questionsOpen = false;
        confirmReset = false;
        BuildPanel();
    }

    public void Close()
    {
        if (visible)
            ConversationRuntime.Current?.Cancel();
        visible = false;
        ReleaseInput();
        panel?.Remove();
        panel = null;
        input = null;
        draft = "";
        confirmReset = false;
    }

    public void ReleaseInput() => input?.Blur();

    private void RememberDraft()
    {
        if (input != null)
            draft = input.Value;
        ReleaseInput();
    }

    private void BuildPanel()
    {
        RememberDraft();
        panel?.Remove();
        input = null;
        status = reply = affinity = model = null;
        panel = new UIPanel { HAlign = 0.5f, VAlign = 0.5f, BackgroundColor = new Color(35, 45, 74) };
        panel.Width.Set(Math.Min(650, Math.Max(250, Main.screenWidth / Main.UIScale - 32)), 0);
        panel.Height.Set(Math.Min(590, Math.Max(180, Main.screenHeight / Main.UIScale - 32)), 0);
        panel.SetPadding(14);
        Append(panel);

        var heading = new UIText(Text(settings ? "Settings" : "Talk"), 1.1f);
        panel.Append(heading);
        var close = new UITextPanel<string>(Text("Close"), 0.75f) { HAlign = 1 };
        close.Width.Set(90, 0);
        close.Height.Set(30, 0);
        close.OnLeftClick += (_, _) => Close();
        panel.Append(close);
        contents = new UIList { ListPadding = 10, ManualSortMethod = _ => { } };
        contents.Top.Set(42, 0);
        contents.Width.Set(-24, 1);
        contents.Height.Set(-42, 1);
        panel.Append(contents);
        var scrollbar = new UIScrollbar();
        scrollbar.Top.Set(42, 0);
        scrollbar.Width.Set(20, 0);
        scrollbar.Height.Set(-42, 1);
        scrollbar.HAlign = 1;
        scrollbar.SetView(100, 1000);
        panel.Append(scrollbar);
        contents.SetScrollbar(scrollbar);

        status = AddText(ConversationRuntime.Current?.Status ?? Text("Unavailable"));
        if (settings)
            BuildSettings();
        else
            BuildConversation();
        lastStatus = lastReply = lastModel = null;
        lastAffinity = int.MinValue;
        lastModels = ConversationRuntime.Current?.Models;
        lastModelCount = ConversationRuntime.Current?.Models.Count ?? 0;
        Recalculate();
    }

    private void BuildConversation()
    {
        var runtime = ConversationRuntime.Current;
        affinity = AddText("");
        Button(Text("SuggestedQuestions"), () => { questionsOpen = !questionsOpen; BuildPanel(); });
        if (questionsOpen && runtime != null)
        {
            for (int i = 0; i < 4; i++)
            {
                string question = Text("Questions." + runtime.NpcKey + ".Q" + i + (i == 3 && Main.hardMode ? "Hardmode" : ""));
                Button(question, () => { draft = question; if (input != null) input.SetValue(question); questionsOpen = false; BuildPanel(); });
            }
        }
        AddText(Text("InputHint"));
        input = new ConversationInput(draft);
        contents.Add(input);
        input.ValueChanged = () => contents.Recalculate();
        Button(Text("Send"), Send);
        Button(Text("Cancel"), () => { ReleaseInput(); runtime?.Cancel(); });
        reply = AddText(runtime?.Reply ?? "");
        Button(Text("Settings"), ShowSettings);
    }

    private void BuildSettings()
    {
        var runtime = ConversationRuntime.Current;
        AddText(Text("SetupInstructions"));
        model = AddText("");
        Button(Text("SelectModel"), () => { modelsOpen = !modelsOpen; BuildPanel(); });
        if (modelsOpen && runtime != null)
        {
            foreach (string name in runtime.Models)
            {
                string modelName = name;
                Button(modelName, () => { runtime.SelectModel(modelName); modelsOpen = false; BuildPanel(); });
            }
            if (runtime.Models.Count == 0)
                AddText(Text("NoModels"));
        }
        Button(Text("RefreshModels"), () => { runtime?.RefreshModels(); modelsOpen = true; BuildPanel(); });
        Button(Text("TestModel"), () => runtime?.TestModel());
        Button(Text("Cancel"), () => runtime?.Cancel());
        AddText(runtime?.HardwareDescription ?? Text("MemoryUnavailable"));
        Button(Text("OpenPersonalities"), () => runtime?.OpenFolder(true));
        Button(Text("OpenMemory"), () => runtime?.OpenFolder(false));
        if (runtime is { NpcKey.Length: > 0 })
        {
            Button(Text(confirmReset ? "ConfirmReset" : "Reset"), () =>
            {
                if (confirmReset)
                {
                    runtime.ResetCurrent();
                    confirmReset = false;
                }
                else
                    confirmReset = true;
                BuildPanel();
            });
            Button(Text("BackToTalk"), ShowConversation);
        }
    }

    private void Send()
    {
        RememberDraft();
        if (!string.IsNullOrWhiteSpace(draft))
            ConversationRuntime.Current?.Send(draft);
    }

    private UIText AddText(string value)
    {
        var label = new UIText(value, 0.85f) { IsWrapped = true, TextOriginX = 0 };
        label.Width.Set(-4, 1);
        contents.Add(label);
        return label;
    }

    private void Button(string label, Action click)
    {
        var button = new UITextPanel<string>(label, 0.75f);
        button.Width.Set(-4, 1);
        button.Height.Set(36, 0);
        button.OnLeftClick += (_, _) => click();
        contents.Add(button);
    }

    public override void Update(GameTime gameTime)
    {
        var runtime = ConversationRuntime.Current;
        bool supportedNpc = runtime is { NpcKey.Length: > 0 } && Main.LocalPlayer.talkNPC >= 0;
        if (!visible && supportedNpc && launcher.Parent == null)
            Append(launcher);
        if ((visible || !supportedNpc) && launcher.Parent != null)
            launcher.Remove();
        if (visible && (!settings && (!supportedNpc || conversationNpc != Main.LocalPlayer.talkNPC)))
            Close();
        if (visible && Main.keyState.IsKeyDown(Keys.Escape) && Main.oldKeyState.IsKeyUp(Keys.Escape))
            Close();
        if (visible && settings && settingsNpc != Main.LocalPlayer.talkNPC)
        {
            settingsNpc = Main.LocalPlayer.talkNPC;
            confirmReset = false;
            BuildPanel();
        }
        float width = Main.screenWidth / Main.UIScale;
        float height = Main.screenHeight / Main.UIScale;
        if (visible && (screenWidth != width || screenHeight != height))
        {
            screenWidth = width;
            screenHeight = height;
            BuildPanel();
        }
        if (visible && settings && modelsOpen && runtime != null &&
            (!ReferenceEquals(lastModels, runtime.Models) || lastModelCount != runtime.Models.Count))
            BuildPanel();
        if ((visible && panel.IsMouseHovering) || (launcher.Parent != null && launcher.IsMouseHovering))
        {
            Main.LocalPlayer.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("NewMod: NPC Conversations");
        }
        if (visible && runtime != null)
        {
            bool changed = false;
            if (lastStatus != runtime.Status) { lastStatus = runtime.Status; status.SetText(lastStatus); changed = true; }
            if (reply != null && lastReply != runtime.Reply) { lastReply = runtime.Reply; reply.SetText(lastReply); changed = true; }
            if (affinity != null && lastAffinity != runtime.Affinity) { lastAffinity = runtime.Affinity; affinity.SetText(Language.GetTextValue("Mods.NewMod.Chat.Affinity", lastAffinity)); changed = true; }
            if (model != null && lastModel != runtime.SelectedModel) { lastModel = runtime.SelectedModel; model.SetText(Language.GetTextValue("Mods.NewMod.Chat.CurrentModel", lastModel)); changed = true; }
            if (changed) contents.Recalculate();
        }
        base.Update(gameTime);
    }
}

internal sealed class ConversationInput : UIPanel
{
    private readonly UIText text;
    private bool focused;
    public string Value { get; private set; }
    public Action ValueChanged { get; set; }

    public ConversationInput(string initial)
    {
        Width.Set(-4, 1);
        Height.Set(130, 0);
        BackgroundColor = new Color(20, 25, 40);
        text = new UIText("", 0.8f) { IsWrapped = true, TextOriginX = 0 };
        text.Width.Set(0, 1);
        Append(text);
        SetValue(initial);
        OnLeftClick += (_, _) =>
        {
            focused = true;
            Main.clrInput();
        };
    }

    public void SetValue(string value)
    {
        Value = value.Length > 300 ? value[..300] : value;
        text.SetText(Value + "\n" + Value.Length + "/300");
        Recalculate();
        ValueChanged?.Invoke();
    }

    public override void Recalculate()
    {
        base.Recalculate();
        if (text == null) return;
        // The entire panel scrolls, including a long question at high UI scale.
        float height = Math.Max(100, text.GetDimensions().Height + PaddingTop + PaddingBottom);
        if (Height.Pixels != height)
        {
            Height.Set(height, 0);
            base.Recalculate();
        }
    }

    public void Blur()
    {
        if (!focused) return;
        focused = false;
        Main.blockInput = false;
        PlayerInput.WritingText = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (focused && Main.mouseLeft && Main.mouseLeftRelease && !IsMouseHovering)
            Blur();
        if (focused)
        {
            Main.blockInput = true;
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();
            string next = Main.GetInputText(Value).Replace("\r", "").Replace("\n", " ");
            if (next != Value)
                SetValue(next);
        }
        base.Update(gameTime);
    }
}
