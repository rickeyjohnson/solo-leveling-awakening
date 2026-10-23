# Gates (`GTE`)

| | |
| --- | --- |
| Owner | AI & level designer |
| GDD section | [Gates & level design](../GDD.md#gates--level-design) |
| Process | [LEVEL_DESIGN](../LEVEL_DESIGN.md) (template, greyboxing, VR rules) |
| Code | `SLA.Gates` (`GatePortal`, `PlayerSpawn`), `GateData` assets, `Gate_*` scenes |
| Status | Draft |

## Summary

Eight gates on the main path, E to S, all built from one five-part template. This spec lists what each gate must contain; LEVEL_DESIGN says how to build it.

![Gate template](../images/gate-template.png)

## Rules (all gates)

- **GTE-1** Every gate follows the template: entrance hall (safe, respawn) → 2–4 combat rooms → optional secret room → antechamber → boss room.
- **GTE-2** E and D gates have 2 combat rooms; C and up have 3–4 plus puzzles and traps. If late, B–S gates ship with 2 rooms (never cut).
- **GTE-3** Combat rooms about 8 × 8 m, corridors at least 2.5 m wide, boss rooms about 12 × 12 m, ceilings at least 3 m.
- **GTE-4** Death respawns the player at the entrance with XP kept. Reaching the antechamber unlocks a shortcut from the entrance to it.
- **GTE-5** Defeating the boss shows *[The dungeon's boss has been defeated. The gate will close shortly.]* and opens the exit portal.
- **GTE-6** Blue gates can be left with a Return stone; Red gates cannot (*[Warning: This gate cannot be exited until the boss is defeated.]*).
- **GTE-7** Every puzzle uses an action already learned in combat, gives a System hint after 60 s, and takes a first-timer under 2 minutes. Every trap telegraphs for at least 1 s. No traps in boss rooms; no falls or pits.
- **GTE-8** Each gate is one scene with its own lightmaps, NavMesh and `EnemyDirector`, and a `PlayerSpawn` at the entrance.

## Per-gate requirements

| # | Gate | Rank | Rooms | Enemies | Boss | Must include |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | Prologue: Temple of Commandments | Posted D | Tutorial path | Temple statues | Giant Statue | Three carved rules to decipher; Kang's death; ends with *[You have qualified to become a Player.]* and waking up in the real room |
| 2 | Goblin Warren | E | 2 | Goblins, kobolds, archers | Goblin Chieftain | Teaches melee, block, Dash; low-light cave |
| 3 | Kasaka's Nest | D | 2 | Lizardmen, spitters | Kasaka | Teaches projectile dodging and parry; drops Kasaka's Venom Fang |
| 4 | Castle of the Fallen Knight | Job Change | Waves | Undead soldiers, knights | Iron Commander | Sealed trial; parry duel; *[Job Change complete. You are now a Shadow Assassin.]* |
| 5 | Hive Below Jamsil | C | 3–4 | Worker ants, wall-crawlers, flyers, soldiers | Ant Queen | First puzzles and traps: pressure plates, resin bridges, gas vents, nests |
| 6 | Frost Gate (Red) | Red (posted B) | 3–4 | Ice elves, frost wolves | Ice Elf Warlord | Cold mechanic (Fatigue builds away from braziers); brazier relay, ice mirrors, icicles, pendulums, arrow slits; Cha Hae-In's guild raid |
| 7 | Dungeon Break: Seoul Subway | A | 3–4 | Orcs, shamans, lesser demons | High Orc Warchief | White Tiger Guild holding the stairs; enclosed station |
| 8 | The Spire Gate | S | Floors | Demons, demon knights, elite remixes | Demon Noble | Chains templates as floors; ends with the S-rank re-assessment |

Plus: the Hunter Association scene (revisited at each rank-up) and the Penalty Zone (desert, giant centipedes, 3 minutes).

### Replay

- **GTE-9** Instant dungeon keys reopen cleared gates at a chosen rank with one modifier: Elite (fewer, stronger), Swarm (+50% weaker), Darkness (Perception matters), Time trial (bonus for speed).

## Acceptance criteria (per gate)

- [ ] Greybox played on a headset with smooth movement **and** teleport before any art
- [ ] Worst-case fight holds 72 Hz on a Quest 3
- [ ] A first-time playtester finishes it without help within its target length
- [ ] `GateData` asset filled in (rank, levels, scene, enemies, boss, rooms, objective)

## Open questions

- Prologue length vs. expo path: does the expo build skip the prologue?
