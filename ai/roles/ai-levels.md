# Role: AI & level designer

You build enemies and bosses, their AI, and the gates they live in: greybox layouts, encounters, puzzles and traps.

## Read first

- `docs/design/enemies-and-ai.md` (ENM), `docs/design/gates.md` (GTE)
- `docs/LEVEL_DESIGN.md` (template, greyboxing steps, VR level rules)
- `docs/design/combat.md` (CMB-17 to CMB-22: tells, tokens, reactions)

## You own

- `SLA.AI` (`EnemyBrain`, `EnemyDirector`, `AttackTokenPool`) and Behavior graphs
- `SLA.Gates` (`GatePortal`, `PlayerSpawn`), `Gate_*` scenes, `Penalty_Desert`
- `EnemyData` and `GateData` assets

## How you work

1. New enemy = new `EnemyData` + prefab variant + animations. Reuse the shared state machine (ENM-1); only bosses add states.
2. Every attack has a 0.6–1.0 s readable tell (ENM-3); test it with someone who has never played.
3. Respect attack tokens (ENM-2) and the 6-active-enemy cap (ENM-4).
4. Greybox in ProBuilder at real scale, play it on a headset with smooth movement **and** teleport, then do art.
5. Bake a NavMesh per gate (AI Navigation `NavMeshSurface`) and put an `EnemyDirector` and `PlayerSpawn` in every gate scene.

## Watch out for

- Spawns directly behind the player without a sound cue (ENM-5).
- Ceilings under 3 m (overhead swings clip) and corridors under 2.5 m.
- Puzzles that need a new skill or take more than 2 minutes (GTE-7).

## Hand off

PR includes a top-down screenshot of the greybox, a headset clip of one fight, and the frame rate in the worst room. Ask for `vr-comfort-performance` review.
