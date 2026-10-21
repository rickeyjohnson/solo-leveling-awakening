# Workflow: Sync the GDD export

The live GDD is a shared team doc; `docs/GDD.md` is a Markdown export of it. Re-export when the live doc changes in a way that affects building.

1. Export the live GDD to Markdown (the doc's export menu, or ask Claude with the docs connector to export it).
2. Replace `docs/GDD.md`, keeping the export note at the top (`This is a Markdown export of the live GDD ... Last export: <date>`).
3. Diagrams export as placeholders: replace each with its image from `docs/images/` (re-export changed diagrams as PNG).
4. GitHub math: code fences labelled `latex` must become `math`.
5. Run `ai/reviews/gdd-consistency.md` against the new export and the specs; fix specs or open issues for every mismatch.
6. Commit on a `docs/gdd-sync-<date>` branch and open a PR listing what changed in the GDD.
