# Agent Archetypes

The orchestrator routes work by risk and deliverable. These are responsibility prompts, not autonomous permission boundaries.

| Archetype | Use for |
|---|---|
| `ORCHESTRATOR.md` | task contracts, decomposition, assignment, integration, closure |
| `ARCHITECT.md` | system boundaries, lifecycle, persistence, networking, dependencies |
| `IDEATOR.md` | player fantasy, mechanics, lore/tone proposals, vertical-slice concepts |
| `GAMEPLAY_ENGINEER.md` | tModLoader C# implementation and integration |
| `WORLD_CONTENT_DESIGNER.md` | NPCs, bosses, biomes, generation, loot/progression encounters |
| `BALANCE_DESIGNER.md` | formulas, comparisons, economy, progression and exploit analysis |
| `SPRITE_ARTIST.md` | pixel art, animation sheets, technical asset delivery |
| `UI_VFX_DESIGNER.md` | interfaces, particles, shaders, telegraphs, visual accessibility |
| `LOCALIZATION_AUDIO.md` | player text, terminology, sound/music contracts and provenance |
| `QA_VALIDATOR.md` | risk-based builds, runtime tests, reproduction and evidence |
| `REVIEWER.md` | independent code/content review and integration verdict |
| `RELEASE_STEWARD.md` | packaging, attribution, compatibility, release rehearsal |

Typical flow is orchestrator → ideator (when product intent is open) → architect/balance/content design → implementer/art/audio → reviewer + QA → orchestrator → release steward. Skip irrelevant roles, not their relevant checks.

For code work, insert a mandatory orchestrator → verifier (`QA_VALIDATOR.md`) baseline build before implementation and a verifier build after implementation. Both use `bash harness/helpers/build.sh`. See `harness/helpers/README.md` for build and sprite helpers.

Every specialist must return artifacts, verified facts, assumptions, decisions needed, validation performed, validation not performed, and next-owner recommendations. The active task and accepted decisions remain authoritative over a role prompt.
