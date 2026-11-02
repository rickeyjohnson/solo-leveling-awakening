# Prefabs

Player, enemies, weapons, UI, gates and props. Build features as prefabs and test them in your Sandbox scene.

## Player

`Player/PlayerRig.prefab` is the one player rig: Bootstrap and every sandbox use it. It is generated from the XRI Starter Assets rig by **Tools → SLA → Set Up Player Rig** (`PlayerRigSetup`), which also places it in those scenes. Don't edit or duplicate it by hand; change `PlayerRigSetup` instead. To test a feature, open your `Sandbox_<Name>` scene: it already has the rig, a NavMesh to teleport on, and a debug panel to switch comfort presets and left-handed mode. See docs/ARCHITECTURE.md#player-rig.
