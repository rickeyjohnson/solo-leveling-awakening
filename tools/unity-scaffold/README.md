# Unity scaffold

The starting code for the Unity project, kept here until the project is created ([issue #1](https://github.com/rickeyjohnson/solo-leveling-awakening/issues/1)). It's plain C# and assembly definitions with no `.meta` files: Unity generates those on import.

## What's in it

| Folder | Contents |
| --- | --- |
| `Assets/_Project/Scripts/Core` | Event channels, `GameState` + `GameStateManager`, `SceneLoader`, `SceneNames`, `SaveSystem` + `SaveData`, `GameBootstrapper` |
| `Assets/_Project/Scripts/Data` | ScriptableObject definitions: `CombatConfig`, `ProgressionConfig`, `WeaponData`, `EnemyData`, `SkillData`, `QuestData`, `GateData`, `ItemData` |
| `Assets/_Project/Scripts/Gameplay` | `DamageCalculator`, `SwingTracker`, `WeaponHitDetector`, `Health`, `HitZoneMarker`; `SquatCounter` + driver; `PlayerStats`, `XpCurve`; `EnemyBrain`, `EnemyDirector`, `AttackTokenPool`; `GatePortal`, `PlayerSpawn` |
| `Assets/_Project/Scripts/MR` | `PassthroughController`, `KeyWallFinder`, `MRHubController` |
| `Assets/_Project/Scripts/UI` | `SystemMessage`, `SystemWindow`, `SystemMessageService` |
| `Assets/_Project/Scripts/Editor` | `PackageInstaller`, `ProjectConfigurator`, `SceneScaffolder` (menu **Tools → SLA**) |
| `Assets/_Project/Tests/EditMode` | Tests for damage, progression, saves, squats and attack tokens |
| `Assets/ThirdParty` | Empty, for imported assets |

Assemblies: `SLA.Core`, `SLA.Data`, `SLA.Gameplay`, `SLA.MR`, `SLA.UI`, `SLA.Editor`, `SLA.Bootstrap.Editor`, `SLA.Tests.EditMode`. Layout and rules: [ARCHITECTURE.md](../../docs/ARCHITECTURE.md).

## How to use it

Follow the sub-issues of [#1](https://github.com/rickeyjohnson/solo-leveling-awakening/issues/1). In short:

1. Create the empty Universal 3D project at `SoloLevelingAwakening/` with Unity 6000.3.26f1.
2. Copy `tools/unity-scaffold/Assets/*` into `SoloLevelingAwakening/Assets/`.
3. Run **Tools → SLA** → install packages, configure project settings, then build the starting scenes.
4. Run the EditMode tests and `bash tools/check-repo.sh`, then commit the project **with** the `.meta` files Unity created.
5. Delete `tools/unity-scaffold/` in the same PR, so there's only one copy of the code.
