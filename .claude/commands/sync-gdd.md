---
description: "Refresh docs/GDD.md from the live GDD and check the specs still agree"
argument-hint: "<optional: path to a new Markdown export>"
---

Follow the playbook `ai/workflows/sync-gdd.md` step by step, with `AGENTS.md` as project context.

Input: $ARGUMENTS

Then run the gdd-consistency-checker subagent on the result.
