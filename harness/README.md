# NewMod Agent Harness

This directory holds the operational state, templates, and validation rules used by AI agents contributing to NewMod. Project-wide intent and immutable rules live at the repository root; role prompts live in `agents/`; mutable execution state lives here.

## Control flow

1. Read root `AGENTS.md`, `MOD_OVERVIEW.md`, and `ROADMAP.md`.
2. The orchestrator turns a request into a testable contract in `current_task.md`.
3. The architect is consulted for cross-system contracts, persistent data, networking, or structural changes.
4. Specialists receive bounded work through a handoff containing inputs, allowed paths, outputs, and validation.
5. Implementers make the smallest coherent change and return evidence, not just a success claim.
6. A reviewer checks the diff independently against `review-checklist.md` and `validation-matrix.md`.
7. The orchestrator integrates, updates `decisions.md` and `build_log.md`, and closes or revises the task.

One agent may fill several roles on small tasks, but it must still apply each role’s checks. A role name is a responsibility boundary, not permission to edit everything.

## Files

- `current_task.md`: only active task and decomposition.
- `decisions.md`: durable architectural/product decisions.
- `build_log.md`: append-only validation evidence.
- `review-checklist.md`: completion gate.
- `validation-matrix.md`: minimum checks by change type.
- `asset-pipeline.md`: sprite and media contracts.
- `context-map.md`: where agents should look before acting.
- `templates/`: repeatable task, handoff, ADR, and asset specifications.

## Truth hierarchy

When sources disagree: explicit user direction wins, then accepted decisions, then `MOD_OVERVIEW.md`, then the active task, then established code/content patterns, then agent proposals. Flag conflicts instead of silently choosing.

## State hygiene

Keep these documents concise and current. Do not paste raw chat transcripts or huge logs. Link to artifacts, summarize evidence, and distinguish observed facts from proposals. Secrets, credentials, private keys, and machine-specific absolute paths do not belong here.
