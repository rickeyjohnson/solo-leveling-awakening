---
description: "Review a PR with the Unity, comfort and GDD reviews that apply"
argument-hint: "<PR number or branch; empty = current branch>"
---

Follow the playbook `ai/workflows/review-pr.md` step by step, with `AGENTS.md` as project context.

Input: $ARGUMENTS

Run the matching reviewer subagents (unity-code-reviewer, vr-comfort-reviewer, gdd-consistency-checker) in parallel, then merge their findings into one list. Don't post anything to GitHub unless asked.
