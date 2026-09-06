# Context Map

Use this search order to minimize guesswork and stale assumptions.

| Question | First source | Then inspect |
|---|---|---|
| What is the mod trying to be? | root `MOD_OVERVIEW.md` | `ROADMAP.md`, accepted decisions |
| What should happen now? | `current_task.md` | user request, handoffs |
| Why is the system shaped this way? | `decisions.md` | git history and nearby implementation |
| What conventions exist? | closest analogous code/content | project files, CI, root rules |
| How is a tModLoader C# API used? | local `~/tmod/ExampleMod/` source | [stable tModLoader API documentation](https://docs.tmodloader.net/docs/stable/), then official source |
| Which API version is valid? | installed project/package version | ExampleMod and stable docs, checking version-sensitive differences explicitly |
| Which .NET target should be used? | repository rules: .NET 8 | project files and installed `dotnet --info` |
| Is work complete? | acceptance criteria | validation matrix, review checklist, build log |

## Reconnaissance checklist

Before editing, locate the Git root, project files, entrypoint, content directories, localization, assets, configs, tests/scripts, CI, formatting rules, and the nearest analogous feature. For C# tModLoader work, search `~/tmod/ExampleMod/` for the closest working example before consulting external documentation. Check working-tree status and preserve unrelated user changes.

## Knowledge labels

Use `Observed` for facts verified in files or runtime, `Decided` for accepted choices, `Assumed` for reversible working hypotheses, and `Proposed` for ideas awaiting approval. Never present the latter two as canon.
