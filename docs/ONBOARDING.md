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
| Unity Editor | **6.3 LTS (6000.3.x)**: everyone uses the exact same patch | Pin the exact version in this doc once the project is created |
| Unity modules | **Android Build Support** with **OpenJDK** and **Android SDK & NDK Tools** | Needed for Quest builds. Add in Unity Hub → Installs → ⚙ → Add modules |
| IDE | JetBrains Rider (free for students) or Visual Studio Code with the C# Dev Kit and Unity extensions | |
| Meta Quest Developer Hub (MQDH) | Latest | Installs builds, shows headset logs, casts the view. macOS and Windows |
| Blender (artists) | 4.x | blender.org |

**Exact Unity version:** `6000.3.____` ← the producer fills this in on project creation day. Do not upgrade Unity without a team decision.

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

The producer does this once; everyone else just pulls.

1. Unity Hub → **New project** → editor **6000.3.x** → template **Mixed Reality** (URP, OpenXR, AR Foundation and XR Interaction Toolkit preconfigured).
2. **Location:** the repo folder. **Project name:** `SoloLevelingAwakening`. This creates `solo-leveling-awakening/SoloLevelingAwakening/`, which the `.gitignore` already expects.
3. Edit → Project Settings → **XR Plug-in Management** → Android tab: enable **OpenXR**, then the **Meta Quest** feature group.
4. Package Manager: add **Unity OpenXR: Meta**, **AI Navigation**, **Behavior**, **ProBuilder** (see [ARCHITECTURE](ARCHITECTURE.md#packages)).
5. Apply the build settings in [ARCHITECTURE](ARCHITECTURE.md#build-settings-quest).
6. Create the folder layout in [ARCHITECTURE](ARCHITECTURE.md#folder-layout).
7. Build once to a Quest and confirm the template scene runs.
8. Commit on a branch (`feature/systems-project-setup`), open a PR, merge. Record the exact Unity version in step 2 of this doc.

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
