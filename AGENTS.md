# NewMod Agent Rules

These instructions apply to every human or AI contributor in this repository. Read this file, `MOD_OVERVIEW.md`, `ROADMAP.md`, `harness/current_task.md`, and the relevant role file in `agents/` before changing anything. More-specific instructions may add constraints but may not relax these rules.

## Core contract

- Preserve player intent, existing behavior, and user-authored changes outside the assigned scope.
- Verify facts in the repository. Label unverified claims as assumptions; never invent lore, balance targets, APIs, test results, or project conventions.
- Work from a bounded task with player value, allowed scope, acceptance criteria, and a validation plan. The orchestrator maintains the active contract.
- Prefer the smallest coherent implementation. Avoid speculative frameworks, unrelated cleanup, and premature content expansion.
- Ask for a decision only when choices materially change player experience, compatibility, architecture, licensing, or scope. Otherwise make a reversible, documented assumption.
- Do not report success without evidence. A build proves compilation, not correct gameplay; manual play proves an example, not every invariant.

## Repository safety

- Inspect status and nearby patterns before editing. Treat pre-existing changes as belonging to the user.
- Do not discard, overwrite, regenerate, rename, or mass-format unrelated work.
- Do not commit generated build output, secrets, local configuration, decompiled proprietary code/assets, or unlicensed media.
- Do not perform destructive Git/filesystem operations or publish releases without explicit authorization.
- Keep stable internal content names unless a migration plan is accepted. Consider existing worlds, characters, configs, and multiplayer peers.

## Terraria/tModLoader engineering

- Use .NET 8 for the mod, its projects, build instructions, and compatible tooling. Do not silently retarget to another .NET version.
- For tModLoader C# API usage, inspect the local `~/tmod/ExampleMod/` source first. Prefer its demonstrated hooks, lifecycle patterns, content structure, and API usage over recalled examples or generic web results, while still confirming that the example applies to this mod's installed tModLoader version.
- If ExampleMod does not answer the question, consult the [stable tModLoader API documentation](https://docs.tmodloader.net/docs/stable/). Prefer official tModLoader sources over third-party tutorials. Record any version-sensitive assumption in the task or handoff.
- Confirm the repository’s target tModLoader, Terraria, and dependency versions before selecting APIs. Project-local established patterns take precedence when they are compatible with the required behavior and current API.
- Keep gameplay state authoritative and deterministic enough for multiplayer. Explicitly define who owns state, what is synchronized, and behavior for late joins/reconnects.
- Guard graphics, audio, input, UI, and other client-only behavior from dedicated servers.
- Avoid allocations, global scans, file/network I/O, reflection, and noisy logs in update/draw/AI loops.
- Respect lifecycle semantics: registration, loading/unloading, cloning, saving/loading, world entry/exit, death/respawn, and hot reload where relevant.
- Use localization keys for player-facing text. Tooltips must describe real mechanics without concealing essential conditions.
- Keep balance values named and justified against the intended progression point and comparable content.
- Keep assets lossless, correctly framed, case-correct, and documented. Validate pixel art in motion and in its actual Terraria contexts.
- Follow ExampleMod's default weapon layout: `Content/Items/Weapons/<Name>.cs` and `<Name>.png` sit together, with class name and namespace matching the path. Do not override `Texture` to relocate the primary weapon sprite. Reserve `assets/` for effects, music, sounds, and shared textures; keep concept/reference art in `designs/`. Verify every weapon's matching PNG before integration.

## Role and handoff protocol

The orchestrator owns decomposition and integration; it does not waive specialist review. Each assignment states inputs, allowed paths, outputs, acceptance criteria, dependencies, and required evidence. Recipients return changed paths, decisions, tests, failures, limitations, and recommended follow-up. Use the templates in `harness/templates/`.

Agents must not silently expand scope. Cross-cutting architecture, save formats, public contracts, core progression, art direction, or network protocols require an accepted entry in `harness/decisions.md`.

## Completion gate

Before completion:

1. Re-read the active contract and inspect the complete diff.
2. Run every applicable check in `harness/validation-matrix.md`.
3. Apply `harness/review-checklist.md`, using a reviewer independent of implementation when risk warrants it.
4. Append real results to `harness/build_log.md`; explicitly list anything not run.
5. Update decisions and project documentation affected by the change.
6. Leave the working tree understandable: no unexplained artifacts, placeholders, or debug behavior.

If required tooling, game access, credentials, or user decisions are unavailable, report the precise blocker and a reproducible next step. “Not run” and “blocked” are valid outcomes; they are not passes.
