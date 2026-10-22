# Current Task

- Date: 2026-10-16
- State: compilation/core tests pass; packaging blocked by sandbox sockets, gameplay untested
- Owner: orchestrator
- Verifier: independent QA_VALIDATOR agent

## Approved contract

Implement local-only single-player NPC conversations (Guide, Merchant, Nurse) through Ollama. Starter model qwen2.5:1.5b; installed-model selector, Windows advisory memory and explicit readiness test. No cloud, credentials or automatic downloads. Preserve vanilla interactions through separate Talk UI. Four authored questions, custom input capped at 300 characters, brief replies, one cancellable request and 45-second timeout.

Editable Markdown personalities; versioned bounded Markdown memories keyed by world/player/NPC type. Up to four recent exchanges, 1,200-character summary, fixed request budget. One structured response supplies dialogue, summary, bounded affinity suggestion. Affinity -100..100, max +/-2 per exchange, five cumulative movement/day, duplicate questions earn no further change that day. Merchant purchases only +/-10%, applied on shop opening. No tools or arbitrary model-directed filesystem access. Prompt injection resistance is best-effort; gameplay containment is enforced in code.

Safely archive all previous weapon work including uncommitted content before retiring it. Do not change unrelated agent guidance or historical evidence. Existing weapons become unloaded saved items; retain exact sources/assets for recovery.

Future multiplayer: explicit identities and server-owned state/inference, isolated per-player context, per-record and aggregate budgets; evict inactive text before separately capped affinity. Networking is out of scope and new behavior must be disabled in multiplayer.

## Validation

QA baseline and final: bash harness/helpers/build.sh. The installed Windows tModLoader is available read only at /mnt/c/Program Files (x86)/Steam/steamapps/common/tModLoader. Validation uses a copy at ../.local-tmodloader and a local ../tModLoader.targets wrapper. Full packaging cannot start the engine because the sandbox denies socket creation; BuildMod=false distinguishes compilation-only evidence. Meaningful standalone .NET 8 core tests for serialization/bounds, identity isolation, affinity and response validation; independent diff/lifecycle review. Manual gameplay/setup matrix remains not run absent game access. Record real evidence in harness/build_log.md.

## Implementation handoff

Implementation and setup/validation docs are present. Archived exact weapon work with checksums, retained unrelated existing changes. Standalone core checks passed 34 assertions; integration static/localization/archive checks passed. Independent final QA evidence is recorded in build_log.md. Compilation-only mandatory helper (BuildMod=false) now passes with zero warnings/errors using copied installed libraries. Normal helper still fails at engine packaging startup due to SocketException(13); no packaging or gameplay pass is claimed. Run normal Build + Reload and docs/VALIDATION.md scenarios outside the restricted sandbox before release.

## 2026-10-17 — Approved engineering-skill setup (complete)

Creator approved GitHub Issues, default triage labels, and the displayed configuration draft. Scope: append the Agent skills block to AGENTS.md; create docs/agents/{issue-tracker,triage-labels,domain}.md and docs/adr/README.md; create only missing GitHub labels. Acceptance: all five canonical labels exist, pointers resolve, and harness/decisions.md remains canonical. Documentation checks and GitHub label readback passed; no runtime code changed. Existing mod validation limits above remain unchanged.
