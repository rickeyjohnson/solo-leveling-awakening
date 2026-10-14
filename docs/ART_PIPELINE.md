# Art & audio pipeline

How a model, texture or sound gets from Blender, Mixamo, Substance Painter or a microphone into the game, and stays inside the Quest's budget. Look and feel are defined in the GDD's [Art direction](GDD.md#art-direction) and [Audio](GDD.md#audio) sections.

## The look in one paragraph

Stylized anime **cel-shading**: two or three flat light bands, strong rim light, glowing System blue against dark dungeons. One toon shader (Shader Graph) on everything, outlines only on enemies, weapons and the player's hands. Baked lighting. Few textures; one atlas per gate.

| Use | Color | Hex |
| --- | --- | --- |
| System windows, mana | Cyan blue | `#3FB8FF` |
| Window background | Deep navy, 80% opacity | `#0B1A2E` |
| Rewards, level-up | Gold | `#FFC94A` |
| Danger, Red Gate | Crimson | `#E5383B` |
| Shadows, Arise | Violet | `#7B3FE4` |
| Off-white text | Light gray | `#DADADA` |

## Budgets (per asset)

| Asset | Triangles | Textures | Bones |
| --- | --- | --- | --- |
| Boss | 15,000 | 1 × 2048 atlas | 60 |
| Regular enemy | 5,000 | Shared 2048 atlas per gate | 40 |
| Player hands + gloves | 4,000 per hand | 1 × 1024 | Hand rig |
| Weapon | 1,500 | Shared weapons atlas | — |
| Prop | 50–800 | Gate atlas | — |
| Whole gate on screen | Under 750,000 | | |

Over budget? Ask the tech artist before committing.

## Naming

`<Prefix>_<Name>_<Variant>`, PascalCase, no spaces.

| Prefix | Type | Example |
| --- | --- | --- |
| `SM_` | Static mesh | `SM_Temple_Pillar_A` |
| `SK_` | Skinned mesh | `SK_Goblin` |
| `A_` | Animation clip | `A_Goblin_Attack_Jab` |
| `T_` | Texture (suffix `_D` color, `_N` normal, `_E` emissive) | `T_GoblinWarren_Atlas_D` |
| `M_` | Material | `M_Toon_Goblin` |
| `VFX_` | Particle system | `VFX_HitSpark_Mana` |
| `SFX_` | Sound effect | `SFX_Impact_Flesh_01` |
| `MUS_` | Music | `MUS_Boss_IronCommander` |
| `VO_` | Voice line | `VO_WooJinchul_Call01` |
| `UI_` | UI sprite | `UI_Icon_Strength` |

## Blender → Unity

1. **Scale:** 1 Blender unit = 1 m. A goblin is about 1.2 m tall; the player's dagger about 0.35 m.
2. **Apply transforms** (Ctrl+A → All Transforms) before export. Origin at the feet for characters, at the grip for weapons.
3. **Export FBX** with: Apply Scalings = FBX All · Forward = −Z · Up = Y · Apply Transform ✓ · Mesh, Armature only.
4. **Save the `.blend` in the shared drive** (`ArtSource/`, not the repo). Commit only the FBX into `Assets/_Project/Art/...`.
5. In Unity: Model import → Read/Write off · Mesh compression Medium · Rig: Humanoid for anything Mixamo-animated.

## Mixamo characters and animations

1. Upload a T-pose character (or pick a Mixamo character) and auto-rig.
2. Download the character **with skin**, FBX for Unity.
3. Download each animation **without skin**, FBX for Unity, 30 fps.
4. Import as **Humanoid**; point every animation at the character's avatar.
5. Mixamo files are royalty-free in our game, but never redistribute the raw FBX outside it.

## Substance 3D Painter

- Use the **Unity URP** export preset, then pack into the gate's atlas.
- Hand-paint wear, scratches and glow, then **flatten** toward 2–3 tones so the toon shader reads cleanly.
- Keep the `.spp` project in the shared drive; commit only exported textures.
- Licenses: each artist uses their own free student license, on one personal computer.

## Textures in Unity

| Setting | Value |
| --- | --- |
| Max size | 2048 atlases, 1024 for small sets, 512 for UI icons |
| Compression | ASTC 6×6 (ASTC 4×4 for UI and the hands) |
| Mipmaps | On for world textures, off for UI |
| sRGB | On for color, off for masks and normals |

## VFX

Built with the **Particle System** (Shuriken); VFX Graph is too heavy for the Quest. Max 200 live particles per effect, additive or alpha-blended unlit materials, no real-time lights on particles.

## Audio

| Kind | Format | Import settings |
| --- | --- | --- |
| 3D sound effects | WAV source, mono, 48 kHz | ADPCM, Decompress on Load, 3D spatialized |
| UI and System chime | WAV source, stereo | ADPCM, 2D |
| Music | WAV source, stereo | Vorbis, quality 70, Streaming |
| Voice lines | WAV source, mono | Vorbis, quality 60, Compressed in Memory |

- Target loudness around −16 LUFS for music, peaks under −1 dBFS.
- Mixer groups: Master → Music, SFX, System/UI, Voice, Ambience. Music and SFX duck 6 dB under System messages and voice.
- **Never use anime or official game audio.** Record or source our own (Freesound CC0/CC-BY, Sonniss GDC bundles, Kenney).

## Credits

Every third-party asset (model, texture, sound, font, music) gets a row in [CREDITS.md](../CREDITS.md) **in the same PR that adds it**: name, author, source link, license.
