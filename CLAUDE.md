@AGENTS.md

## Claude Code specifics

- **Subagents** live in `.claude/agents/`: one per workstream (`combat-engineer`, `systems-engineer`, `mr-ui-engineer`, `ai-level-designer`, `tech-artist`) and three reviewers (`unity-code-reviewer`, `vr-comfort-reviewer`, `gdd-consistency-checker`). Delegate to the matching role for implementation, and run the reviewers before opening a PR.
- **Slash commands** live in `.claude/commands/`: `/implement-feature`, `/write-tests`, `/review-pr`, `/playtest-report`, `/weekly-build`, `/sync-gdd`, `/new-spec`. Each one follows a playbook in `ai/workflows/`.
- **Unity Editor:** if a Unity Editor is open on this project, prefer driving it (Unity CLI `unity command ...` via the Pipeline package) over editing files. Never hand-edit scene or prefab YAML.
- Permissions in `.claude/settings.json` are shared by the team; personal overrides go in `.claude/settings.local.json` (git-ignored).
