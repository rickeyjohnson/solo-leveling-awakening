# Combat (`CMB`)

| | |
| --- | --- |
| Owner | Combat engineer |
| GDD section | [Combat design](../GDD.md#combat-design) |
| Code | `SLA.Combat` (`DamageCalculator`, `WeaponHitDetector`, `SwingTracker`, `Health`, `HitZoneMarker`) |
| Data | `CombatConfig.asset`, `WeaponData` assets |
| Status | Draft |

## Summary

Physics melee: weapons are real physics objects held with XR Interaction Toolkit's **Velocity Tracking**, so they collide, carry weight and drag through enemies. Damage depends on how hard and where you hit. Enemies are animated (not physics-simulated) and ragdoll only on death. Dual daggers are the signature weapons.

## Rules

### Holding weapons

- **CMB-1** Weapons are `XRGrabInteractable` with movement type **Velocity Tracking**. They never pass through walls or enemy weapons.
- **CMB-2** Each hand is a Rigidbody that follows its controller. If the hand is blocked more than **0.3 m** from the controller, show a translucent "ghost hand" at the real controller position.
- **CMB-3** Below a weapon's `strengthRequirement`, its follow strength drops so it lags behind the hand. Two-handed weapons regain full control when held with both hands.

### Damage

- **CMB-4** `Damage = base × swing × (1 + 0.02 × STR) × zone × crit − armor`, floored at 0.
- **CMB-5** Swing factor = 0 below **1.5 m/s** relative hit speed (a tap does nothing), linear to **1.0 at 5 m/s**, capped at **1.2**.
- **CMB-6** Zone multipliers: head **2.0**, torso **1.0**, limb **0.6**, glowing weak point **2.5**. Colliders without a `HitZoneMarker` count as torso.
- **CMB-7** Critical hits multiply by **1.5**. Crit chance comes from Perception (value TBD in tuning).
- **CMB-8** Staggered targets ignore armor.

### Anti-cheese

- **CMB-9** A hit only counts if the weapon tip travelled at least **0.3 m in the last 0.25 s**.
- **CMB-10** A weapon damages the same target at most once every **0.25 s**.
- **CMB-11** Slashes count only with the blade edge; stabs only along the tip direction. Hits with the flat or pommel deal **30%** as blunt damage.
- **CMB-12** On a hit, the weapon's follow strength drops for **0.1 s** so the blade drags instead of passing through.

### Defense

- **CMB-13** Block: the player's weapon in the path of an enemy attack reduces damage by **80%** and costs **10 Fatigue**.
- **CMB-14** Perfect parry: hitting the enemy's weapon during its active frames at **2 m/s or more**, inside the parry window (**150 ms + 4 ms per Perception point above 10**), negates the damage and staggers the enemy for **1.5 s**.
- **CMB-15** Physical dodge: the player's hurtbox is a capsule from the headset down; leaning or stepping out of an attack avoids it.
- **CMB-16** Dash: see [SKL-1](skills.md).

### Pacing and feedback

- **CMB-17** Every enemy attack has a **0.6–1.0 s** windup with a red weapon glow and a sound cue (the "tell").
- **CMB-18** At most **2** enemies attack at once (attack tokens; see [ENM-2](enemies-and-ai.md)).
- **CMB-19** An enemy winding up outside the player's view triggers a spatial sound and a red edge glow on that side.
- **CMB-20** Every hit fires four channels, scaled by damage: controller haptics (**40–120 ms**), enemy-only hitstop (**40–60 ms**), cyan spark VFX + System-blue damage number, and a layered impact sound at the hit point.
- **CMB-21** The camera never freezes, shakes or gets pushed by hits (see CMF-1).
- **CMB-22** Enemies use animated hit reactions (flinch, stagger, knockdown by damage threshold) and switch to a ragdoll on death.

## Data

| Field | Asset | Default |
| --- | --- | --- |
| `minHitSpeed`, `fullHitSpeed`, `swingFactorCap` | `CombatConfig` | 1.5, 5, 1.2 |
| `strengthDamagePerPoint` | `CombatConfig` | 0.02 |
| `head/torso/limb/weakPointMultiplier` | `CombatConfig` | 2.0 / 1.0 / 0.6 / 2.5 |
| `critMultiplier`, `bluntFraction` | `CombatConfig` | 1.5, 0.3 |
| `minSwingArc`, `swingArcWindow`, `perTargetCooldown`, `contactDragSeconds` | `CombatConfig` | 0.3, 0.25, 0.25, 0.1 |
| `blockDamageReduction`, `blockFatigueCost` | `CombatConfig` | 0.8, 10 |
| `parryWindowBase`, `parryWindowPerPerception`, `parryMinSpeed`, `staggerSeconds` | `CombatConfig` | 0.15, 0.004, 2, 1.5 |
| `baseDamage`, `strengthRequirement`, `massKg`, `twoHanded` | `WeaponData` | per weapon ([items](items-and-economy.md)) |

## Code

- `DamageCalculator` (pure, unit-tested) implements CMB-4 to CMB-8 and the parry window.
- `WeaponHitDetector` on each held weapon implements CMB-9, CMB-10; **CMB-11, CMB-12 and crit are not built yet**.
- `Health` implements damage, death and stagger; raises `Damaged` and `Died`.
- Feedback (CMB-20) subscribes to `WeaponHitDetector.Hit`.

## Acceptance criteria

- [ ] A slow tap does no damage; a full swing on a goblin's torso with the Rusty Dagger at STR 10 does about 12 (CMB-4/5)
- [ ] Wrist-waggling against an enemy deals no damage (CMB-9)
- [ ] A parry inside the window staggers the enemy for 1.5 s and the next hit ignores armor
- [ ] Four feedback channels fire on every hit, and the view never shakes
- [ ] 6 enemies fighting at once still holds 72 Hz on a Quest 3
- [ ] Unit tests pass for `DamageCalculator` and `SwingTracker`

## Open questions

- Crit chance per Perception point.
- Whether contact drag (CMB-12) feels better as a follow-strength drop or a brief joint.
