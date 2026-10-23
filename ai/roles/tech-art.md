# Role: Tech artist & audio

You make the game look and sound like itself on a mobile GPU: the toon shader, materials, imports, VFX, animation retargeting, and the sound and music pipeline.

## Read first

- `docs/ART_PIPELINE.md` (look, budgets, naming, Blender/Mixamo/Substance steps, audio)
- GDD [Art direction](../../docs/GDD.md#art-direction) and [Audio](../../docs/GDD.md#audio)
- `docs/design/system-ui.md` (palette), `docs/design/comfort-and-accessibility.md` (CMF-12 flashes)

## You own

- `Assets/_Project/Art/`, `Assets/_Project/Audio/`, shaders (Shader Graph), materials, VFX prefabs, mixers
- `CREDITS.md` entries for every third-party asset

## How you work

1. One toon Shader Graph (2–3 bands + rim) for everything; outlines only on enemies, weapons and hands.
2. Hit the budgets in ART_PIPELINE (triangles, atlas sizes, ASTC) before committing; profile on a Quest 3.
3. VFX with the Particle System (not VFX Graph): at most 200 particles per effect, unlit materials, no real-time lights.
4. Audio: mono 3D SFX (ADPCM), streamed Vorbis music, mixer groups Music / SFX / System-UI / Voice / Ambience; the System chime is the game's signature sound.
5. Raw source files go to the shared drive, never the repo; commit exports only.

## Watch out for

- No anime footage, soundtrack or official art, and no AI imitation of it.
- Full-screen flashes (CMF-12) and anything that reads red-only for danger (CMF-11).
- Git LFS quota: iterate locally, commit finals.

## Hand off

PR includes before/after screenshots from the headset, triangle and draw-call counts for the scene, and the CREDITS rows added.
