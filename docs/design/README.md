# Design specs

The [GDD](../GDD.md) explains **what the game is and why**. These specs say **exactly what to build**: one file per system, with numbered rules, the data that tunes them, where the code lives, and how to know it's done.

Teammates and AI agents implement from the specs. If a spec and the GDD disagree, the spec wins for implementation and the disagreement gets fixed (see "Changing a spec").

## Index

| Spec | Prefix | Owner (role) | Code | Status |
| --- | --- | --- | --- | --- |
| [Combat](combat.md) | `CMB` | Combat engineer | `SLA.Combat` | Draft |
| [Skills & Shadow Extraction](skills.md) | `SKL` | Combat engineer | `SLA.Skills` | Draft |
| [Progression](progression.md) | `PRG` | Systems engineer | `SLA.Progression` | Draft |
| [Quests & exercise](quests-and-exercise.md) | `QST` | Systems engineer | `SLA.Quests`, `SLA.Exercise` | Draft |
| [Items & economy](items-and-economy.md) | `ITM` | Systems engineer | `SLA.Items` | Draft |
| [MR hub](mr-hub.md) | `MRH` | MR & UI engineer | `SLA.MR` | Draft |
| [System UI](system-ui.md) | `UI` | MR & UI engineer | `SLA.UI` | Draft |
| [Enemies & AI](enemies-and-ai.md) | `ENM` | AI & level designer | `SLA.AI` | Draft |
| [Gates](gates.md) | `GTE` | AI & level designer | `SLA.Gates`, scenes | Draft |
| [Narrative](narrative.md) | `NAR` | Whole team | Dialogue data | Draft |
| [Comfort & accessibility](comfort-and-accessibility.md) | `CMF` | Everyone (MR & UI leads) | All | Draft |

How things are built (process, not design) lives in [LEVEL_DESIGN](../LEVEL_DESIGN.md), [ART_PIPELINE](../ART_PIPELINE.md), [ARCHITECTURE](../ARCHITECTURE.md) and [TESTING](../TESTING.md).

## How a spec is written

Every spec uses the same sections, so people and agents always know where to look:

1. **Summary**: one paragraph.
2. **Rules**: numbered and testable (`CMB-1`, `CMB-2` ...). One behavior per rule, with real numbers.
3. **Data**: which ScriptableObject holds each tunable number.
4. **Code**: namespaces, classes, events.
5. **Acceptance criteria**: checkboxes, all verified **on a Quest**.
6. **Open questions**.

Reference rules by ID everywhere: in issues (`Implements CMB-4`), in commits, in test names (`CMB_4_PerTargetCooldown`), and in code comments where a rule explains a non-obvious line.

## Changing a spec

1. Open an issue labelled `type: task` describing the change and why.
2. Edit the spec **and** the matching GDD section in the same pull request (the live GDD first, then re-export [GDD.md](../GDD.md)).
3. Never renumber rules. Retire a rule by striking it through and noting the date; add new rules at the end.
4. Tuning numbers that live in ScriptableObjects can change without a spec edit; the spec lists the default.

New features start from the [feature spec template](../templates/FEATURE_SPEC.md).
