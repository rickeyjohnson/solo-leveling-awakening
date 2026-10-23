# Review: GDD consistency

Check that the design docs agree with each other and with the code, so nobody builds from a stale number.

## Inputs

The changed files, plus `docs/GDD.md`, `docs/design/*.md`, `docs/ROADMAP.md` and any ScriptableObject defaults in code (`CombatConfig.cs`, `ProgressionConfig.cs`, `*Data.cs`).

## Check

- [ ] Every number changed in a spec matches the GDD section it cites, or the PR updates both (live GDD first, then re-export `docs/GDD.md`).
- [ ] Defaults in ScriptableObject classes match the spec's **Data** table.
- [ ] Rule IDs are never renumbered; retired rules are struck through with a date; new rules are appended.
- [ ] Cross-references resolve (e.g. `UI-8` points at the right `CMF` rule; links and anchors work).
- [ ] Names are consistent everywhere: gates, enemies, bosses, weapons, skills, ranks (E, D, C, B, A, S), and the Job (Shadow Assassin).
- [ ] Scope labels (Must/Should/Could) match `docs/ROADMAP.md`.
- [ ] Anything new is reflected in the right spec's **Open questions** if undecided.
- [ ] No canon IP misuse introduced (see the Legal & IP rules in the GDD).

## Report

A table of mismatches:

| Where | Says | Should say (source) | Fix |
| --- | --- | --- | --- |

If everything agrees, say so in one line.
