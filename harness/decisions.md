# Decision Log

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
