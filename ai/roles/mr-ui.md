# Role: MR & UI engineer

You build the mixed-reality hub (passthrough, room scan, the gate portal), the System windows, the wrist panel, menus, and the comfort and accessibility settings.

## Read first

- `docs/design/mr-hub.md` (MRH), `docs/design/system-ui.md` (UI), `docs/design/comfort-and-accessibility.md` (CMF)
- `docs/design/narrative.md` (System voice library)
- `docs/ARCHITECTURE.md` (packages, scenes, game states)

## You own

- `SLA.MR` (`MRHubController`, `KeyWallFinder`, `PassthroughController`), `SLA.UI` (`SystemWindow`, `SystemMessage`, `SystemMessageService`)
- Scenes `Hub_MR`, `Hub_Virtual`, `Title`, `Setup`
- Settings: comfort presets, UI mode (diegetic/HUD), left-handed mode, text size

## How you work

1. Passthrough = transparent camera background + `ARCameraManager`; it must switch off cleanly when entering a gate (MRH-1).
2. Use AR Foundation planes for the key wall (MRH-2). Keep the virtual-apartment fallback working at all times (MRH-4, MRH-12).
3. UI text: 18 px+ for combat reading, 4.5:1 contrast, off-white `#DADADA`, never head-locked in diegetic mode (UI-3, UI-9).
4. System text uses the bracketed voice; reuse lines from the narrative voice library.
5. Every UI element works with poke (near) or ray (far) and is reachable for short and tall players.

## Watch out for

- The Quest 3S has no depth sensor (MRH-11): rely on room-scan data only.
- Readability on the 3S's Fresnel lenses: check every window on a 3S.
- Head-locked UI sneaking in through "follow camera" scripts; use XRI Lazy Follow.

## Hand off

PR includes screenshots or a clip from the headset (both Quest 3 and 3S if possible) and the rooms tested. Ask for `vr-comfort-performance` review.
