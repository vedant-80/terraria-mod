# Local NPC conversation setup (Windows first)

This preview supports **single-player** Guide, Merchant and Nurse conversations. Internet is needed to download Ollama/model files; inference then runs on your computer. There is no API key, hosted proxy or paid inference fallback. Electricity, RAM and CPU/GPU use remain your responsibility.

## Install and start Ollama

1. Install Ollama from https://ollama.com/download/windows.
2. Set the **user environment variable** `OLLAMA_NO_CLOUD` to `1` using Windows “Edit environment variables for your account”. Quit Ollama completely from the system tray and restart it so the daemon inherits the setting. This disables Ollama cloud models and web search. See https://docs.ollama.com/faq#how-do-i-disable-ollamas-cloud-features.
3. In a new PowerShell window, run `ollama pull qwen2.5:1.5b`. The current quantized model download is approximately 986 MB; allow additional disk space for Ollama, temporary downloads and any other models. Source: https://ollama.com/library/qwen2.5:1.5b.
4. Keep Ollama running. The mod uses only `http://127.0.0.1:11434`, bypasses HTTP proxies and refuses redirects. Do not expose this unauthenticated endpoint to your network.
5. Launch the mod in a single-player world and type `/npcchat`. Refresh installed models, choose `qwen2.5:1.5b` (or another installed local chat model) and run the readiness test.

No automatic model download or runtime installation is performed. Models must support chat completion and Ollama structured outputs; an installed model is not necessarily compatible. Local cloud aliases/remote metadata are rejected, including `/api/show` checks before inference. Disabling cloud in the daemon is required: the mod cannot change another process's settings or protect against a deliberately modified local daemon.

## Hardware and model choice

A 986 MB download is not a RAM requirement. Runtime overhead, context, model loading, Terraria and other apps consume memory. A dedicated GPU is not required; CPU inference may be slow. Start with the small recommended model, leave several GB of RAM headroom and test while Terraria is running. The Windows display reports available/total physical RAM as an advisory snapshot, warning below 2 GiB available without blocking selection. Other platforms report unknown hardware readings.

The readiness test reports one successful structured response and measured latency, not guaranteed speed or roleplay quality. Model cold starts can be slower than subsequent calls. A request times out after 45 seconds. Close other apps or choose a smaller model if needed. Models can remain loaded for five minutes after use; manage them through Ollama if you need the memory back. Switching models retains conversation memory and affinity.

## Talk and relationships

Speak to the Guide, Merchant or Nurse and select the separate **Talk** button. Normal chat, Shop, Heal and happiness interactions remain available. Expand the suggested-question dropdown or type up to 300 characters; select Send. Replies aim for 1–3 sentences and are capped at 600 characters. Cancel, close chat or switch NPCs to discard a pending reply. An already displayed/accepted reply can continue saving after you close the panel.

Affinity starts at zero, ranges from -100 to +100 and changes at most two points per eligible exchange, with five points of cumulative movement per Terraria day. Repeated normalized questions earn no further movement that day. At most 64 distinct questions are tracked per day; after that affinity stops changing until the next dawn. Death does not reset relationships. NPC replacements share their type's memory in that world; different characters/worlds have separate records.

Merchant purchase prices adjust within +/-10% when opening a fresh shop; tiny copper values may stay unchanged to respect the cap. Existing open shop prices are not continuously changed. Sell prices and Nurse fees are unchanged. Vanilla happiness/accessory composition and buyback behavior still require in-game verification before release. Merchant affinity loads asynchronously; if a shop opens before its saved relationship is ready, reopen it after conversation readiness.

## Local files and resets

Use settings to open the personality or memory folder. Files live under your tModLoader save directory:

- `NewMod/Conversations/Personalities/{Guide,Merchant,Nurse}.md`: editable personalities, copied from bundled defaults only if absent. Changes are read on the next question. Keep each short; only the first 1,600 UTF-8 bytes enter the bounded prompt, and files over 16 KiB are refused.
- `NewMod/Conversations/Memory/<world>_<player>_<NPC>.md`: versioned Markdown with a JSON block preserving exact strings. Summary <=1,200 characters; at most four recent question/reply pairs. A stricter total prompt budget can omit older exchanges and shorten summary text.
- `NewMod/Conversations/settings.json`: selected model, containing no credentials.

Close the game before manually editing memory files. Prefer the confirmed per-NPC Reset in settings; it resets both memory and affinity for the current character/world/NPC. Corrupt, oversized and future-format files are preserved and rejected instead of silently overwritten; back them up before resetting. Personalized memories are untrusted conversation data, never executable instructions.

Default storage allows 128 relationships with conversation text and 512 relationship records total. Least recently touched text is evicted first; at the relationship limit the oldest recognized record is removed, including its affinity. Unknown files count toward capacity and are preserved. File size limits bound each record. Atomic file replacements protect individual writes, but disk failure/crash during a multi-file eviction can partially complete the update. Save failures are shown explicitly; an accepted in-memory conversation may not survive restart if its write fails. Back up the save folder when needed.

## Security and future multiplayer

The model cannot execute code, use tools, request arbitrary file access or directly set game state. Replies and metadata are validated; deterministic rules enforce affinity and price bounds. Prompt injection and spoiler/character breaks cannot be completely prevented with free-text models. Personality editing and local save tampering remain under the single-player user's control.

Conversation features and price adjustments are disabled in multiplayer. Future multiplayer will move inference and authoritative relationships to the host, isolate each player's context, and add authenticated identity, request scheduling and synchronization. Clients will not be trusted to report affinity.

## Building this checkout

Use .NET SDK 8 and `bash harness/helpers/build.sh`. For a normal development machine, install tModLoader through Steam and use the generated ModSources/tModLoader.targets with its actual assembly paths. The current validation workspace instead imports a local copy of installed tMLMod.targets and libraries from ../.local-tmodloader; nothing is executed inside or written to the Steam installation. Targets identify TML_2026_08 and dependency metadata identifies Terraria 1.4.4.9. Do not replace missing references with fake stubs or retarget to another .NET version. In a read-only home environment, use a writable `DOTNET_CLI_HOME`, for example `/tmp/tmod-dotnet-home` on Linux.

The independent core suite needs no game installation: see [tests/README.md](../tests/README.md). Compilation-only validation now passes against the installed libraries with zero warnings/errors. In this sandbox, the normal helper compiles but the subsequent engine packaging process fails when creating a socket. To reproduce compilation alone, use `BuildMod=false` in the Linux environment when running the helper (or `dotnet build NewMod.csproj -p:BuildMod=false`); this does not produce a .tmod or validate loading. Full packaging and manual gameplay remain release prerequisites.
