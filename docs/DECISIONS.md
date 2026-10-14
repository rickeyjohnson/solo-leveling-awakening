# Decisions

Why the project is the way it is. Newest first. Add a row (or a short section for big ones) whenever the team settles something that crosses workstreams or changes the GDD.

| Date | Decision | Why | Revisit if |
| --- | --- | --- | --- |
| Oct 14, 2026 | Title is **Solo Leveling: Awakening** | From the midterm presentation | — |
| Oct 14, 2026 | Full **E → S** rank progression on the main path; level cap 50 | Matches the midterm deck's progression and dungeon slides | Phase 5 slips: B–S gates drop to 2 rooms each |
| Oct 14, 2026 | Private GitHub repo with docs; the Unity project is created by the team in week 1 | Everyone learns the setup; avoids version mismatch | — |
| Oct 14, 2026 | Substance 3D Painter added to the toolset | Free student license; hand-painted detail | License terms change |
| Oct 9, 2026 | **Unity 6** with URP, OpenXR + Unity OpenXR: Meta, XR Interaction Toolkit 3, AR Foundation | Most documentation and tutorials for a first-time team; Unity and Meta both point new projects to the OpenXR plugin | — |
| Oct 9, 2026 | Not using Meta's `OVRCameraRig` / MR Utility Kit | MRUK needs Meta's own rig; mixing it with XRI's XR Origin is a common beginner trap | Week-3 spike shows AR Foundation wall planes are unreliable → switch the hub to MRUK |
| Oct 9, 2026 | Meta Quest 3 / 3S standalone, 72 Hz in gates | The team's hardware; one build covers both | — |
| Oct 9, 2026 | $0 budget: free tools and assets only | Senior project, no funding | Funding appears |
| Oct 9, 2026 | Direct fan game, non-commercial, never published under the IP's name | Course project; see the GDD's Legal & IP rules | Anyone wants to publish → rename first |
| Oct 9, 2026 | Single-player only | Fits the "solo" fantasy; no networking code | — |
| Oct 9, 2026 | Physics melee (Velocity Tracking) with animated enemies and death-only ragdolls | Most of the *Blade & Sorcery* feel for a fraction of the engineering | — |
| Oct 9, 2026 | Smooth locomotion + room-scale; teleport as the "Comfortable" preset (proposed) | Meta's comfort guidance; study participants new to VR | Team decides against teleport |
| Oct 9, 2026 | Daily quest = squats + punches, once per play session | The two movements the headset tracks reliably; canon push-ups and sit-ups are unsafe in a headset | — |
| Oct 9, 2026 | Shadow Extraction ("Arise") is a stretch goal, gesture-only | Big feature; the core game stands without it | Phase 5 finishes early |
| Oct 9, 2026 | Original story; the player is themselves; canon-era Seoul with Association and guild cameos | Avoids retelling Jinwoo's story; first person suits VR | — |
| Oct 9, 2026 | Hub is the player's real room in mixed reality; gates open on the wall | The project's signature moment, and true to canon's "gates in the real world" | No room scan → virtual apartment fallback |
| Oct 9, 2026 | Death respawns at the gate entrance; boss shortcut after the antechamber | Simple, fair, cheap to build | — |
| Oct 9, 2026 | Standing play required | Combat and the daily quest need it | — |

## Template for a big decision

```markdown
### <Title> (<date>)

**Context:** what forced the decision.
**Options:** A, B, C, one line each.
**Decision:** what we chose.
**Why:** the deciding reasons.
**Consequences:** what this makes easier and harder; what to watch.
```
