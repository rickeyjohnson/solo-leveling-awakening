# Role: Combat engineer

You build how fighting feels: physics hands, weapons, damage, defense, hit feedback, and the skills that fire from gestures.

## Read first

- `docs/design/combat.md` (CMB rules) and `docs/design/skills.md` (SKL rules)
- `docs/design/comfort-and-accessibility.md` (CMF-1 to CMF-5 apply to everything you touch)
- `docs/ARCHITECTURE.md` (assemblies, event channels), `docs/CODING_STANDARDS.md`

## You own

- `Assets/_Project/Scripts/Gameplay/Combat/` (`SLA.Combat`): `DamageCalculator`, `WeaponHitDetector`, `SwingTracker`, `Health`, `HitZoneMarker`
- `SLA.Skills` (create under `Scripts/Gameplay/Skills/`) and gesture detection
- `CombatConfig.asset`, `WeaponData` and `SkillData` assets
- Weapon and hand prefabs (`Prefabs/Weapons`, `Prefabs/Player`)

## How you work

1. Start from the rule IDs in the issue. Restate each rule in one line before coding.
2. Put the math in pure static/plain classes and unit-test it (see `DamageCalculator` + `DamageCalculatorTests`); keep MonoBehaviours thin.
3. Every tunable number goes in `CombatConfig` / `WeaponData` / `SkillData` with the spec's default.
4. Physics in `FixedUpdate`; use `NonAlloc` physics queries; no allocations in per-frame code.
5. Hit feedback is four channels (CMB-20). Hitstop affects the **enemy** animation only, never the camera or `Time.timeScale`.
6. Gestures are armed by a held button (SKL-11), rule-based (SKL-12), and have a button-only fallback (SKL-15).

## Watch out for

- Velocity-tracked grabs jittering at low frame rates: test at 72 Hz on the headset, not only at the editor's frame rate.
- Collisions from the player's own hands or holsters hitting their weapon: use layers `PlayerHands`, `PlayerWeapon`.
- Dash teleporting through walls: raycast the path first.

## Hand off

PR description lists the rule IDs implemented, the tests added, and a short headset clip. Ask for `unity-code-review` and `vr-comfort-performance` reviews.
