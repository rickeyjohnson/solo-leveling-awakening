# Quests & exercise (`QST`)

| | |
| --- | --- |
| Owner | Systems engineer |
| GDD section | [Quest system](../GDD.md#quest-system) |
| Code | `SLA.Quests` (to create), `SLA.Exercise` (`SquatCounter`, `SquatCounterDriver`) |
| Data | `QuestData` assets |
| Status | Draft |

## Summary

The System gives four kinds of quests. The signature one is the **daily quest**: real squats and punches at the start of every play session, counted by the headset and controllers and paid out in stat points. Abandoning it sends the player to the Penalty Zone, as in canon.

## Rules

### Quest types

- **QST-1** Daily quest: first hub visit each session. Reward: **+3 stat points**, full recovery, one random box.
- **QST-2** Story quests drive the main path (gates, re-assessments). Reward: XP, rank-up, new gate.
- **QST-3** Emergency quests appear mid-gate (e.g. *[Defeat all enemies within 90 seconds.]*). Bonus XP and loot; failure has no penalty.
- **QST-4** Hidden quests are never announced (spare a fleeing goblin, find a secret room). Reward: a title or rare item.

### Daily quest

- **QST-5** Reps by player level: 1–9: 20 squats / 100 punches (~3 min); 10–19: 30 / 150; 20–29: 40 / 200; 30–40+: 50 / 250 (~6 min); optional "Canon mode": 100 / 400.
- **QST-6** Rounds alternate squats and punches so legs and arms rest in turn. Finishing faster than the target time gives a small XP bonus.
- **QST-7** The game shows reps and time, never calories.

### Squat detection

- **QST-8** Calibrate standing head height H₀ over **2 s** while the System says *[Stand still.]*.
- **QST-9** A rep starts when the head drops below **H₀ × 0.80** (adjustable 0.70–0.85) and counts when it rises above **H₀ × 0.95**.
- **QST-10** Reject the rep if head pitch exceeds **45°** down or the head moves more than **0.3 m** horizontally from the start spot.
- **QST-11** A rep must take at least **0.8 s** from leaving the standing zone to returning.
- **QST-12** Accuracy target: 95% of reps counted, at most 1 false count per 50 (checked against video).

### Punch detection

- **QST-13** Punches land on glowing System target pads in jab, cross, hook and uppercut positions. A hit counts at **2.5 m/s or more**, moving away from the headset.

### Penalty Zone

- **QST-14** Choosing **Abandon** triggers *[You have failed to complete the Daily Quest. You will now be moved to the Penalty Zone.]*, then a desert where the player survives giant centipedes for **3 minutes**. Surviving gives a little XP; dying returns to the hub with no further loss.
- **QST-15** Pausing or resting is never treated as abandoning.

### Safety

- **QST-16** A play-space check (room scan) runs before every daily quest and warns about furniture within reach.
- **QST-17** A **Rest** button is always available with no penalty; hydration and break reminders appear between rounds.
- **QST-18** A low-impact option swaps squats for arm-only rounds.

## Data

`QuestData`: `id`, `title` (System voice), `kind`, `objective`, `rewardXp`, `rewardStatPoints`, `rewardGold`, `nextQuestId`. Daily rep counts per level band live in a `DailyQuestConfig` (to create).

## Code

- `SquatCounter` (pure) implements QST-9 to QST-11; tested in `SquatCounterTests`.
- `SquatCounterDriver` reads the headset, implements QST-8 calibration, raises `OnRepCounted`.
- `PunchPad` (to create): trigger collider + speed check (QST-13).
- `QuestTracker` (to create): active quests, progress, rewards; raises `OnQuestUpdated`, `OnQuestCompleted`.

## Acceptance criteria

- [ ] Squat accuracy QST-12 verified with 5 people on video
- [ ] The daily quest takes about 3 minutes at levels 1–9
- [ ] Abandon → Penalty Zone → back to the hub works end to end
- [ ] Recorded-motion tests cover squats, bows, bobs and fast reps

## Open questions

- Random box contents table (see [items](items-and-economy.md)).
