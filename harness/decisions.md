# Decision Log

### ADR-005: Imperial thrust/shard iteration and Dirt Block recipes

- Date: 2026-09-06
- Status: accepted
- Deciders: creator request; implementer selects reversible test values
- Decision: ImperialSword uses a dagger-style projectile thrust with one delayed ImperialShard projectile. Shard is not an inventory item. All craftable NewMod items now and going forward use exactly one Dirt Block, no station or condition, yielding one item until creator ends test policy.
- Implementation: retain 50 base damage and 20-tick cadence; thrust lasts 16 ticks, releases shard at tick 8. Shard gets 40% inherited damage, 4 pixels/tick, 24-tick lifetime (at most 96 pixels), continuous fade, tile collision, one hit per NPC. These values bound the requested short after-effect and are not approved progression balance.
- Authority: default owner-spawned item projectile; only owning player spawns child, synced ai[1] emission flag. Normal projectile synchronization, no custom packets/saves. Transient projectiles expire; no reconnect persistence.
- Compatibility: stable ImperialSword internal name, existing texture/default layout; add default sibling projectile textures. Thrust replaces item contact damage to avoid double hits. Fire debuff moves to thrust's OnHitNPC.
- Validation: local ExampleShortsword/ExampleShortswordProjectile, ExampleFlailProjectile spawning and immunity patterns; baseline/final helper, asset checks and documented runtime matrix.

### ADR-004: Default ExampleMod weapon resource layout

- Date: 2026-09-06
- Status: accepted
- Deciders: repository creator
- Context: ImperialSword reload failed because its primary texture was absent from the default resource path.
- Decision: Keep weapon C# and same-name PNG together under Content/Items/Weapons, matching class/namespace to path. No primary weapon Texture relocation override. Reserve assets/ for effects, music, sounds and shared textures; concepts and specs go in designs/. Run a sibling PNG preflight in the build helper.
- Consequences: ImperialSword receives a 40×40 export from the creator-provided concept. Original concepts move unchanged to designs/concepts. Runtime appearance remains subject to client testing; external provenance is unverified.
- Validation: missing-file regression, PNG decode/dimensions, source hashes, build helper and client reload when available.
- Supersedes: none.

Record choices that constrain future work. The architect owns architectural decisions; the orchestrator ensures entries are written. Never rewrite history: supersede an old entry with a new one.

## Entry format

### ADR-NNN: Short title

- Date: YYYY-MM-DD
- Status: `proposed | accepted | rejected | superseded`
- Deciders:
- Context: facts, constraints, and the problem—not the preferred answer.
- Options: credible alternatives and tradeoffs.
- Decision: the selected option.
- Consequences: benefits, costs, migration, compatibility, and follow-up work.
- Validation: how the decision will be proven.
- Supersedes / superseded by: ADR identifier or none.

## Decisions

### ADR-001: Layer agent guidance by stability and responsibility

- Date: 2026-09-05
- Status: `accepted`
- Deciders: repository creator request; harness implementer
- Context: Contributors need shared product/rule context, specialized operating roles, and mutable execution evidence without mixing those concerns.
- Options: one monolithic prompt; tool-specific hidden prompts; repository-native layered Markdown.
- Decision: Keep durable project rules/product intent at the repository root, role contracts in `agents/`, and operational state, validation, pipelines, and templates in `harness/`.
- Consequences: Agents can discover stable rules and load only relevant specialist detail. Maintainers must keep cross-links and the truth hierarchy consistent.
- Validation: audit all three layers for discoverability, complete role routing, non-empty content, and valid relative paths.
- Supersedes / superseded by: none.

### ADR-002: Use .NET 8 and ExampleMod-first API research

- Date: 2026-09-05
- Status: `accepted`
- Deciders: repository creator
- Context: Agents need a consistent runtime target and a reliable way to resolve tModLoader C# API and lifecycle questions.
- Options: agent memory/third-party examples; official documentation only; local ExampleMod-first research with official stable documentation as fallback.
- Decision: Target .NET 8. Search `~/tmod/ExampleMod/` first for concrete tModLoader C# patterns, then use the [stable tModLoader API documentation](https://docs.tmodloader.net/docs/stable/) when necessary.
- Consequences: API choices should be grounded in working official examples and agents must identify version-sensitive differences. Project setup and validation must reject unintended framework retargeting.
- Validation: engineering and review handoffs cite the ExampleMod pattern or stable documentation used; project files and SDK evidence confirm .NET 8.
- Supersedes / superseded by: none.

The absence of another entry is not permission to invent project canon.

### ADR-003: Shared build verification and sprite resizing

- Date: 2026-09-05
- Status: `accepted`
- Deciders: repository creator request
- Context: Code changes need consistent baseline and final compilation; sprite exports need dimensions grounded in local examples.
- Decision: Orchestrator invokes the existing QA_VALIDATOR as verifier before code additions/edits and after implementation. Verifier always uses `harness/helpers/build.sh`. Pin .NET SDK 8 via global.json. Sprite artist uses `harness/helpers/resize_asset.py` with a comparable reference canvas and an explicit square pixel grid.
- Consequences: Failed builds remain failures; baseline blockers may permit documented scoped repairs/helper work. Agent instructions require the routing but are not an automated execution scheduler. Resizing requires Pillow and manual art review. No gameplay, save or network contracts change.
- Validation: actual build invocation and status; automated image geometry, grid and overwrite checks; agent instruction review.
- Supersedes / superseded by: none.

### ADR-006: Local NPC conversations and bounded relationship ownership

- Date: 2026-10-16
- Status: accepted
- Deciders: creator approved interview and implementation plan.
- Context: Open-ended NPC conversation must avoid API charges/credentials, bound memory and gameplay effects, and leave a path to multiplayer.
- Options: hosted free-tier proxy versus local inference; client-owned versus future server-owned relationships; unlimited history versus bounded summary and recent turns.
- Decision: Local-only Ollama, editable personalities, bounded versioned Markdown memory addressed by world/player/NPC type. Separate UI, transport, persistence and relationship rules. Single-player first; explicitly disable conversation and pricing in multiplayer. Future host owns inference, memory and economy; total budgets evict old text before separately capped affinity records.
- Consequences: Players install a runtime/model; hardware and roleplay quality vary. Models never execute tools or access secrets. Output is untrusted; deterministic limits contain affinity effects, without promising perfect injection immunity. Preserve and archive legacy weapons outside compilation/packaging; existing saved items become unloaded items. No networking or cloud service added.
- Validation: baseline/final build helper, standalone core boundary/failure tests, independent review and documented manual gameplay checks.
- Supersedes: ADR-005 weapon feature direction only; iteration recipe policy remains for any future craftable items.
