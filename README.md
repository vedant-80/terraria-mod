# NewMod — local NPC conversations

A single-player Terraria/tModLoader preview for open-ended conversations with the Guide, Merchant and Nurse. Uses local Ollama models, editable personalities and bounded per-character/world memories. No API keys or cloud inference. Relationships can adjust Merchant purchase prices within +/-10%.

- [Install a local model and configure the mod](docs/LOCAL_SETUP.md)
- [Validation and known release blockers](docs/VALIDATION.md)
- [Core tests without Terraria](tests/README.md)
- [Project direction](MOD_OVERVIEW.md)
- [Recover retired weapon content](archive/imperial-weapons-2026-10-16/README.md)

.NET 8 is required. Compilation against a workspace copy of the installed tModLoader libraries passes with zero warnings/errors; all 34 standalone core assertions pass. Full packaging is blocked here by sandbox socket restrictions during tModLoader startup, and in-game validation remains unrun. Multiplayer features are disabled in this iteration; future ownership is designed around server-hosted inference and per-player relationships.
