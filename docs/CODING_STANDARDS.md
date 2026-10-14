# Coding standards

Short on purpose. `.editorconfig` enforces formatting in Rider and VS Code; this page covers what a formatter can't.

## C# style

| Thing | Convention | Example |
| --- | --- | --- |
| Types, methods, properties, events | PascalCase | `class ParryDetector`, `void ApplyDamage()` |
| Public fields on ScriptableObjects | camelCase (Unity Inspector convention) | `public float baseDamage;` |
| Private fields | `_camelCase` | `private float _cooldownRemaining;` |
| Serialized private fields | `[SerializeField] private` + `_camelCase` | `[SerializeField] private WeaponData _weapon;` |
| Constants | PascalCase | `const float ParryWindowBase = 0.15f;` |
| Interfaces | `I` + PascalCase | `IDamageable` |
| Namespaces | `SLA.<Area>` | `SLA.Combat` |
| Braces | New line (Allman) | |
| Units | In the name when not obvious | `hitSpeedMetersPerSecond`, `durationSeconds` |

- Prefer `[SerializeField] private` over `public` fields on MonoBehaviours.
- One MonoBehaviour per file; the file name matches the class.
- No magic numbers in gameplay code: put them in a ScriptableObject or a named constant.

## Unity rules

1. **No `Find`, `GetComponent` or `FindObjectOfType` in `Update`.** Cache references in `Awake`.
2. **Physics in `FixedUpdate`, input and visuals in `Update`.** Weapon forces, hit detection and Rigidbody changes go in `FixedUpdate`.
3. **Subscribe in `OnEnable`, unsubscribe in `OnDisable`.** Every event channel, every C# event.
4. **No allocations in per-frame code.** No `new List<>()`, LINQ, string concatenation or lambdas capturing variables inside `Update`. The Quest's garbage collector causes visible hitches.
5. **Never move the camera.** No script writes to the XR Origin's camera transform; the player's head belongs to the player (see the GDD's comfort rules).
6. **Layers and tags are shared config.** Add one only through a PR that updates the table below.
7. **Prefab variants** for enemy and weapon variations; never duplicate a prefab and edit the copy.
8. **Pool** anything spawned often: hit sparks, damage numbers, projectiles, enemies.

### Layers

| Layer | Used by |
| --- | --- |
| `Player` | The player's body hurtbox capsule |
| `PlayerHands` | Physics hands |
| `PlayerWeapon` | Held weapons |
| `Enemy` | Enemy bodies and hurtboxes |
| `EnemyWeapon` | Enemy weapon hitboxes (parry targets) |
| `Environment` | Walls, floors, props |
| `RoomScan` | MR room geometry from AR Foundation |
| `UI` | World-space System windows |

## Comments and docs

- Comment **why**, not what. The code says what.
- Every public API in `SLA.Core` and `SLA.Data` gets an XML `/// <summary>`.
- A change to a design rule (a formula, a timing, a number in the GDD) updates the GDD in the same PR.

## Tests

- **Pure logic** (damage, XP, drop tables, rep counting, save/load) is written as plain C# classes so it can be unit-tested in Edit Mode.
- **Recorded-motion tests:** squat and punch detectors are tested against CSV recordings of real sets in `Tests/Recordings/`.
- Run the test suite before opening a PR that touches `SLA.Core`, `SLA.Data` or `SLA.Gameplay`.

## Performance habits

- Check the Profiler on a headset before and after any change to combat, AI or VFX.
- Keep `Update` methods short; if many objects need ticking, use one manager that loops over them.
- Use `Physics.OverlapSphereNonAlloc` and other `NonAlloc` APIs.
