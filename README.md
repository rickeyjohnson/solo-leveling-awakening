# Solo Leveling: Awakening

> From the Weakest Hunter to the Strongest — Experience the Rise.

A first-person VR and mixed-reality action RPG for **Meta Quest 3 / 3S**, built in **Unity 6** as a two-semester senior project (Oct 2026 – Apr 2027).

The System wakes up in your real room. A gate tears open on your actual wall. You step through, fight with your own arms, train with real squats and punches, and rise from E-rank to S-rank.

![Core loop](docs/images/core-loop.png)

## Team

| Member | Role |
| --- | --- |
| Ciara McLaughlin | TBD (picked in week 1) |
| Elijah Lewis | TBD |
| Rickey Johnson | TBD |
| Samaii Hartley | TBD |
| Tiara McLaughlin | TBD |

Roles and rituals: [docs/TEAM.md](docs/TEAM.md).

## Status

**Phase 1 of 7: Learn and set up** (Oct 12 – Nov 6, 2026). The Unity project does not exist yet; it is created in week 1 following [docs/ONBOARDING.md](docs/ONBOARDING.md#5-create-the-unity-project-one-person-week-1). Track progress on the [milestones](https://github.com/rickeyjohnson/solo-leveling-awakening/milestones).

## Docs

| Doc | What it covers |
| --- | --- |
| [Game Design Document](docs/GDD.md) | Everything about the game: vision, systems, levels, enemies, UI, art, audio, plan. Export of the live team doc |
| [Onboarding](docs/ONBOARDING.md) | Accounts, installs, headset setup, creating the Unity project, the 4-week learning path |
| [Contributing](CONTRIBUTING.md) | Branches, commits, pull requests, scene ownership, Git LFS |
| [Architecture](docs/ARCHITECTURE.md) | Packages, code layers, folders, scenes, game states, events, save data |
| [Coding standards](docs/CODING_STANDARDS.md) | C# and Unity conventions |
| [Art & audio pipeline](docs/ART_PIPELINE.md) | Blender, Mixamo, Substance Painter, toon look, budgets, naming, audio |
| [Level design](docs/LEVEL_DESIGN.md) | Gate template, greyboxing, VR level rules, puzzles and traps |
| [Testing](docs/TESTING.md) | Acceptance targets, playtests, the user study, bug severity |
| [Roadmap](docs/ROADMAP.md) | Phases, dates, milestones, scope |
| [Decisions](docs/DECISIONS.md) | Why we chose what we chose |
| [Credits](CREDITS.md) | Third-party assets and their licenses |

## Tech stack

Unity 6.3 LTS · Universal Render Pipeline · OpenXR + Unity OpenXR: Meta · XR Interaction Toolkit 3 · AR Foundation · AI Navigation · Behavior · ProBuilder. Art in Blender, Mixamo and Substance 3D Painter. All free for students; budget is $0.

## Repository layout

```text
solo-leveling-awakening/
├── SoloLevelingAwakening/   Unity project (created in week 1)
├── docs/                    design and process docs, images
├── tools/                   setup scripts
├── .github/                 issue and pull request templates
├── CONTRIBUTING.md
├── CREDITS.md
└── README.md
```

## Quick start

```bash
git clone https://github.com/rickeyjohnson/solo-leveling-awakening.git
cd solo-leveling-awakening
bash tools/setup-git.sh
```

Then follow [docs/ONBOARDING.md](docs/ONBOARDING.md).

## Disclaimer

Unofficial, non-commercial fan project made for a university course. Not affiliated with or endorsed by D&C Media, Kakao Entertainment, Chugong, REDICE Studio, Netmarble or Aniplex. *Solo Leveling* and its characters belong to their owners. This repository is private and the game is not for sale or public distribution. See the Legal & IP rules in the [GDD](docs/GDD.md#risks-legal--ip).
