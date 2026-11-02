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
| Movement | Smooth, 1.5 m/s walk, 3.0 m/s sprint | Teleport arc + room-scale | Smooth, same speeds |
| Turning | Snap 45° | Snap 45° | Smooth, 60°/s by default (adjustable) |
| Vignette | Medium (aperture 0.70, feathering 0.20), only while moving or turning by stick | Strong (aperture 0.55, feathering 0.15) | Off |
| Dash | Blink with brief fade | Blink with brief fade | Blink, no fade |

- **CMF-7** Every fight must be winnable with the Comfortable preset (teleport).

How the presets behave:

- **Smooth movement** is head-relative. Inside the stick's dead zone the player stands still; any deflection past it moves at exactly walk speed, or sprint speed, so the stick's direction is analog but its magnitude isn't (CMF-2). Clicking the movement stick toggles sprint; sprint ends when the stick returns to centre.
- **Sticks:** the movement stick (left; right in left-handed mode, CMF-8) moves. The other stick turns sideways and, pushed forward, aims the teleport arc. In Comfortable both sticks aim the arc and turn.
- **Teleport** lands only on NavMesh-walkable floor; an arc aimed anywhere else is grey and doesn't teleport. The view fades to black over 0.1 s, moves, and fades back in.
- **No falling:** gravity is off while every floor is flat (CMF-3), so open edges and leaning can't drop the view. It comes back with a fall guard when a gate gets elevation changes. The step offset is 0.05 m, so walking into a low prop never lifts the view.
- **Vignette** values are the XRI Tunneling Vignette's (a smaller aperture is stronger); it eases in and out over 0.3 s.

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

## Code

- `ComfortPreset` assets in `Settings/Comfort/` (`Recommended`, `Comfortable`, `Advanced`) hold the presets table; `ComfortPresetTests` checks them against it.
- The `PlayerRig` prefab (`Prefabs/Player/`, built by `PlayerRigSetup`) carries `ComfortSettings`, which applies `settings.comfortPreset` and `settings.leftHanded` live whenever `OnSettingsChanged` is raised.
- `QuantizedMoveProvider` + `QuantizedMove` + `SprintToggle` (CMF-2), `HandednessMap` (CMF-8), `NavMeshTeleportationArea` and `TeleportFade` (CMF-7), `HeightCalibrator` in Setup (CMF-13, QST-8). Tested in `LocomotionTests` and `HeightCalibrationTests`.

## Acceptance criteria

- [ ] Median SSQ increase under 20 after 20 minutes in playtests
- [ ] Each preset tested by at least one VR-new player per playtest round
- [ ] A reviewer checks CMF-1 to CMF-5 on every PR that touches the camera, movement, combat feedback or cutscenes
