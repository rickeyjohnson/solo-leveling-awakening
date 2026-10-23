# Workflow: Write unit tests

Input: a class, a folder, or rule IDs.

1. Find the pure logic (no `MonoBehaviour`, no scene). If the logic is buried in a MonoBehaviour, propose extracting it into a plain class first (like `SquatCounter` and `SquatCounterDriver`).
2. List the behaviors to cover from the spec rules: the normal case, each boundary (exactly at a threshold, just below, just above), and each rejection rule.
3. Write NUnit EditMode tests in `Assets/_Project/Tests/EditMode/`, in the `SLA.Tests` namespace. Name each test after its rule: `QST_10_BowingIsRejected`.
4. Create ScriptableObject configs with `ScriptableObject.CreateInstance<T>()` in `[SetUp]` and destroy them in `[TearDown]`; use the spec defaults, never production assets.
5. For expected error logs use `LogAssert.Expect`, never `ignoreFailingMessages`.
6. For exercise detectors, add or reuse CSV recordings in `Tests/Recordings/` and replay them.
7. Run the tests (command in `AGENTS.md`) and report pass/fail counts.

Don't change production code to make a test pass without saying why; a failing test may have found a real bug.
