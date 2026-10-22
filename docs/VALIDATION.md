# Conversation validation

## Automated checks

Run from NewMod:

```sh
DOTNET_CLI_HOME=/tmp/tmod-dotnet-home dotnet run --project tests/ConversationCore/ConversationCore.csproj --configfile tests/NuGet.Config
DOTNET_CLI_HOME=/tmp/tmod-dotnet-home bash harness/helpers/build.sh
python3 harness/helpers/check_weapon_assets.py
git diff --check
```

The standalone core project uses actual .NET 8, no Terraria stubs and no NuGet dependencies. It exercises affinity/day/duplicate caps, identity isolation, bounded history and prompt bytes, atomic persistence staging, text-first eviction, malformed/oversized/future-file preservation, model discovery, local endpoint/remote-model rejection and independent metadata validation. These checks do not prove tModLoader compatibility, live model quality or gameplay.

## Manual acceptance checklist (not yet run)

1. Build/load against an installed, recorded tModLoader/Terraria version. Load existing character/world with legacy ImperialSword: expect unloaded item, no deletion/migration. Recover matching source/assets from archive if desired.
2. Verify that a saved qwen2.5:1.5b preference is labeled not verified until discovery succeeds. With Ollama missing/stopped, settings must report it unreachable after refresh and must not imply the preferred model is installed. With an empty catalog, show a reachable runtime and no installed models. Stop Ollama after successful discovery and retry; stale model entries must clear on connection failure. Sending/testing before verification must show an actionable setup message without inference. On clean Windows installation follow LOCAL_SETUP.md. Test missing/stopped Ollama, no models, selected model removed, another installed local model, cloud alias rejection, failed structured output and successful readiness latency.
3. Visit Guide/Merchant/Nurse: right-click shows Talk in the native dialogue button row, with no floating launcher or conversation panel. Click Talk to open the conversation. Normal vanilla buttons still work. Other NPCs show no Talk button. Test four suggested questions, progression-dependent fourth question, typing after clicking the input, backspace, 300-character typing/paste, Unicode, long wrapped replies, narrow resolution, high UI scale, mouse scroll and keyboard focus release on clicking outside, Send, Cancel, Escape, close, settings, NPC switch, death and world exit. Verify Talk placement beside long Nurse prices and localized labels, including when Happiness is absent in a remix world. No gamepad-specific navigation is promised in this preview.
4. Turn autopause on: reply/status still arrive. Cancel mid-request, close panel, switch NPCs, die or leave world; no late reply/affinity/memory appears. A reply already accepted may finish its background save.
5. Save/reload and replace an NPC: its relationship remains. Another character/world cannot see that memory. Switch models: relationship remains. Reset confirmation must affect only current character/world/NPC.
6. Exercise daily movement both directions, repeated normalized questions and next dawn. Affinity remains -100..100, +/-2/exchange and five cumulative/day. Edits to saved text never become executable commands.
7. Merchant prices at -100/0/+100: compare newly opened stock including 1–9 copper items and existing custom coin prices. Reopen repeatedly: no compounding. Compare happiness/discount accessories, sell prices and buyback inventory. Nurse fees remain unchanged. Reopen after initial asynchronous affinity load before expecting saved discounts.
8. Try malformed/oversized/future-version memory, inaccessible folder and full disk. Files should be preserved, errors visible and reset explicit. Accepted save failures must not be advertised as durable memory. Exercise text/relationship eviction with reduced budgets in the core test, without altering real saves.
9. Try direct and remembered prompt injections, fake JSON, instructions to increase affinity and off-progression lore requests. Record dialogue failures honestly; prove effect limits and no tools/filesystem actions, without claiming perfect model resistance.
10. Host/client and dedicated-server smoke test: no Talk/settings inference or relationship price changes, no graphics initialization on dedicated server. Multiplayer functionality itself is deferred.

Previous evidence (2026-10-16): compilation-only helper with `BuildMod=false` passes against the copied installed tModLoader libraries with zero warnings/errors, and 34 core assertions pass. The normal helper reaches C# success, then engine packaging crashes on sandbox-denied socket creation. Release remains blocked until packaging and relevant gameplay checks pass outside this restriction. Record exact results in harness/build_log.md.

## 2026-10-22 UI implementation notes

Talk uses the installed `On_Main.DrawNPCChatButtons` detour. Vanilla still draws its own actions; the additional action uses its font, coordinate system and same row. The row anchor, 0.9 text measurement, 260-pixel first-action cap and 30-pixel gaps were checked against the installed 2026.08 assembly. These are version-sensitive implementation details, so recheck after tModLoader upgrades. No controller navigation slot is added in this preview.

Keyboard input follows the official tModLoader 1.4.4 `UIInputTextField.DrawSelf` lifecycle (`WritingText`, `HandleIME`, `GetInputText`). Focus and input blocking remain in update, with release on UI transitions. Compilation and core checks cannot prove in-game typing or layout; the manual scenarios above remain required.

API references: [official Main patch](https://github.com/tModLoader/tModLoader/blob/1.4.4/patches/tModLoader/Terraria/Main.cs.patch), [official text field](https://github.com/tModLoader/tModLoader/blob/1.4.4/patches/tModLoader/Terraria/ModLoader/UI/UIInputTextField.cs). The expected local `~/tmod/ExampleMod/` checkout was unavailable during this change.

2026-10-22 validation: the normal build helper now also packages successfully (zero warnings/errors) with pinned SDK 8.0.130, the isolated installed-version copy under `/tmp/newmod-tmodloader`, its runtime probing configuration and Linux native library search path. The sandbox-only run still fails on socket creation; execution with expanded permissions resolves that restriction. Core checks pass 41 assertions. This supersedes the older packaging limitation above for this environment; gameplay/manual acceptance remains unrun.
