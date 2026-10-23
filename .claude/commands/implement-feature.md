---
description: "Implement a feature from spec rules: plan, tests first, code, self-review, PR"
argument-hint: "<issue # and/or rule IDs, e.g. 42 CMB-11 CMB-12>"
---

Follow the playbook `ai/workflows/implement-feature.md` step by step, with `AGENTS.md` as project context.

Input: $ARGUMENTS

Delegate the implementation to the subagent that owns the area (combat-engineer, systems-engineer, mr-ui-engineer, ai-level-designer or tech-artist). Before the PR, run unity-code-reviewer, and vr-comfort-reviewer if the change is felt in the headset. Stop and ask after the plan step if any rule is ambiguous.
