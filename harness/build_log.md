# Build and Validation Log

Append evidence for meaningful validation runs. Keep failures: they are useful diagnostic history. Do not claim a check passed unless its command or reproducible manual procedure is recorded.

## Entry format

### YYYY-MM-DD HH:MM — task/commit

- Agent:
- Environment: OS, .NET SDK, tModLoader/Terraria version when known
- Change under test:
- Command or procedure:
- Result: `pass | fail | blocked | not run`
- Evidence: concise output, screenshot path, log path, or observed behavior
- Warnings/follow-up:

## Runs

### 2026-09-06 — Imperial Sword texture repair and default weapon layout

- Agent: primary implementer with separate verifier review pass; no independent subagent invoked.
- Baseline: `bash harness/helpers/build.sh` failed with MSB4019 (missing `/home/vdnt/tmod/tModLoader.targets`); this pre-existing blocker does not prevent scoped asset repair.
- Regression evidence: `python3 harness/helpers/check_weapon_assets.py` reported `Missing weapon texture: Content/Items/Weapons/ImperialSword.png` before export. After export the same preflight, invoked through the build helper, passed.
- Export: existing resize helper with `--reference /home/vdnt/tmod/ExampleMod/Content/Items/Weapons/ExampleSword.png --pixel-size 1`; Pillow full decode verified 40×40 RGBA and content bounds (0,0,39,40). Native image visually inspected. See designs/weapons/ImperialSword.md.
- Preservation: SHA256 before/after moving both concept files to designs/concepts matched: sword ba79542e8af823b6c25be683edceb41a03a42d1750168927459b122fcf76c500; shard 83c8dc561b44a06c6d2c93021e94687f9e26e09d3dea5c78503373c2ab9e6405.
- Final build: asset preflight passed; dotnet compilation still blocked by MSB4019. `git diff --check` passed. Reviewed changed files and default class/namespace alignment; no gameplay C# changed.
- Not run: client reload, inventory/world/swing, 4× and bright/dark visual tests. Restore the compatible installed tModLoader targets, run the helper, then Build + Reload and inspect Imperial Sword in those contexts. This repairs the confirmed missing file; it is not evidence of a successful game load or final art approval.

### 2026-09-05 — Build verifier and sprite helpers

- Agent: primary agent, separate orchestrator/implementation/verifier review passes; no independent subagent tool available
- Environment: Ubuntu, .NET SDK 8.0.130, Python 3.13, Pillow 12.3.0 in `/tmp/newmod-sprite-tools`
- Changes: `harness/helpers/`, SDK selection, build-output ignores, agent routing and sprite process documentation
- Baseline/build: `bash harness/helpers/build.sh` initially failed on SDK first-run sentinel write permissions. Retried with approved elevation; actual `dotnet build` returned exit 1, MSB4019 for missing `/home/vdnt/tmod/tModLoader.targets`. No compilation pass claimed.
- Dependency setup: initial Pillow installation failed on sandbox DNS; approved retry installed Pillow in the temporary environment.
- Resize validation: `/tmp/newmod-sprite-tools/bin/python -m unittest discover -s harness/helpers -p 'test_*.py'` passed; covers aspect/padding, dimensions, RGBA, square 2×2 clusters, source preservation, existing-output refusal, invalid grids and empty input.
- Static validation: `bash -n harness/helpers/build.sh` and `git diff --check` passed. Reviewed helper files, agent instructions, local ExampleMod PNG dimensions and ExampleSword C# size fields. Gameplay, networking, localization and runtime checks are N/A for helper-only changes; in-game sprite validation not run because no asset is integrated.
- Review outcome: helper behavior verified; mod compile remains blocked. Restore/configure the installed tModLoader target and assemblies at the project's import location, then rerun `bash harness/helpers/build.sh`. Generated build files are ignored; original code and concept assets preserved.

### 2026-09-05 — initial agent harness

- Agent: primary harness implementer
- Environment: repository documentation only; no mod project/toolchain exists yet
- Change under test: root guidance, role archetypes, operational files, and templates
- Command or procedure: enumerated all Markdown through depth three; checked every file was non-empty and had an H1; searched for stale stub/TODO/FIXME text; manually audited cross-references and role coverage
- Result: `pass`
- Evidence: 28 non-empty Markdown documents found; all have H1 headings; no stale architect stub, TODO, or FIXME remains. Remaining `TBD` fields are intentional creator-owned product decisions in `MOD_OVERVIEW.md`.
- Warnings/follow-up: no `.csproj`, mod source, assets, or tModLoader version exists yet, so compile and in-game validation are not applicable. Complete Phase 0 before implementation.

### 2026-09-05 — API reference and runtime policy

- Agent: primary harness implementer
- Environment: local `/home/vdnt/tmod/ExampleMod` directory present
- Change under test: .NET and tModLoader API research instructions
- Command or procedure: verified the ExampleMod directory exists; scanned shared rules, context map, validation guidance, and engineering/reviewer roles for the required policy
- Result: `pass`
- Evidence: .NET 8 is stated as required; ExampleMod is the first C# API reference; `https://docs.tmodloader.net/docs/stable/` is the documented fallback.
- Warnings/follow-up: the exact tModLoader/Terraria project version still needs to be established when the mod project is initialized.

### 2026-09-05 — Imperial Sword implementation

- Agent: gameplay engineer
- Environment: Ubuntu; .NET SDK 8.0.130 installed; tModLoader target version unavailable
- Change under test: root project scaffold, `ImperialSword` item, localization, and moved concept references
- Command or procedure: `dotnet build NewMod.csproj --no-restore`; scanned source/localization/path references; inspected complete working-tree status and relevant diff
- Result: `blocked`
- Evidence: build failed with `MSB4019`: `/home/vdnt/tmod/tModLoader.targets` was not found. Static scans found the item/localization references and both PNGs at `assets/concepts/`.
- Warnings/follow-up: install/configure tModLoader so `../tModLoader.targets` resolves, then rebuild and exercise obtaining, swinging, hitting, reforging, and multiplayer loading in-game. No final Terraria item sprite is included; the moved sword PNG remains concept art.
