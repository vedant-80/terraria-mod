# Issue tracker: GitHub

Issues and specs live in vedant-80/terraria-mod.
Use the gh CLI from this repository.

## Operations

- Create: gh issue create --title "..." --body-file <file>
- Read: gh issue view <number> --json number,title,body,labels,comments
- List: gh issue list --state open --json number,title,labels
- Comment: gh issue comment <number> --body-file <file>
- Label: gh issue edit <number> --add-label "<label>"
- Remove label: gh issue edit <number> --remove-label "<label>"
- Close: gh issue close <number>

Write multiline bodies to a temporary file and pass --body-file.
Outside this checkout, specify --repo vedant-80/terraria-mod.

“Publish to the issue tracker” means create a GitHub issue.
“Fetch the relevant ticket” means read the referenced GitHub issue.

## Pull requests as a triage surface

**PRs as a request surface: no.**

## Related work

Prefer native GitHub sub-issues and blocking dependencies when supported.
Otherwise, link children in the parent task list, put “Part of #N”
in each child, and record “Blocked by: #N” for dependencies.
A blocked ticket becomes available when all its blockers are closed.
