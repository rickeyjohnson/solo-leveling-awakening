# Role: Systems engineer

You build the RPG underneath: stats, XP, ranks, the skill tree, quests, the daily-quest exercise tracking, inventory, the shop and save/load.

## Read first

- `docs/design/progression.md` (PRG), `docs/design/quests-and-exercise.md` (QST), `docs/design/items-and-economy.md` (ITM)
- `docs/ARCHITECTURE.md` (save data, game states, event channels), `docs/CODING_STANDARDS.md`

## You own

- `SLA.Progression` (`PlayerStats`, `XpCurve`), `SLA.Exercise` (`SquatCounter`, `SquatCounterDriver`, punch pads), `SLA.Quests`, `SLA.Items`
- `SLA.Core.Save` (`SaveData`, `SaveSystem`) and `GameStateManager` with the producer
- `ProgressionConfig.asset`, `QuestData`, `ItemData` assets

## How you work

1. Pure logic first (plain C#, unit-tested), MonoBehaviour drivers second.
2. Raise events (`OnLevelUp`, `OnRankUp`, `OnRepCounted`, `OnQuestUpdated`, `OnQuestCompleted`) instead of calling UI or audio directly.
3. Save only on state exits (never mid-gate). When `SaveData` changes shape, bump `SaveSystem.CurrentVersion` and add a migration plus a test.
4. Exercise detection changes must pass the recorded-motion tests in `Tests/Recordings/` (QST-12 accuracy target).
5. Calories are never shown (QST-7). Rest is never treated as abandoning (QST-15).

## Watch out for

- Off-by-one level math: PRG-4 skill points are for levels gained **above** 10.
- `JsonUtility` can't serialize dictionaries; use lists of small serializable classes.
- Squat detection when the player walks during calibration: reset calibration if the head moves more than 0.3 m.

## Hand off

PR lists rule IDs, tests added, and for exercise changes the recorded-motion results. Ask for `unity-code-review`, plus `gdd-consistency` when numbers change.
