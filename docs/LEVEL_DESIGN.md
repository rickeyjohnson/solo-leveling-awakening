# Level design

Eight gates, one template. This page is the working guide for building them; the full level list, enemies and story beats are in the GDD's [Gates & level design](GDD.md#gates--level-design).

## The gate template

![Gate template](images/gate-template.png)

| Part | Size | Purpose | Rules |
| --- | --- | --- | --- |
| Entrance hall | ~6 × 6 m | Safe start, objective, respawn point | No enemies, ever. The gate panel shows rank and objective |
| Combat rooms (2–4) | ~8 × 8 m | One new enemy type or twist per room | Flat floor; no blind corners closer than 2 m; corridors at least 2.5 m wide |
| Secret room (0–1) | ~4 × 4 m | Hidden quests, rare loot | Optional; never required to finish |
| Antechamber | ~5 × 5 m | Heal, change gear | Reaching it unlocks a shortcut from the entrance |
| Boss room | ~12 × 12 m | The fight | No traps; readable silhouettes; clear arena edge |

**Room count by rank:** E–D gates use 2 combat rooms; C-rank and up use 3–4 and add puzzles and traps. The Spire Gate (S) chains several templates as floors. If the schedule slips, B–S gates drop to 2 rooms rather than being cut.

## Level list (main path)

| # | Gate | Rank | Player level | Boss | Owner |
| --- | --- | --- | --- | --- | --- |
| 1 | Prologue: Temple of Commandments | Posted D | 1 | The Giant Statue | |
| 2 | Goblin Warren | E | 1–9 | Goblin Chieftain | |
| 3 | Kasaka's Nest | D | 10–19 | Kasaka | |
| 4 | Job Change: Castle of the Fallen Knight | Trial | 20 | The Iron Commander | |
| 5 | The Hive Below Jamsil | C | 20–29 | Ant Queen | |
| 6 | The Frost Gate (Red Gate) | Red (posted B) | 30–39 | Ice Elf Warlord | |
| 7 | Dungeon Break: Seoul Subway | A | 40–49 | High Orc Warchief | |
| 8 | The Spire Gate | S | 50 | Demon Noble | |

Plus the MR hub, the Hunter Association scene (revisited at each rank-up) and the Penalty Zone.

## Building a gate, step by step

1. **Paper map.** Draw the template on paper or in a whiteboard tool; mark each room's new enemy or twist.
2. **Greybox in ProBuilder.** Grey cubes only, real scale (1 unit = 1 m). Drop in placeholder enemies.
3. **Headset test.** Play it in a Quest build. Walk every corridor with smooth movement *and* teleport. Fix anything that feels cramped, dark or sickening.
4. **Encounter pass.** Place spawners from `GateData`; tune counts so at most 6 enemies are active and 2 attack at once.
5. **Art pass.** Swap greybox for modular kit pieces from the gate's atlas. Bake lighting.
6. **Profile.** 72 Hz with the worst-case fight on screen, on a Quest 3.
7. **Playtest** with someone who has never seen the gate. Watch, don't help.

## VR level-design rules

1. **The camera belongs to the player.** No moving platforms under the player, no forced camera motion, no falls.
2. **Flat floors in combat.** Elevation changes happen between rooms; ramps only outside fights; no stairs under smooth locomotion.
3. **Readable arenas.** Enemies silhouetted against lighter backgrounds; lights placed to lead the eye to exits.
4. **Fair 360°.** Spawns never directly behind the player without a sound cue first.
5. **Teleport-safe.** Every fight must be winnable with the teleport preset: no gaps that need smooth movement to cross.
6. **Scale for VR.** Doors at least 2.2 m tall, ceilings at least 3 m in combat rooms (overhead swings hit low ceilings).

## Puzzles and traps (C–B gates)

Every puzzle uses a physical action the player already learned in combat. Every trap telegraphs for at least one second and never moves the camera.

| Name | Gate | Kind | VR action |
| --- | --- | --- | --- |
| Pressure plates | Hive (C) | Puzzle | Grab and drag a body or resin block onto the plate |
| Resin bridges | Hive (C) | Puzzle | Slash the right cords |
| Gas vents | Hive (C) | Trap | Read the 1.5 s hiss, step out |
| Wall-crawler nests | Hive (C) | Trap | Look up; Dagger Rush the nest |
| Brazier relay | Frost Gate (B) | Puzzle | Carry a torch; protect the flame |
| Ice mirrors | Frost Gate (B) | Puzzle | Grab and rotate mirrors |
| Falling icicles | Frost Gate (B) | Trap | Watch the growing shadow, step aside |
| Pendulum blades | Frost Gate (B) | Trap | Duck or lean |
| Arrow slits | Frost Gate (B) | Trap | Deflect with your weapon |

**Puzzle rules:** a System hint after 60 s stuck (*[Hint: The plate requires weight.]*), two minutes maximum for a first-time player, no traps in boss rooms.

## The MR hub

![MR hub](images/mr-hub-mockup.png)

- The gate opens on the largest clear wall found in the room scan.
- The daily quest's target pads and squat spot go in the clearest 2 × 2 m of floor, facing the gate.
- Nothing spawns on scanned furniture.
- With no room scan (or at the expo), the hub loads **Hub_Virtual**, a small apartment with the same features.
