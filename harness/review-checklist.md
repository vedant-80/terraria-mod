# Review Checklist

Use the applicable sections. Mark `N/A` with a reason; unchecked items block completion unless the orchestrator documents an exception.

## Task contract

- [ ] Diff matches `current_task.md`; unrelated changes are absent.
- [ ] Acceptance criteria are testable and satisfied.
- [ ] Assumptions and new decisions are recorded.
- [ ] User-authored work was preserved.

## Code and architecture

- [ ] Code follows current tModLoader APIs and established repository patterns.
- [ ] C# API usage was checked against `~/tmod/ExampleMod/` first; stable official documentation was used for unresolved or version-sensitive questions.
- [ ] Project targets and build tooling remain on .NET 8.
- [ ] Content has stable internal names; renames and save/network compatibility were considered.
- [ ] Client-only APIs are guarded from dedicated-server execution.
- [ ] Multiplayer state has clear authority, synchronization, and late-join behavior.
- [ ] Per-tick and draw paths avoid needless allocation, scans, reflection, I/O, and logging.
- [ ] Hooks, stat modifiers, recipes, localization, and content registration compose correctly.
- [ ] Debug code, magic numbers, dead code, and avoidable duplication are absent.
- [ ] Public or subtle behavior is documented close to the code.

## Gameplay and balance

- [ ] Feature has a clear player-facing purpose and acquisition/progression context.
- [ ] Numbers are justified against comparable content and intended progression tier.
- [ ] Edge cases were checked: death, world reload, rejoin, stacking, reforges, prefixes, difficulty, and sequence breaks.
- [ ] Accessibility and readability are acceptable at gameplay speed.

## Visual assets

- [ ] Sprite dimensions, frame count/layout, origin, animation timing, palette, and glow behavior match the asset specification.
- [ ] Transparent pixels, seams, stray colors, silhouette, and one-pixel artifacts were inspected at 1× and enlarged nearest-neighbor scale.
- [ ] Inventory/world/projectile frames align without jitter or clipping.
- [ ] Filenames, casing, and locations match consuming code exactly.

## Text, audio, and localization

- [ ] Player-facing text uses localization keys; tone and terminology are consistent.
- [ ] Tooltip claims match actual behavior and expose essential conditions.
- [ ] Audio usage, looping, volume, pitch variation, and ownership/licensing are documented.

## Validation

- [ ] Build completed with no new errors and warnings were reviewed.
- [ ] Relevant automated/static checks passed.
- [ ] Focused in-game test passed; multiplayer/server test was run when applicable.
- [ ] Evidence was appended to `build_log.md`.
- [ ] Remaining risks and untested behavior are explicitly handed off.
