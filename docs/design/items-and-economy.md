# Items & economy (`ITM`)

| | |
| --- | --- |
| Owner | Systems engineer |
| GDD section | [Items, loot & economy](../GDD.md#items-loot--economy) |
| Code | `SLA.Items` (to create) |
| Data | `WeaponData`, `ItemData` assets |
| Status | Draft |

## Summary

Items are ranked E–S like gates and hunters. One currency, **Gold**, comes from kills and from selling monster cores. Gear stays small: weapons, gloves, a few consumables and dungeon keys. Using items is physical, not menu-driven.

## Rules

### Weapons

| Weapon | Type | Rank | Base dmg | STR req. | Special | Source |
| --- | --- | --- | --- | --- | --- | --- |
| Rusty Dagger | Dagger | E | 10 | 0 | — | Prologue |
| Steel Dagger | Dagger | E | 12 | 0 | — | Shop, 150 g |
| Hunter's Short Sword | Sword | D | 18 | 12 | Longer reach | Shop, 600 g |
| Kasaka's Venom Fang | Dagger | C | 22 | 15 | Poison 4 dmg/s for 4 s | Kasaka |
| Orc Greataxe | Two-handed axe | C | 35 | 30 | Knockback; two hands below STR 40 | Orc, 5% |
| Knight Killer | Dagger | B | 30 | 25 | +50% vs armored | Iron Commander |
| Baruka's Dagger | Dagger | A | 40 | 35 | Frost: slow 30% for 2 s | Ice Elf Warlord |
| Warchief's Cleaver | Two-handed axe | A | 50 | 45 | Breaks blocks and shields | High Orc Warchief |
| Demon Noble's Blade | Sword | S | 55 | 40 | Hits restore 2% HP | Demon Noble |

- **ITM-1** Holsters: two weapons on the hips, one two-handed weapon on the back, two potions on the chest.
- **ITM-2** Bosses always drop their named item the first time, then a random item of their rank on replays.

### Gloves

- **ITM-3** Gloves are the main armor slot because players see their hands constantly: Hunter's Wraps (E, +5 VIT, start), Lizardhide Gloves (D, +8 STR +5 VIT), Gauntlets of the Fallen (B, +15 VIT, blocks cost 50% less Fatigue, Job Change reward), Frostbound Grips (A, +15 AGI, cold resistance). Chest and boots are inventory-only with smaller bonuses.

### Consumables (physical use)

- **ITM-4** Health potion (50 g): raise the bottle to your mouth → +50 HP over 2 s. Mana potion (60 g): +30 MP. Fatigue tonic (80 g): Fatigue to 0.
- **ITM-5** Return stone (200 g): crush in your fist → leave a blue gate instantly. Does not work in red gates.
- **ITM-6** Instant dungeon key: hold it to the gate wall in the hub to replay a cleared gate at a chosen rank with one modifier (Elite, Swarm, Darkness, Time trial). E 100 g · D 300 g · C 800 g · B 2,000 g.
- **ITM-7** Skill book (1,500 g): read in the hub to learn a skill early. Random box (daily quest reward, not sold): open with both hands → gold, a potion, or a weapon of the player's rank.

### Drops and income

| Drop | E | D | C | B / Red |
| --- | --- | --- | --- | --- |
| Gold per regular kill | 5–10 | 15–25 | 40–60 | 100–150 |
| Monster core sell value | 20 g | 60 g | 150 g | 400 g |
| Potion drop chance | 8% | 10% | 12% | 15% |
| Equipment drop chance | 1% | 2% | 3% | 4% |
| Target gold per clear | ~150 | ~400 | ~1,000 | ~2,500 |

- **ITM-8** Economy goal: one clear pays for the next gate's potions plus savings; a new weapon takes about two clears. Tune drops if playtesters hoard or go broke.
- **ITM-9** Inventory: 20 slots, no weight limit. Gate loot goes straight to the System inventory.
- **ITM-10** A-rank and S-rank drop rates follow the B column ×1.5 and ×2 until tuned.

## Acceptance criteria

- [ ] Drinking a potion by raising it to the mouth works for short and tall players
- [ ] Every weapon above exists as a `WeaponData` asset with its stats
- [ ] Gold after each gate lands within ±25% of the target in a playtest

## Open questions

- Shop location: a hub window only, or also a merchant NPC at the Association?
