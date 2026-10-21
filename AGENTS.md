# AGENTS.md

Instructions for AI coding agents (Claude Code, Gemini CLI, GitHub Copilot, Codex, Cursor and others) working in this repository. Humans: this is also the fastest overview of the project.

## The project

**Solo Leveling: Awakening** is a first-person VR and mixed-reality action RPG for **Meta Quest 3 / 3S**, built in **Unity 6.3 LTS (URP)** by a five-person senior-project team (Oct 2026 to Apr 2027). The player's real room (passthrough) is the hub, gates open on their wall, combat is physics melee, and real squats and punches earn stat points. Unofficial, non-commercial fan project.

## Where things are

| Need | Go to |
| --- | --- |
| What to build, exactly (numbered rules) | `docs/design/*.md` (index: `docs/design/README.md`) |
| Why, and the full vision | `docs/GDD.md` (export of the live team doc) |
| Code layout, assemblies, packages, game states, save format | `docs/ARCHITECTURE.md` |
| C# and Unity rules | `docs/CODING_STANDARDS.md` |
| Branches, commits, PRs, scene ownership, Git LFS | `CONTRIBUTING.md` |
| Art, audio, naming, budgets | `docs/ART_PIPELINE.md` |
| Gate building process | `docs/LEVEL_DESIGN.md` |
| Tests, playtests, the user study | `docs/TESTING.md` |
| Schedule and scope | `docs/ROADMAP.md` |
| Agent roles, reviews and workflows | `ai/README.md` |

## Repository layout

```text
SoloLevelingAwakening/        Unity project (Assets/_Project/** is ours; Assets/Samples, ThirdParty are imported)
  Assets/_Project/Scripts/    Core, Data, Gameplay, MR, UI, Editor (one assembly each, namespace SLA.*)
  Assets/_Project/Tests/      EditMode tests (NUnit)
docs/                         GDD, design specs, process docs, images
ai/                           tool-neutral agent playbooks (roles, reviews, workflows)
tools/                        setup and check scripts
.github/                      templates, workflows, Copilot instructions and prompts
```

If `SoloLevelingAwakening/` does not exist yet, the Unity project hasn't been created; see `docs/ONBOARDING.md` step 5.

## Rules that are never optional

1. **Implement from the spec.** Find the rule IDs (e.g. `CMB-9`) in `docs/design/` and cite them in commits, PR descriptions and test names. If the spec is unclear or wrong, stop and say so; don't invent numbers.
2. **Comfort is a hard requirement** (`docs/design/comfort-and-accessibility.md`): never move, shake, freeze or push the camera; no acceleration curves; no stat changes real movement speed. A violation is an `S1` bug.
3. **Performance budget:** 72 Hz in gates (12 ms frame), under 150 draw calls, under 750K triangles on screen, at most 6 physics-active enemies. No allocations in per-frame code.
4. **Never hand-edit `.unity`, `.prefab`, `.asset` or `.meta` YAML.** Change scenes and assets through the Unity Editor (or editor scripts). Every new asset is committed with its `.meta`.
5. **Tunable numbers live in ScriptableObjects** (`CombatConfig`, `ProgressionConfig`, `*Data` assets), never hard-coded in gameplay code.
6. **Pure logic is plain C# with unit tests** in `Assets/_Project/Tests/EditMode/`. Follow the existing `DamageCalculator` / `SquatCounter` pattern.
7. **Systems talk through ScriptableObject event channels**, not direct references (`docs/ARCHITECTURE.md#event-channels`).
8. **Scene ownership:** don't modify a scene you weren't asked to; build features as prefabs and test in `Scenes/Sandbox/Sandbox_<Name>.unity`.
9. **Never commit** `Library/`, `Temp/`, builds, raw source art (`.blend`, `.psd`, `.spp`), secrets, or user-study participant data.
10. **IP:** no anime footage, soundtrack, voice clips, manhwa art or official logos. Don't generate images imitating official Solo Leveling art.

## Code conventions (short version)

- Namespaces `SLA.Core`, `SLA.Data`, `SLA.Combat`, `SLA.Skills`, `SLA.Progression`, `SLA.Quests`, `SLA.Exercise`, `SLA.AI`, `SLA.Gates`, `SLA.MR`, `SLA.UI`.
- `[SerializeField] private` fields named `_camelCase`; PascalCase types and methods; Allman braces; units in names (`durationSeconds`).
- Cache components in `Awake`; physics in `FixedUpdate`; subscribe in `OnEnable`, unsubscribe in `OnDisable`; pool frequently spawned objects.
- Details: `docs/CODING_STANDARDS.md` and `.editorconfig`.

## Commands

```bash
bash tools/setup-git.sh            # once per clone: Git LFS + Unity Smart Merge
bash tools/check-repo.sh           # same hygiene checks CI runs (meta files, forbidden files, large files)
```

Unity EditMode tests (macOS path; adjust the version to `ProjectVersion.txt`):

```bash
"/Applications/Unity/Hub/Editor/6000.3.26f1/Unity.app/Contents/MacOS/Unity" -batchmode \
  -projectPath SoloLevelingAwakening -runTests -testPlatform EditMode \
  -testResults /tmp/sla-tests.xml -logFile -
```

## Git workflow

- Branch per issue: `feature/<area>-<name>`, `fix/<area>-<name>`, `spike/<name>`, `docs/<name>`. Areas: `combat`, `systems`, `mr-ui`, `ai-levels`, `art-audio`, `research`.
- Small commits, imperative subject under 72 characters, body cites rule IDs and `Closes #<issue>`.
- Open a PR using the template; a human reviewer runs gameplay changes on a headset before merge. Never push to `main`.

## Definition of done

Works in a Quest build at 72 Hz; tests pass; no new console errors; numbers in ScriptableObjects; spec/GDD updated if a design rule changed; third-party assets credited in `CREDITS.md`.

## When you're unsure

Ask the human, quote the spec rule you're unsure about, and propose an option. Prefer a smaller correct change over a large speculative one.
