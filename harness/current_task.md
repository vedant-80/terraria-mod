# Current Task

This is the single source of truth for work in progress. The orchestrator owns this file. Do not begin implementation until the task contract is complete enough to test.

## Status

- State: `review` (`idle | planning | active | review | blocked | done`)
- Owner: orchestrator / helper implementer
- Reviewers: primary agent self-review
- Last updated: 2026-09-05

## Goal

Add shared dotnet build and sprite resize helpers, mandatory orchestrator/verifier routing, and ExampleMod-based sprite sizing guidance. This supersedes the previous Imperial Sword implementation task; its evidence remains in build_log.md.

Current scope: `global.json`, `harness/helpers/`, agent routing/process documents, asset pipeline, validation matrix, task, decisions and build log. Preserve gameplay code and original assets. Acceptance: build helper invokes dotnet build under SDK 8 and preserves failures; resize helper preserves source, proportions and square pixel grid; agents explicitly require helper use before code and after implementation. Validate actual build invocation, resize output and invalid-input behavior. No gameplay/runtime changes are assigned.

Baseline exception: bootstrap the new build helper before its first invocation. The missing tModLoader target is a pre-existing blocker; scoped helper/documentation work may proceed. Primary agent performs distinct verifier passes because no callable subagent tool is exposed. Sections below retain the prior item task context and are not acceptance evidence for this task.

Helper result: implemented and tested; actual mod compilation remains blocked by missing `/home/vdnt/tmod/tModLoader.targets`. Build helper returns exit 1 under SDK 8.0.130. Resize tests pass; shell syntax and whitespace checks pass. No gameplay/art edits or in-game validation performed. `.gitignore` also excludes build/Python outputs created during validation. See the helper entry in build_log.md for evidence and next steps.

## Player value

The mod gains a discoverable melee weapon whose use, hit feedback, and localization can be exercised once the mod project is loaded.

## Scope

- In: `ImperialSword` C# item, minimal mod project metadata, localization, and moving `harness/assets` plus `harness/designs` to the repository root.
- Out: custom projectiles, world progression, balance expansion, final pixel-art sprite production, and unrelated harness changes.
- Allowed paths: `NewMod.csproj`, `NewMod.cs`, `build.txt`, `description.txt`, `Content/Items/Weapons/ImperialSword.cs`, `Localization/en-US.hjson`, `assets/`, `designs/`, and this task/build evidence.
- Protected paths: existing root guidance, role files, and unrelated harness documentation.

## Acceptance criteria

- [x] The project targets .NET 8 through the tModLoader project import.
- [x] `ImperialSword` registers as a melee swinging `ModItem`, has localized name/tooltip, applies On Fire on NPC hit, and has a vanilla Anvil recipe.
- [x] Asset/design references formerly under `harness/` are at the repository root with contents preserved.

## Constraints and references

- Relevant decisions: ADR-002; no approved balance canon exists.
- Relevant vanilla/tModLoader precedent: `~/tmod/ExampleMod/Content/Items/Weapons/ExampleSword.cs`.
- Multiplayer/server considerations: uses built-in item hit authority; no custom synchronized state.
- Save compatibility considerations: new stable internal item name `ImperialSword`; no save schema changes.
- Art direction references: `assets/concepts/imperial-sword-concept.png` and `assets/concepts/imperial-shard-concept.png`.

## Work plan

| ID | Deliverable | Owner/archetype | Dependencies | Validation | Status |
|---|---|---|---|---|---|
| T1 | Implement item and project scaffold | gameplay engineer | ExampleMod API inspection | compile/static review | done |
| T2 | Validate paths, localization, and diff | reviewer | T1 | repository checks | done |

## Handoffs

Record sender, recipient, artifact/path, assumptions, and requested response. Use `harness/templates/handoff.md` for larger handoffs.

## Evidence and result

- Commands run: `dotnet build NewMod.csproj --no-restore` (blocked by missing `/home/vdnt/tmod/tModLoader.targets`); path/localization scans completed.
- Manual checks: not run; no game launch performed.
- Screenshots/captures: concept reference inspected; no in-game capture.
- Known limitations: no final Terraria-sized item sprite exists; project build may require a local tModLoader installation target.
- Final disposition: implementation complete; compile and in-game validation blocked until tModLoader is installed/configured.
