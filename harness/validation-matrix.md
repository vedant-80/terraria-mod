# Validation Matrix

Run every check whose trigger applies. If tooling or the game is unavailable, record `blocked/not run` and give an exact human procedure; that is not a pass.

| Change type | Static/build checks | Runtime checks | Extra evidence |
|---|---|---|---|
| Any C# | format/analyzer if configured; compile mod | load mod and exercise changed path | command and relevant log excerpt |
| Item/weapon/accessory | compile; localization/assets exist | obtain, use, reforge where relevant; inventory/world behavior | stats and recipe observed in game |
| Projectile/NPC | compile; frame bounds; network fields reviewed | spawn/despawn, collision, AI transitions, difficulty; host/client | capture or steps for edge states |
| Player/system/global hook | compile; hook lifecycle and allocations reviewed | enter/leave world, death/respawn, reload; multiplayer | state-transition observations |
| World generation/persistence | compile; serialization compatibility review | new world, save/reload, old/missing data, multiplayer join | seeds/files tested and migration notes |
| UI/draw/shader | compile; dedicated-server guards | resolutions, UI scales, lighting/backgrounds, gamepad if relevant | screenshots at representative settings |
| Recipe/loot/progression | registration and condition review | progression-valid acquisition; Journey/Expert/Master if relevant | comparison table or observed drops |
| Localization/text | key/reference scan; fallback review | render in context for wrapping and accuracy | screenshot or localization audit |
| Sprite/animation | dimensions/alpha/frame script or inspection | inventory/world/animation at speed | sprite sheet plus contact sheet/capture |
| Audio/music | import/path review; license provenance | triggering, overlap, distance, looping, volume | provenance and listening notes |
| Config/keybind | defaults and localization review | change, persist, reload, conflict/unbound behavior | settings tested |
| Documentation only | link/path and internal consistency review | follow instructions once when practical | reviewer sign-off |

## Recommended command discovery

The mandatory NewMod compile entry point is `bash harness/helpers/build.sh`. The orchestrator calls the verifier (`agents/QA_VALIDATOR.md`) before code changes for a baseline and again after changes; the verifier always uses this helper. Its nonzero result blocks a build pass. See `helpers/README.md` for prerequisites and sprite resizing checks.

Do not assume a command exists. Inspect the repository for `.csproj`, solution files, scripts, CI, and tModLoader setup. The project must target .NET 8; verify the target framework and installed SDK rather than accepting an implicit retarget. Prefer repository-provided commands. A typical local compile may use `dotnet build <project>.csproj`, but record the command actually available.

For API-sensitive changes, record the analogous file or pattern inspected in `~/tmod/ExampleMod/`. If that reference was insufficient, record the relevant page from the [stable tModLoader API documentation](https://docs.tmodloader.net/docs/stable/) and any version caveat.

## Multiplayer minimum

For network-relevant changes, verify single-player, server authority, one host plus one client, reconnect/late join, and dedicated-server loading where feasible. Cosmetic-only behavior may remain client-side; gameplay truth must not depend on a client’s unsynchronized state.
