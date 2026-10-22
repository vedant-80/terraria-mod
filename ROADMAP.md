# NewMod Roadmap

## First iteration — local single-player conversations

Implement Guide, Merchant and Nurse personalities, local Ollama discovery/readiness/model selection, dedicated conversation UI, bounded Markdown memory and affinity, and Merchant buying adjustments. Preserve legacy weapon work in a recoverable archive.

Completion requires core tests, independent review, a real installed-version build and the manual checks in docs/VALIDATION.md. A missing game/toolchain is a blocker to release, not evidence of gameplay correctness.

## Stabilization

Measure starter-model roleplay, structured-output reliability and CPU latency while Terraria runs. Validate Windows setup, UI scales, failure/cancellation paths, save/reload, memory resets and vanilla pricing composition. Expand lore/context only from trusted sources after the slice works.

## Future multiplayer

Server owns stable authenticated player identities, relationships, memories, economy and one host-local model. Add validated request/response packets, fair bounded inference queues, per-player limits, reconnect/late join behavior and private conversation delivery. Review total text/affinity budgets for real lobby sizes. Test dedicated servers and prevent fabricated client affinity. No client LLM result becomes authoritative.

## Later content

Expand NPC personality coverage only after the initial three meet the same validation bar. Consider other local runtimes and non-Windows setup after first-release validation.
