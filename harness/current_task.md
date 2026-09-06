# Current Task

- Date: 2026-09-06
- State: review
- Owner: orchestrator / implementer
- Verifier: primary agent in a separate verification pass (no subagent tool available)

## Goal and player value

Fix Imperial Sword's missing runtime texture and prevent recurrence with ExampleMod weapon layout rules and a build preflight.

## Scope

Allowed: Content/Items/Weapons/ImperialSword.png, moving concept PNGs from assets/ to designs/, asset specifications, shared rules and affected role/pipeline/helper docs, build preflight and evidence.
Protected: gameplay C#, localization, existing project/framework settings and unrelated user changes.

## Acceptance and validation

- Exact sibling ImperialSword.cs / ImperialSword.png; namespace and class match default lookup.
- 40×40 RGBA export based on local ExampleMod/Content/Items/Weapons/ExampleSword.png; preserve concept originals.
- assets/ reserved for effects, music, sounds and shared textures; concepts under designs/.
- Build helper checks weapon PNG existence/case/header before dotnet build.
- Verify missing-file failure then repaired pass, inspect PNG, compare source hashes, run build helper and diff check.
- Client reload, inventory/world/swing checks require game access; do not claim a runtime pass.

## Decisions and handoff

ADR-004 records the creator's requested layout. Baseline build helper returned MSB4019 for missing ../tModLoader.targets. Scoped asset repair proceeds despite this pre-existing blocker. No API, balance, network or save changes.
Prior task evidence remains in harness/build_log.md.

## Result

Texture and documentation implemented. Preflight detected the original missing texture. Final checks recorded in build_log.md; client reload and final art approval remain unverified.
