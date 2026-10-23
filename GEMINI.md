@./AGENTS.md

## Gemini CLI specifics

- **Commands** live in `.gemini/commands/` and follow the playbooks in `ai/`:
  - Roles: `/role:combat`, `/role:systems`, `/role:mr-ui`, `/role:ai-levels`, `/role:tech-art` (adopt that workstream's context before a task).
  - Reviews: `/review:unity`, `/review:comfort`, `/review:gdd`.
  - Workflows: `/implement-feature`, `/write-tests`, `/review-pr`, `/playtest-report`, `/weekly-build`, `/sync-gdd`, `/new-spec`.
- Check what context loaded with `/memory show`; reload after edits with `/memory refresh`.
- Never hand-edit Unity scene, prefab or `.meta` YAML.
