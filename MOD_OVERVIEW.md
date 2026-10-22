# NewMod Overview

## Product brief

NewMod adds open-ended conversations with Terraria townsfolk, whose distinct personalities and bounded memories make relationships feel persistent. The first iteration covers Guide, Merchant and Nurse in single-player, using a local Ollama model with no API key or cloud inference.

Design pillars: distinct in-world personalities, private local computation, bounded context and deterministic gameplay limits. Personality interpretations follow vanilla characterization; generated replies are not official lore and spoiler resistance is best-effort.

## Player experience

A Talk action beside the native NPC dialogue buttons opens the conversation interface on demand, preserving vanilla chat, Shop, Heal and happiness interactions. Choose four authored questions or enter up to 300 characters. Replies target 1–3 sentences. Memory and visible affinity belong to one character/world/NPC type and survive NPC replacement. Merchant buying prices can change by up to 10%; selling prices and Nurse fees remain vanilla.

Players install Ollama and a model separately; qwen2.5:1.5b is the starter recommendation, not a guaranteed quality or performance minimum. Settings distinguish the saved preferred model from verified Ollama availability, list installed local models and offer a readiness test. Sending and readiness tests require the preferred model to appear in the refreshed local catalog. Windows hardware readings are advisory. See docs/LOCAL_SETUP.md.

## Boundaries and compatibility

.NET 8 remains required. Compilation passes against a workspace copy of the creator's installed tModLoader libraries (tModLoader stable 2026.08.3.0 / Terraria 1.4.4.9, confirmed from embedded assembly version). The Steam installation remains read only. Compilation and packaging pass using an isolated installed-version copy with the Linux native library search path and expanded execution permissions. Gameplay remains unvalidated.

Multiplayer conversation/pricing is disabled in v1. Future multiplayer will use server-owned state/inference, isolated per-player memory and bounded total storage, evicting old conversation text before separately capped relationship records. Stable identities and bounded storage seams are implemented now; networking and player authentication are deferred.

Legacy Imperial weapon sources/assets are recoverably archived, excluded from compilation and packaging. Existing saved weapons become unloaded items until restored; no destructive world/item migration is performed. The one-Dirt-Block iteration recipe policy remains for any future craftable content.
