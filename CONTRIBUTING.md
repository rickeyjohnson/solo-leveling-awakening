# Contributing

Five people, one Unity project, binary files everywhere. These rules keep us from overwriting each other's work. Read once, follow always.

## The short version

1. One issue per task, assigned to one person, in a milestone.
2. One branch per issue, off `main`.
3. Open a pull request early (draft is fine). One teammate reviews before merge.
4. **Test on the headset before you ask for review.**
5. Never edit a scene you don't own. Work in prefabs.
6. Never commit raw source art (`.blend`, `.psd`, `.spp`) or study data.

## Setup (once per clone)

```bash
bash tools/setup-git.sh
```

This turns on Git LFS for the repo and registers Unity's Smart Merge for scenes and prefabs. Install Git LFS first if the script asks ([ONBOARDING](docs/ONBOARDING.md#2-install-the-tools)).

## Branches

`main` is always buildable to the Quest. Nobody pushes to `main` directly.

| Kind | Name | Example |
| --- | --- | --- |
| Feature | `feature/<area>-<short-name>` | `feature/combat-parry-window` |
| Bug fix | `fix/<area>-<short-name>` | `fix/ui-status-window-flicker` |
| Spike (experiment) | `spike/<short-name>` | `spike/wall-plane-detection` |
| Docs | `docs/<short-name>` | `docs/art-pipeline-budgets` |

Areas match the issue labels: `combat`, `systems`, `mr-ui`, `ai-levels`, `art-audio`, `research`.

Keep branches short-lived: merge within a sprint (two weeks). Rebase or merge `main` into your branch at least twice a week.

## Commits

- Imperative mood, under 72 characters: `Add parry window scaling from PER`.
- One logical change per commit. Don't mix a scene edit with a script refactor.
- Reference the issue in the body: `Closes #42`.

## Pull requests

1. Fill in the template (it asks what changed, how you tested it, and on which headset).
2. Attach a short clip or screenshot for anything visual.
3. Request one reviewer. The reviewer runs the branch on a headset if it touches gameplay.
4. Squash-merge into `main`. Delete the branch.

**Definition of done**

- [ ] Works in a Quest build, not only in the editor
- [ ] Frame rate still at 72 Hz or better in the affected scene
- [ ] No new console errors or warnings
- [ ] Data lives in ScriptableObjects, not hard-coded numbers
- [ ] Docs updated if the change affects design (GDD) or process

## Scenes and prefabs

Unity scenes are large YAML files; two people editing one scene means a painful merge.

- **Every scene has one owner**, listed in the table below. Others ask the owner, or work in a prefab.
- Build features as **prefabs** and test them in your own sandbox scene: `Scenes/Sandbox/<YourName>.unity` (never shipped).
- Put tuning numbers in **ScriptableObject** assets, so balancing never touches a scene.

| Scene | Owner |
| --- | --- |
| Bootstrap | Producer / tech lead |
| Hub_MR, Hub_Virtual | MR & UI engineer |
| Gate_* scenes | AI & level designer |
| Penalty_Desert | AI & level designer |
| Title, Setup | MR & UI engineer |

Update this table when roles are assigned.

## Large files and Git LFS

- Binary assets (`.fbx`, `.png`, `.wav`, and so on) go through **Git LFS** automatically ([.gitattributes](.gitattributes)).
- GitHub's free plan includes **10 GiB of LFS storage and 10 GiB of download bandwidth a month**. When we run out, LFS stops working until the next month. So:
  - Keep raw source art in the shared drive (`ArtSource/` is git-ignored).
  - Commit only game-ready exports, at the sizes in [ART_PIPELINE](docs/ART_PIPELINE.md).
  - Don't commit the same 50 MB texture ten times while iterating; iterate locally, commit the final.
- The producer checks LFS usage at every sprint review.

## Issues

Use the templates: **Bug**, **Feature / task**, or **Spike**. Labels:

| Label | Meaning |
| --- | --- |
| `S1 crash/sickness` | Crash or motion-sickness bug. Blocks the weekly build; fix before anything else |
| `S2 broken feature` | A feature doesn't work |
| `S3 minor` | Cosmetic or minor |
| `area: …` | Which workstream owns it |
| `type: task`, `type: spike` | Planned work, or a time-boxed experiment |

Every issue goes in a [milestone](https://github.com/rickeyjohnson/solo-leveling-awakening/milestones) (one per production phase).

## Weekly build

Every Friday the producer builds `main` to a Quest and the whole team plays it for 15 minutes. Anything that breaks becomes an issue before the weekend.
