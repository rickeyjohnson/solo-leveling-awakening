# Review: Unity code review

Review C# changes for correctness and for this project's Unity rules. Report findings; don't rewrite the code unless asked.

## Inputs

The diff (`git diff main...HEAD -- '*.cs' '*.asmdef'`), the issue, and the spec rules it claims to implement.

## Check

### Correctness

- [ ] Does the code do what the cited spec rules say, with the spec's numbers? Quote any mismatch with the rule ID.
- [ ] Null and edge cases: missing references, destroyed objects, zero/negative values, empty collections.
- [ ] Events: every `+=` in `OnEnable` has a matching `-=` in `OnDisable`.
- [ ] State: no static state that survives scene loads by accident; singletons clear themselves in `OnDestroy`.

### Unity and performance

- [ ] No `Find*`, `GetComponent` or `Camera.main` in `Update`/`FixedUpdate` (cache in `Awake`).
- [ ] No allocations in per-frame code (new collections, LINQ, string building, capturing lambdas, boxing).
- [ ] Physics changes in `FixedUpdate`; `NonAlloc` queries; correct layers.
- [ ] Spawned-often objects are pooled.

### Project rules

- [ ] Tunable numbers come from ScriptableObjects, not literals.
- [ ] Systems communicate through event channels; no new direct cross-system references.
- [ ] Code is in the right assembly and namespace; asmdef references don't create cycles.
- [ ] Pure logic has unit tests; test names cite rule IDs.
- [ ] No hand-edited `.unity` / `.prefab` / `.asset` / `.meta` YAML in the diff.
- [ ] Naming and style match `docs/CODING_STANDARDS.md`.

## Report

A list ordered by severity:

```text
[S1|S2|S3] path/File.cs:line — what's wrong — the rule or spec it breaks — suggested fix
```

End with: "Ready to merge after fixes" or "Needs another pass", plus what must be tested on a headset.
