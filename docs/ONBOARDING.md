# Onboarding

Everything a new teammate needs to go from nothing to running a build on a Quest. Expect about **2–3 hours** for steps 1–4 (mostly downloads).

## 1. Accounts

| Account | Why | Notes |
| --- | --- | --- |
| GitHub | The repo | Ask the producer to add you as a collaborator |
| Unity ID | Unity Hub and the Personal license | Free under $200K revenue |
| Meta account + developer organization | Developer Mode on the headset | Create or join the team's organization at developers.meta.com. Meta may ask you to verify the account (phone number or card) |
| Adobe Substance 3D (artists only) | Texturing in Substance 3D Painter | Free 12-month student license; apply with proof of enrollment; one personal computer per license |
| Mixamo (artists, animators) | Free rigged characters and animations | Sign in with an Adobe ID |

## 2. Install the tools

| Tool | Version | Notes |
| --- | --- | --- |
| Git | Latest | macOS: `brew install git` · Windows: Git for Windows |
| Git LFS | Latest | macOS: `brew install git-lfs` · Windows: included in Git for Windows (tick "Git LFS") |
| Unity Hub | Latest | unity.com/download |
| Unity Editor | **6000.3.27f1** (6.3 LTS): everyone uses this exact version | Install link below |
| Unity modules | **Android Build Support** with **OpenJDK** and **Android SDK & NDK Tools** | Needed for Quest builds. Add in Unity Hub → Installs → ⚙ → Add modules |
| IDE | JetBrains Rider (free for students) or Visual Studio Code with the C# Dev Kit and Unity extensions | |
| Meta Quest Developer Hub (MQDH) | Latest | Installs builds, shows headset logs, casts the view. macOS and Windows |
| Blender (artists) | 4.x | blender.org |

**Exact Unity version: `6000.3.27f1`.** With Unity Hub installed, run `open "unityhub://6000.3.27f1/672d562a42a7"` (macOS) or open that link in a browser (Windows) to jump straight to the install screen, then tick the Android modules above. Do not upgrade Unity without a team decision.

## 3. Clone the repo

```bash
git clone https://github.com/rickeyjohnson/solo-leveling-awakening.git
cd solo-leveling-awakening
bash tools/setup-git.sh
```

`setup-git.sh` turns on Git LFS for the repo and registers Unity's Smart Merge for scenes and prefabs. Run it again after you install a new Unity version.

## 4. Set up your headset (Quest 3 / 3S)

1. In the **Meta Horizon** phone app: Menu → Devices → your headset → Headset settings → turn on **Developer Mode**. (Your Meta account must belong to a developer organization.)
2. Connect the headset to your computer with a USB-C cable and accept **Allow USB debugging** inside the headset.
3. Open **MQDH**; the headset should show as connected.
4. Run **Space Setup** on the headset in the room you'll test in, so the hub can find your walls.

### Testing in the editor

| Your computer | How to play in the editor | Fallback |
| --- | --- | --- |
| Windows PC | **Meta Horizon Link** (cable or Air Link): press Play in Unity and play in the headset | Build to the headset |
| Apple Silicon Mac | **Meta XR Simulator** (needs Unity OpenXR Plugin 1.13+); Link does not run on macOS | Build to the headset |
| Intel Mac | No simulator | Build to the headset |

Plan for at least one Windows PC on the team: Link is the fastest way to iterate.

## 5. Create the Unity project (one person, week 1)

**Already done** (Oct 2026, issue #1): just pull `main` and open `SoloLevelingAwakening/` in Unity Hub. Kept here for reference, in case the project ever has to be rebuilt:

1. Create an empty project at `SoloLevelingAwakening/` with 6000.3.27f1 (`Unity -batchmode -quit -createProject <repo>/SoloLevelingAwakening`).
2. Copy in only `Assets/_Project/Scripts/Editor/Bootstrap/`, then run `-batchmode -buildTarget Android -executeMethod SLA.EditorTools.Bootstrap.PackageInstaller.Install` (no `-quit`). It installs every package in [ARCHITECTURE](ARCHITECTURE.md#packages).
3. Copy in the rest of `Assets/_Project/`, then run **Tools → SLA → Configure Project Settings** (`SLA.EditorTools.ProjectConfigurator.ConfigureSettings`): URP for Quest, OpenXR + Meta features, the [build settings](ARCHITECTURE.md#build-settings-quest), layers and starter data assets.
4. Run **Tools → SLA → Rebuild Starting Scenes** (`SLA.EditorTools.SceneScaffolder.BuildScenes`) once. It overwrites scenes.
5. Run the EditMode tests and `bash tools/check-repo.sh`, then commit everything **with** its `.meta` files.

## 6. Your first build

1. Open `SoloLevelingAwakening/` in Unity Hub (Add → Add project from disk).
2. File → Build Profiles → **Android** → Switch Platform (first time takes a while).
3. Plug in the headset → **Build and Run**.
4. Put the headset on. If nothing launches, check MQDH → Apps, and the headset's Unknown Sources library.

## 7. Learning path (weeks 1–4)

The team starts with no VR or Unity experience, so the first four weeks are mostly structured learning.

| Week | Everyone | Then by role |
| --- | --- | --- |
| 1 (Oct 12) | Unity Learn: **Unity Essentials** pathway; this repo's docs; set up accounts and installs | Producer: create the Unity project |
| 2 (Oct 19) | Unity Learn: **VR Development** pathway (XR Interaction Toolkit basics) | Read your area's section of the GDD |
| 3 (Oct 26) | One **spike** per person (time-boxed, on a `spike/` branch) | Combat: weapon grab with Velocity Tracking · MR & UI: wall-plane detection for the gate wall · Systems: squat counter from headset height · AI & levels: NavMesh enemy that chases you · Art: Mixamo character with a toon material on the Quest |
| 4 (Nov 2) | Demo each spike **on a headset** to the team | Decide what to keep; write findings in [DECISIONS](DECISIONS.md) |

## 8. Where to ask

- Design questions: comment on the live GDD, or open a `type: task` issue.
- Tech questions: the team chat, then a GitHub Discussion if it's worth keeping.
- Blocked for more than a day: tell the producer at the next check-in.

## 9. AI assistants (optional)

The repo is set up for **Claude Code**, **Gemini CLI**, **GitHub Copilot** and **ChatGPT**. They all read the same context ([AGENTS.md](../AGENTS.md)) and the same playbooks in [`ai/`](../ai/README.md), so you get the same rules whichever one you use.

| Tool | Setup |
| --- | --- |
| Claude Code | Run `claude` in the repo. It loads `CLAUDE.md`, the agents in `.claude/agents/` and the `/` commands in `.claude/commands/` |
| Gemini CLI | Run `gemini` in the repo. It loads `GEMINI.md` and the commands in `.gemini/commands/` (`/role:combat`, `/review:unity` ...) |
| Copilot (VS Code or Rider) | Open the repo. Instructions load automatically; pick an agent (e.g. **Combat engineer**) or type `/` for prompts |
| ChatGPT | Make a Project and follow [ai/chat/PROJECT_INSTRUCTIONS.md](../ai/chat/PROJECT_INSTRUCTIONS.md) |

Read the [team rules for using AI](../ai/README.md#team-rules-for-using-ai) before you start.
