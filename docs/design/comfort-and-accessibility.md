# Comfort & accessibility (`CMF`)

| | |
| --- | --- |
| Owner | Everyone; MR & UI engineer keeps the settings |
| GDD section | [VR comfort & accessibility](../GDD.md#vr-comfort--accessibility) |
| Status | Draft |

## Summary

These rules apply to **every** system. A feature that breaks one is an `S1 crash/sickness` bug. The default is smooth movement with a vignette and snap turning; three presets follow Meta's Recommended / Comfortable / Advanced guidance.

## Rules

### Hard rules (never break)

- **CMF-1** The camera belongs to the player. No forced camera movement, camera shake, camera hitstop, or knockback on the view, from any system or cutscene.
- **CMF-2** No acceleration curves: movement speed is quantized (stopped, walk, sprint). No stat changes real movement speed.
- **CMF-3** Flat floors in combat; elevation changes between rooms; no stairs under smooth movement.
- **CMF-4** Walking into a wall slides the player along it; the view never jolts to a stop.
- **CMF-5** Stable horizon: no rolling camera, swaying platforms or tilted rooms.
- **CMF-6** A gentle break reminder appears after 30 minutes of play.

### Presets

| Setting | Recommended (default) | Comfortable | Advanced |
| --- | --- | --- | --- |
| Movement | Smooth, 1.5 m/s walk, 3.0 m/s sprint | Teleport arc + room-scale | Smooth |
| Turning | Snap 45° | Snap 45° | Smooth, adjustable |
| Vignette | Medium, while moving | Strong | Off |
| Dash | Blink with brief fade | Blink with brief fade | Blink, no fade |

- **CMF-7** Every fight must be winnable with the Comfortable preset (teleport).

### Accessibility

- **CMF-8** Left-handed mode mirrors all controls and holster sides.
- **CMF-9** Button-only mode for every skill gesture; a low-impact daily quest (arm-only rounds).
- **CMF-10** Subtitles always on; adjustable text size; a high-contrast UI option.
- **CMF-11** Danger is never shown by red alone (shape and sound too); off-screen attacks get a directional edge glow as well as sound.
- **CMF-12** No full-screen flashes; a setting reduces flash and bloom.
- **CMF-13** Height is calibrated at the start; standing height drives squats and UI placement. Seated play is not supported, and the store page and title screen say so.

### Physical safety

- **CMF-14** A play-area check runs at launch and before each daily quest. The Guardian boundary is never disabled.
- **CMF-15** Warm-up (daily quest) comes before combat; the game never asks for burst sprints from cold.

### Performance is comfort

- **CMF-16** 72 Hz held in gates and 90 Hz in the hub where possible. A dropped-frame regression is treated as a comfort bug.

## Acceptance criteria

- [ ] Median SSQ increase under 20 after 20 minutes in playtests
- [ ] Each preset tested by at least one VR-new player per playtest round
- [ ] A reviewer checks CMF-1 to CMF-5 on every PR that touches the camera, movement, combat feedback or cutscenes
