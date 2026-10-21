# Copilot instructions

Read [AGENTS.md](../AGENTS.md) first; it is the shared context for every AI tool on this project. The short version:

- **Solo Leveling: Awakening** is a VR / mixed-reality action RPG for Meta Quest 3/3S in Unity 6.3 LTS (URP, OpenXR + Meta, XR Interaction Toolkit 3, AR Foundation).
- Implement from the numbered rules in `docs/design/` (e.g. `CMB-9`) and cite them. Never invent tuning numbers.
- Comfort is a hard rule: never move, shake, freeze or push the camera; no acceleration curves.
- Quest budget: 72 Hz, under 150 draw calls, under 750K triangles, at most 6 physics-active enemies, no per-frame allocations.
- Tunable numbers live in ScriptableObjects; systems talk through ScriptableObject event channels.
- Pure logic is plain C# with NUnit EditMode tests named after the rule.
- Never write or edit `.unity`, `.prefab`, `.asset` or `.meta` YAML; describe the Editor steps instead.
- No official Solo Leveling art, music, voice clips or logos.

## When reviewing pull requests

Use the checklists in `ai/reviews/`: [unity-code-review.md](../ai/reviews/unity-code-review.md) for C#, [vr-comfort-performance.md](../ai/reviews/vr-comfort-performance.md) for camera, movement, feedback, VFX and scenes, and [gdd-consistency.md](../ai/reviews/gdd-consistency.md) for docs and ScriptableObject defaults. Flag comfort problems as `S1`.

Agents for each workstream are in `.github/agents/`; prompts for reviews and workflows are in `.github/prompts/`. See [ai/README.md](../ai/README.md).
