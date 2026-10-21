# Workflow: Write a new spec

Input: a feature or system name and the GDD section it comes from.

1. Copy `docs/templates/FEATURE_SPEC.md`. New system → `docs/design/<system>.md` with a new 2–3 letter prefix; feature inside an existing system → add rules to that spec (append, never renumber).
2. Pull every number from the GDD. If the GDD has no number, write the rule with `TBD` and add an open question; don't invent one.
3. One behavior per rule, testable, with units.
4. Fill **Data** (which ScriptableObject field holds each tunable value and its default) and **Code** (namespace, classes, events).
5. Acceptance criteria are verified on a Quest, plus unit tests for pure logic.
6. Add the spec to the index in `docs/design/README.md`.
7. Run `ai/reviews/gdd-consistency.md`, then open a `docs/spec-<name>` PR.
