# Asset Spec: ImperialShard and ImperialSwordThrust

- Owner/task: gameplay implementer, dagger thrust and short after-effect.
- Outputs: Content/Projectiles/ImperialShard.png (32×32 RGBA); ImperialSwordThrust.png (40×40 RGBA). Single-frame PNGs beside same-name classes, no Texture overrides.
- Source: creator-provided designs/concepts/imperial-shard-concept.png; thrust reuses existing ImperialSword.png byte-for-byte. External license/provenance unverified; no ExampleMod pixels copied.
- Reference: ExampleShortswordProjectile.png 32×32, ExampleSword.png 40×40. Shard resized with shared helper, --pixel-size 1 --alpha-cutoff 16, nearest neighbor, transparent fit; source preserved. Crop ignores near-invisible alpha fringes; visible source bounds (699,327)-(1215,497) fit to 32×11 pixels inside canvas.
- Frame/anchor: shard points right at zero rotation, center aligned with 12×12 hitbox; thrust diagonal tip upper right, 18×18 collision base plus forward blade line, centered 40×40 sprite. No sprite sheet or glow layer.
- Timing/palette: inherited silver shard palette, continuous opacity fade across 24 ticks. Thrust extends/retracts over 16 ticks; visible on player's held-projectile layer.
- Validation: PNG full decode and native inspection; inventory/world motion, bright/dark and host/client checks require game access. Art remains a test export pending those checks.
