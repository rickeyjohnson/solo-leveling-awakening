# Review: VR comfort & performance

Check that a change can't make players sick and won't break the Quest frame budget. Comfort problems are `S1` bugs.

## Inputs

The diff, the scenes or prefabs touched, and any headset notes or OVR Metrics numbers in the PR.

## Comfort (docs/design/comfort-and-accessibility.md)

- [ ] **CMF-1:** nothing moves, rotates, shakes, freezes or pushes the camera / XR Origin except player-driven locomotion, Dash and `PlayerSpawn` on scene load (behind a fade).
- [ ] No `Time.timeScale` changes used as "hitstop" (that freezes everything, including head-attached effects).
- [ ] **CMF-2:** movement has no acceleration curves; no stat or effect changes real movement speed.
- [ ] **CMF-3–5:** no slopes/stairs in combat spaces, soft wall collisions, no horizon roll.
- [ ] UI is never head-locked in diegetic mode; text is readable (UI-9).
- [ ] No full-screen flashes; danger isn't shown by red alone (CMF-11, CMF-12).
- [ ] Works with the Comfortable preset (teleport) (CMF-7).

## Performance (docs/ARCHITECTURE.md#performance-budget)

- [ ] 72 Hz in gates / 90 Hz target in the hub. If the PR has no numbers, ask for OVR Metrics captures of the worst case.
- [ ] Draw calls under 150, triangles on screen under 750K; new art within ART_PIPELINE budgets.
- [ ] At most 6 physics-active enemies; no new real-time lights beyond the one directional; no post-processing.
- [ ] VFX use the Particle System, at most 200 particles each, no lights on particles.
- [ ] Per-frame code doesn't allocate; expensive work is spread out or cached.
- [ ] Textures ASTC, meshes compressed, audio import settings per ART_PIPELINE.

## Report

```text
[S1|S2|S3] where — what could make players sick or drop frames — rule — fix
```

Finish with the headset checks a human must do before merge (e.g. "turn quickly in the boss room and watch for judder").
