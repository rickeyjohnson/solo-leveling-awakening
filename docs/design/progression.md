# Progression (`PRG`)

| | |
| --- | --- |
| Owner | Systems engineer |
| GDD section | [Player character & progression](../GDD.md#player-character--progression) |
| Code | `SLA.Progression` (`PlayerStats`, `XpCurve`) |
| Data | `ProgressionConfig.asset` |
| Status | Draft |

## Summary

The player starts at Level 1, E-rank, every stat at 10, and climbs to Level 50 and S-rank on the main path. Each stat changes something the player can **feel**, never their real movement speed. Ranks go up only at Hunter Association re-assessments, which story missions unlock.

## Rules

### Levels and XP

- **PRG-1** XP to next level = `round(50 × L^1.6)`. Level cap **50**; XP stops at the cap.
- **PRG-2** Enemy XP = base XP × gate-rank multiplier: E ×1, D ×3, C ×8, B ×20, A ×45, S ×100.
- **PRG-3** Each level grants **5 stat points** and fully restores HP, MP and Fatigue (canon-style comeback heal, even mid-fight).
- **PRG-4** 1 skill point per level gained above Level 10 ([SKL-17](skills.md)).

### Stats

All stats start at 10. "Per point" means per point above 10.

- **PRG-5 Strength:** +2% melee damage (inside CMB-4) and unlocks heavier weapons at their `strengthRequirement`.
- **PRG-6 Vitality:** +10 max HP (base 100) and +0.5% damage reduction.
- **PRG-7 Agility:** −1% Dash cooldown (floor 40%) and +1% stamina regen.
- **PRG-8 Perception:** +1 m enemy-highlight range and +4 ms parry window; weak points glow.
- **PRG-9 Intelligence:** +2 max MP (base 20) and +2% skill power.
- **PRG-10** No stat changes the player's real movement speed (comfort; see CMF-2).
- **PRG-11** Points are spent in the Status window in the hub, or from the pause window inside a gate.

### Ranks

- **PRG-12** Main-path ranks by level: E 1–9, D 10–19, C 20–29, B 30–39, A 40–49, S 50.
- **PRG-13** Reaching a rank's level threshold unlocks that rank's story quest; clearing its gate unlocks a re-assessment at the Association, where rank goes up. Levels alone never change rank.
- **PRG-14** The S-rank re-assessment breaks the measuring device (canon homage) and ends the story.

### Job Change and titles

- **PRG-15** At Level 20 the System issues the Job Change quest (Castle of the Fallen Knight). Clearing it makes the player a **Shadow Assassin** (dual-wield, Stealth, Shadow Step). If Arise ships, the Job is **Shadow Summoner**.
- **PRG-16** One title can be equipped at a time: Survivor of the Double Dungeon (+5% max HP, finish tutorial), One Who Does Not Skip Days (+10% stamina regen, 5 daily quests in a row), Pest Control (+10% damage to insects, 200 insect kills), Unbroken (+5% parry window, clear a gate without damage), Kingslayer (+5% all damage, final boss).

### Pacing targets

| Rank | Levels | Main-path time | Gate |
| --- | --- | --- | --- |
| E | 1–9 | 0–15 min | Prologue, Goblin Warren |
| D | 10–19 | 15–35 min | Kasaka's Nest |
| Job Change | 20 | 35–50 min | Castle of the Fallen Knight |
| C | 20–29 | 50–70 min | Hive Below Jamsil |
| B | 30–39 | 70–90 min | Frost Gate |
| A | 40–49 | 90–110 min | Seoul Subway |
| S | 50 | 110–130 min | Spire Gate |

The effort per level (XP ÷ rank multiplier) rises within a rank and drops at each rank-up by design ([XP chart](../images/xp-effort-curve.png)).

## Data

`ProgressionConfig`: `xpBase` 50, `xpExponent` 1.6, `levelCap` 50, `rankXpMultipliers` {1, 3, 8, 20, 45, 100}, `statPointsPerLevel` 5, `skillPointsAfterLevel` 10, `baseStat` 10, `baseHp` 100, `hpPerVitality` 10, `baseMp` 20, `mpPerIntelligence` 2.

## Code

- `XpCurve` (pure): `XpToNext`, `EnemyXp`, `EffortToNext`, `RankForLevel`.
- `PlayerStats` (pure, serializable): level, XP, points, five stats; `AddXp`, `TryAllocate`, `MaxHp`, `MaxMp`.
- Raises `OnLevelUp` (new level) and `OnRankUp` (new rank index).

## Acceptance criteria

- [ ] Unit tests cover PRG-1 to PRG-4 (exists: `ProgressionTests`)
- [ ] Level-up fully heals, shows the gold toast, and plays the level-up sound
- [ ] A playtester reaches D-rank in 15–35 minutes on the main path
- [ ] Each stat's effect is noticeable to a playtester without reading numbers

## Open questions

- Should Agility also speed up weapon recovery animations? (Must not affect real movement.)
