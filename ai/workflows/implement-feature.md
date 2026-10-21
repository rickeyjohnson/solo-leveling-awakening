# Workflow: Implement a feature from a spec

Input: an issue number and/or spec rule IDs (e.g. `CMB-11 CMB-12`).

1. **Understand.** Read the issue, the cited rules in `docs/design/`, and the role playbook for that area (`ai/roles/`). Restate each rule in one line with its numbers. List anything ambiguous and ask before coding.
2. **Branch.** `feature/<area>-<short-name>` from an up-to-date `main`.
3. **Plan.** Name the files to add or change, the ScriptableObject fields (with spec defaults), the events raised or listened to, and the tests. Keep the plan to one issue's worth of change.
4. **Test first for pure logic.** Write failing EditMode tests named after the rules (`CMB_11_FlatHitsDealBluntDamage`).
5. **Implement.** Pure logic in plain classes; thin MonoBehaviours; numbers in ScriptableObjects; no hand-edited scene/prefab YAML (list the Editor steps a human must do instead).
6. **Run the tests** (command in `AGENTS.md`) and `bash tools/check-repo.sh`.
7. **Self-review** with `ai/reviews/unity-code-review.md` and, for anything felt in the headset, `ai/reviews/vr-comfort-performance.md`. Fix what they find.
8. **Commit and PR.** Commit messages cite rule IDs; the PR uses the template, lists the rules implemented, the tests, the manual Editor steps, and what to check on the headset.

Done means the PR is ready for a human to test on a Quest, not that the feature is merged.
