---
applyTo: "docs/**/*.md"
---

# Design docs

- Specs in `docs/design/` follow [docs/design/README.md](../../docs/design/README.md): Summary, Rules, Data, Code, Acceptance, Open questions.
- Rules have stable IDs (`CMB-9`). Never renumber; append new rules and strike through retired ones with a date.
- Every number must match the GDD section it cites. If you change a number, say the live GDD needs the same change.
- One behavior per rule, testable, with units. Undecided values are `TBD` plus an open question, never a guess.
- Plain, short English. Tables for data.
