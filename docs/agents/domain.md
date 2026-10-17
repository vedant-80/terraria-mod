# Domain docs

This repository has one domain context.

## Before exploring

Read GLOSSARY.md for domain vocabulary.
Read relevant decisions in harness/decisions.md.
Check docs/adr/ for decision indexes or additional referenced records.

If optional domain documents are absent, proceed silently.
Create terminology and decision records lazily through domain-modeling.

## Vocabulary

Use glossary terms in issues, designs, tests and explanations.
When a concept is missing or ambiguous, resolve its meaning before
adding it to the glossary.

## Decisions

harness/decisions.md remains the canonical architectural decision log.
Preserve existing entries and identifiers.
New architecture, persistence and networking decisions follow the
acceptance requirements in AGENTS.md.

Surface conflicts with accepted decisions explicitly.
Record accepted replacements as superseding decisions rather than
rewriting history.
