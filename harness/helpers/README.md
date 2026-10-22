# Shared helpers

For concept images with near-invisible fringe pixels, use `--alpha-cutoff 16` to ignore alpha ≤16 when choosing crop bounds. Default is 0. This only changes crop bounds and does not change the retained pixels' alpha.

The sprite preflight also checks all C# / PNG pairs under `Content/Projectiles/`, including ImperialSwordThrust and ImperialShard. Keep non-content helper classes outside these directories.

Build now requires Python 3 for `check_weapon_assets.py`, run before dotnet. Every C# file under `Content/Items/Weapons/` requires a case-exact sibling PNG with a nonzero PNG header. This is not full PNG decoding or C# parsing: review namespace/class alignment and client reload separately. Keep helper classes outside the weapon directory. Primary weapon sprites follow ExampleMod's sibling layout. Reserve `assets/` for effects, music, sounds and shared textures; concepts belong in `designs/`.

Run `bash harness/helpers/build.sh` before adding or editing code and after implementation. It selects SDK 8 through root `global.json`, calls `dotnet build` on NewMod.csproj, and propagates failure. Run from any directory using the script's absolute path. It does not install tModLoader: the project still requires `../tModLoader.targets` and its referenced assemblies. Save the command, exit status, and output in the verifier handoff/build log. A failed baseline permits scoped repairs or helper work recorded by the orchestrator, but never counts as a passing build.

For resizing, create a virtual environment and install the helper dependency:

```sh
python3 -m venv /tmp/newmod-sprite-tools
/tmp/newmod-sprite-tools/bin/pip install -r harness/helpers/requirements.txt
/tmp/newmod-sprite-tools/bin/python harness/helpers/resize_asset.py designs/concepts/imperial-sword-concept.png /tmp/imperial-sword-draft.png --reference /home/vdnt/tmod/ExampleMod/Content/Items/Weapons/ExampleSword.png --pixel-size 2
```

The importable `resize_asset` function and CLI preserve source files, reject existing outputs, trim transparent margins, fit proportions to within logical-pixel rounding, center with transparent padding, and export RGBA PNG with nearest-neighbor sampling. Output directories must already exist. `--pixel-size 2` gives consistent 2×2 clusters on a 20×20 logical grid for a 40×40 export; use 1 for individually authored export pixels. Both canvas dimensions must divide evenly by the selected pixel size. This controls square pixel clusters, not Terraria world scale or item hitboxes.

Local ExampleMod measurements (2026-09-05), under `Content/Items/Weapons/`:

| Reference | PNG canvas |
|---|---|
| ExampleSword.png | 40×40 |
| ExampleShortsword.png | 32×32 |
| ExampleCustomSwingSword.png | 46×48 |
| ExampleShootingSword.png | 26×42 |

Use 40×40 as the Imperial Sword starting canvas, matching ExampleSword's PNG and item dimensions. These are examples, not universal bounds. Select a comparable single frame for other asset types and document its dimensions and consumer. Never resize a packed sprite sheet as one sprite: split and process individual frames with identical geometry and anchors, then assemble under the asset spec. The helper cannot detect a static packed sheet.

Downsampling concept art loses detail; it produces a draft requiring silhouette, palette, alpha-edge, origin and in-game review. Resizing does not establish final art quality. Inspect at 1× and 4× nearest-neighbor against bright/dark backgrounds. Reference images supply dimensions only and are not copied into the output.
