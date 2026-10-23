# AI setup

The team uses **Claude**, **Gemini**, **GitHub Copilot** and **ChatGPT**. Every tool gets the same project context and the same playbooks, so a combat task gets the same rules whichever assistant you open.

## How it fits together

| Layer | File(s) | Read by |
| --- | --- | --- |
| Shared context | [`AGENTS.md`](../AGENTS.md) | Copilot, Codex, Cursor directly; Claude via `CLAUDE.md`; Gemini via `GEMINI.md` |
| Playbooks (source of truth) | `ai/roles/`, `ai/reviews/`, `ai/workflows/` | Every tool, via the adapters below |
| Claude Code | [`CLAUDE.md`](../CLAUDE.md), `.claude/agents/`, `.claude/commands/`, `.claude/settings.json` | Claude Code |
| Gemini CLI | [`GEMINI.md`](../GEMINI.md), `.gemini/commands/` | Gemini CLI |
| GitHub Copilot | `.github/copilot-instructions.md`, `.github/instructions/`, `.github/prompts/`, `.github/agents/` | Copilot in VS Code / Rider / GitHub |
| ChatGPT (and Claude.ai / Gemini web) | [`ai/chat/PROJECT_INSTRUCTIONS.md`](chat/PROJECT_INSTRUCTIONS.md) | Paste into a ChatGPT Project's instructions; upload the files it lists |

**Edit playbooks, not adapters.** Adapters only say "read this playbook". To change how a review works, edit `ai/reviews/<name>.md` and every tool picks it up.

## Playbooks

### Roles: who does the work

| Role | Playbook | Specs owned |
| --- | --- | --- |
| Combat engineer | [roles/combat.md](roles/combat.md) | combat, skills |
| Systems engineer | [roles/systems.md](roles/systems.md) | progression, quests & exercise, items |
| MR & UI engineer | [roles/mr-ui.md](roles/mr-ui.md) | MR hub, System UI, comfort settings |
| AI & level designer | [roles/ai-levels.md](roles/ai-levels.md) | enemies & AI, gates |
| Tech artist & audio | [roles/tech-art.md](roles/tech-art.md) | art direction, VFX, audio |

### Reviews: checks before a PR

| Review | Playbook | Run when |
| --- | --- | --- |
| Unity code review | [reviews/unity-code-review.md](reviews/unity-code-review.md) | Every PR with C# |
| VR comfort & performance | [reviews/vr-comfort-performance.md](reviews/vr-comfort-performance.md) | Camera, movement, combat feedback, VFX, scenes |
| GDD consistency | [reviews/gdd-consistency.md](reviews/gdd-consistency.md) | Any design change, spec edit or new feature |

### Workflows: repeatable tasks

| Workflow | Playbook |
| --- | --- |
| Implement a feature from a spec | [workflows/implement-feature.md](workflows/implement-feature.md) |
| Write unit tests | [workflows/write-tests.md](workflows/write-tests.md) |
| Review a PR (runs the three reviews) | [workflows/review-pr.md](workflows/review-pr.md) |
| Playtest report | [workflows/playtest-report.md](workflows/playtest-report.md) |
| Weekly build checklist | [workflows/weekly-build.md](workflows/weekly-build.md) |
| Sync the GDD export | [workflows/sync-gdd.md](workflows/sync-gdd.md) |
| Write a new spec | [workflows/new-spec.md](workflows/new-spec.md) |

## How to call them

| | Claude Code | Gemini CLI | Copilot (VS Code) | ChatGPT |
| --- | --- | --- | --- | --- |
| Role | "Use the combat-engineer agent to ..." | `/role:combat <task>` | Pick the **Combat engineer** agent | Paste the role playbook, then the task |
| Review | "Run unity-code-reviewer on this diff" | `/review:unity` | `/review-unity` prompt | Paste the review playbook + diff |
| Workflow | `/implement-feature CMB-9` | `/implement-feature CMB-9` | `/implement-feature` prompt | Paste the workflow playbook |

## Team rules for using AI

1. **You own what you commit.** Read every line an assistant writes, and test gameplay on a headset before asking for review.
2. **Cite the spec.** Give the assistant rule IDs (`CMB-9`), not vague goals. If it invents numbers, push back.
3. **No secrets or personal data in prompts:** no API keys, passwords, or user-study participant data.
4. **Unity files:** assistants must not hand-edit scene, prefab, asset or `.meta` YAML. They write C# and editor scripts; you apply changes in the Editor.
5. **Small steps:** one issue, one branch, a small diff. Big AI-generated rewrites are hard to review and usually wrong somewhere.
6. **Academic integrity:** follow the course syllabus and your professor's policy on AI for the **written report and assignments**, and disclose AI help the way the course requires. AI-written prose in graded work is your responsibility.
7. **Art and IP:** don't use AI to imitate official *Solo Leveling* art, music or characters' likenesses. Note any AI-generated asset in `CREDITS.md`.
8. **Paid tools are personal:** the project budget is $0; don't add paid AI services to the repo's automation without a team decision.
