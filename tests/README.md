# Standalone conversation checks

From `NewMod`, run:

```sh
DOTNET_CLI_HOME=/tmp/newmod-dotnet-home dotnet run --project tests/ConversationCore/ConversationCore.csproj --nologo
```

The repository's `global.json` selects .NET 8. No NuGet packages, Terraria installation, network access, Ollama daemon or model are required. The project links the production core sources. HTTP tests use an in-memory handler and temporary memory directories are removed after each run. `tests/NuGet.Config` clears package feeds for reproducible offline restores.

These checks cover bounded Markdown persistence, invalid/future file preservation, player/world isolation, text-first eviction and relationship capacity, cancellation, daily affinity and duplicate caps, independent response metadata validation, Unicode prompt budgets, local model filtering and metadata checks, redirects and response size limits. They do not establish live model quality, prompt-injection immunity, or Terraria UI/gameplay compatibility.
