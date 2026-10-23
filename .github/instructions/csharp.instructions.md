---
applyTo: "**/*.cs"
---

# C# in this project

Follow [CODING_STANDARDS.md](../../docs/CODING_STANDARDS.md). The rules reviewers check most:

- Namespaces `SLA.Core`, `SLA.Data`, `SLA.Combat`, `SLA.Skills`, `SLA.Progression`, `SLA.Quests`, `SLA.Exercise`, `SLA.AI`, `SLA.Gates`, `SLA.MR`, `SLA.UI`, matching the assembly folder.
- `[SerializeField] private` fields named `_camelCase`; PascalCase types, methods and public members; Allman braces; units in names (`durationSeconds`, `speedMetersPerSecond`).
- Cache components in `Awake`. No `GetComponent`, `Find*` or `Camera.main` in `Update`.
- No allocations in per-frame code: no LINQ, `new` collections, string building or capturing lambdas in `Update`/`FixedUpdate`. Use `NonAlloc` physics queries and pools.
- Physics in `FixedUpdate`. Subscribe to events in `OnEnable`, unsubscribe in `OnDisable`.
- Tunable numbers come from ScriptableObjects (`CombatConfig`, `ProgressionConfig`, `*Data`), with defaults from the spec's Data table.
- Keep logic in plain classes (see `DamageCalculator`, `SquatCounter`) and MonoBehaviours thin. Tests go in `Assets/_Project/Tests/EditMode/`, named after the rule: `CMB_11_FlatHitsDealBluntDamage`.
- Cite the spec rule in a short comment where a number or behavior comes from it: `// CMB-9: swing factor is 0 below 1.5 m/s`.
