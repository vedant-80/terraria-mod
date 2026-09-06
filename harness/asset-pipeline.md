# Asset Pipeline

Every asset starts with a written specification and ends with an in-context check. The sprite creator proposes pixels; code and art reviewers confirm integration.

## Sprite workflow

Mechanical resizing uses `helpers/resize_asset.py` as documented in `helpers/README.md`. Derive canvas dimensions from a comparable local ExampleMod single-frame PNG, preserve aspect ratio with transparent padding, and choose a square logical pixel grid that divides the canvas. Record these choices in the asset spec. A resized concept remains a draft until the checks below pass.

1. Inspect nearby vanilla and mod assets for scale, outline weight, palette, perspective, and file conventions.
2. Fill `templates/asset-spec.md`, including exact canvas and frame geometry.
3. Create editable source when the tool supports it, then export a lossless PNG with alpha. Never upscale with smoothing.
4. Inspect silhouette and contrast at native scale and 4× nearest-neighbor scale.
5. Check alpha edges, frame cells, consistent origins, palette count, and transparent padding.
6. Integrate using exact case-sensitive paths and correct animation/frame registration.
7. Test against bright/dark backgrounds, inventory UI, world lighting, and active motion as applicable.

Generated imagery is a draft, not a shippable sprite by default. It must be redrawn/cleaned to deterministic pixel placement and the project’s palette. Never imitate a living artist or import copyrighted assets without explicit permission and provenance.

## Naming and sidecars

Weapon layout follows ExampleMod: `Content/Items/Weapons/<Name>.cs` plus a case-exact sibling `<Name>.png`; class/namespace follow that directory. Keep primary sprites with content, with no `Texture` relocation override. Reserve `assets/` for effects, music, sounds and shared textures. Put concepts, provenance and specifications under `designs/`. Run `python3 harness/helpers/check_weapon_assets.py` before integration (also run by the build helper).

Match the codebase’s established naming before adding a convention. Keep each source/export/license note discoverable. Until the repository defines an asset metadata format, store the approved spec in the task handoff and record provenance in `decisions.md` when it affects redistribution.

## Animation contract

Define sheet width/height, frame width/height, layout direction, ordered frame meanings, ticks per frame, looping behavior, origin/anchor, hitbox relationship, and whether glowmask or emissive layers share the same layout. Validate bounds mechanically where possible and motion visually.

## Audio contract

Record creator/source, license, edit history, duration, sample format, intended trigger, concurrency limit, loop points, spatial behavior, and volume target. Confirm that redistribution is allowed before committing source material.
