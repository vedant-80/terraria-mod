# Asset Spec: ImperialSword

- Task/owner: missing runtime texture repair / primary agent
- Asset: Content/Items/Weapons/ImperialSword.png
- Purpose: default inventory/world/held sword texture; fixes missing resource path.
- Reference: local ExampleMod/Content/Items/Weapons/ExampleSword.png and ExampleSword.cs (40×40).
- Source: designs/concepts/imperial-sword-concept.png, creator-provided; external provenance/license unverified.
- Canvas/frame: 40×40 RGBA, one frame, no sheet animation or glow layer.
- Grid: 1 export pixel per logical pixel, nearest-neighbor fit of trimmed content to 39×40 with transparent padding. No smoothing.
- Origin: default vanilla swing handling, tip upper right, grip lower left; existing Item.width/height remain 40×40.
- Palette: inherited from supplied silver/gold/red concept; no palette remapping or alpha cleanup performed.
- Generation: shared resize_asset.py helper, ExampleSword.png as dimension reference, --pixel-size 1.
- Review: native image inspected; silhouette and colors preserved at reduced size. Export is integrated for resource correctness, not claimed as final-quality art. Client inventory/world/swing and bright/dark background review still required.
- Original sword SHA256: ba79542e8af823b6c25be683edceb41a03a42d1750168927459b122fcf76c500.
