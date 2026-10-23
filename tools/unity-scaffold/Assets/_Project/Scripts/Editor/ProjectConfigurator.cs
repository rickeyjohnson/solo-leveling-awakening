using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager.UI;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using SLA.Core.Events;
using SLA.Data;

namespace SLA.EditorTools
{
    /// <summary>
    /// One-time project setup, re-runnable (it skips anything that already exists).
    /// Headless, with -buildTarget Android:
    ///   -executeMethod SLA.EditorTools.ProjectConfigurator.ConfigureSettings
    ///   -executeMethod SLA.EditorTools.SceneScaffolder.BuildScenes   (second run, after the samples compile)
    /// Also available from the menu: Tools > SLA.
    /// </summary>
    public static class ProjectConfigurator
    {
        public const string Root = "Assets/_Project";
        public const string SettingsDir = Root + "/Settings";
        public const string DataDir = Root + "/Data";
        public const string ScenesDir = Root + "/Scenes";
        public const string MaterialsDir = Root + "/Art/Materials";

        static readonly string[] Layers =
        {
            "Player", "PlayerHands", "PlayerWeapon", "Enemy", "EnemyWeapon", "Environment", "RoomScan",
        };

        // OpenXR features for Quest 3 / 3S (matched by class name, so this file needs no Meta package reference).
        static readonly string[] OpenXrFeatures =
        {
            "MetaQuestFeature",
            "OculusTouchControllerProfile",
            "MetaQuestTouchPlusControllerProfile",
            "HandTracking",
            "ARSessionFeature",
            "ARCameraFeature",
            "ARPlaneFeature",
            "ARBoundingBoxFeature",
            "ARMeshFeature",
            "ARAnchorFeature",
            "ARRaycastFeature",
        };

        [MenuItem("Tools/SLA/Configure Project Settings")]
        public static void ConfigureSettingsMenu() => Run(false);

        public static void ConfigureSettings() => Run(true);

        static void Run(bool exitWhenDone)
        {
            try
            {
                EnsureFolders();
                ConfigurePlayer();
                ConfigureUrp();
                ConfigureXr();
                ConfigureLayers();
                CreateDataAssets();
                ImportXriStarterAssets();
                AssetDatabase.SaveAssets();
                Debug.Log("[ProjectConfigurator] Settings configured.");
                if (exitWhenDone) EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (exitWhenDone) EditorApplication.Exit(1);
            }
        }

        // ------------------------------------------------------------------ folders

        public static void EnsureFolders()
        {
            foreach (var path in new[]
                     {
                         SettingsDir, DataDir + "/Events", DataDir + "/Weapons", DataDir + "/Enemies",
                         DataDir + "/Gates", DataDir + "/Skills", DataDir + "/Quests", ScenesDir + "/Gates",
                         ScenesDir + "/Sandbox", MaterialsDir,
                     })
            {
                EnsureFolder(path);
            }
        }

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        // ------------------------------------------------------------------ player

        static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "SLA Team";
            PlayerSettings.productName = "Solo Leveling: Awakening";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.slateam.sololevelingawakening");
            PlayerSettings.colorSpace = ColorSpace.Linear;

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            // Active Input Handling = Input System Package (new). XRI requires it.
            var projectSettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset").FirstOrDefault();
            if (projectSettings != null)
            {
                var so = new SerializedObject(projectSettings);
                var input = so.FindProperty("activeInputHandler");
                if (input != null)
                {
                    input.intValue = 1;
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        // ------------------------------------------------------------------ URP

        static void ConfigureUrp()
        {
            string assetPath = SettingsDir + "/URP_Quest.asset";
            var urp = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(assetPath);
            if (urp == null)
            {
                var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, SettingsDir + "/URP_Quest_Renderer.asset");
                urp = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(urp, assetPath);
            }

            // GDD build settings: MSAA 4x, HDR off, no post-processing, one real-time light.
            urp.msaaSampleCount = 4;
            urp.supportsHDR = false;
            urp.renderScale = 1f;
            urp.shadowDistance = 20f;
            EditorUtility.SetDirty(urp);

            GraphicsSettings.defaultRenderPipeline = urp;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = urp;
            }
            QualitySettings.SetQualityLevel(current, false);
        }

        // ------------------------------------------------------------------ XR

        static void ConfigureXr()
        {
            if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget perTarget) || perTarget == null)
            {
                EnsureFolder("Assets/XR");
                perTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(perTarget, "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, perTarget, true);
            }

            if (!perTarget.HasSettingsForBuildTarget(BuildTargetGroup.Android))
                perTarget.CreateDefaultSettingsForBuildTarget(BuildTargetGroup.Android);
            if (!perTarget.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))
                perTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);

            var general = perTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
            general.InitManagerOnStart = true;
            var manager = perTarget.ManagerSettingsForBuildTarget(BuildTargetGroup.Android);
            XRPackageMetadataStore.AssignLoader(manager, "UnityEngine.XR.OpenXR.OpenXRLoader", BuildTargetGroup.Android);
            EditorUtility.SetDirty(perTarget);

            FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);
            var openXr = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            if (openXr == null)
            {
                Debug.LogWarning("[ProjectConfigurator] No OpenXR settings for Android yet; open Project Settings > XR Plug-in Management once, then re-run.");
                return;
            }

            var metaSet = OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, "com.unity.openxr.featureset.meta");
            if (metaSet != null)
            {
                metaSet.isEnabled = true;
                OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(BuildTargetGroup.Android);
            }

            foreach (var feature in openXr.GetFeatures())
            {
                if (OpenXrFeatures.Contains(feature.GetType().Name))
                {
                    feature.enabled = true;
                    EditorUtility.SetDirty(feature);
                }
            }

            openXr.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
            EditorUtility.SetDirty(openXr);

            string enabled = string.Join(", ", openXr.GetFeatures().Where(f => f.enabled).Select(f => f.GetType().Name));
            Debug.Log($"[ProjectConfigurator] OpenXR (Android) features enabled: {enabled}");
        }

        // ------------------------------------------------------------------ layers

        static void ConfigureLayers()
        {
            var tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset").FirstOrDefault();
            if (tagManagerAsset == null) return;

            var tagManager = new SerializedObject(tagManagerAsset);
            var layers = tagManager.FindProperty("layers");
            foreach (string name in Layers)
            {
                bool exists = false;
                for (int i = 0; i < layers.arraySize; i++)
                {
                    if (layers.GetArrayElementAtIndex(i).stringValue == name) { exists = true; break; }
                }
                if (exists) continue;

                for (int i = 8; i < layers.arraySize; i++)
                {
                    var slot = layers.GetArrayElementAtIndex(i);
                    if (string.IsNullOrEmpty(slot.stringValue))
                    {
                        slot.stringValue = name;
                        break;
                    }
                }
            }
            tagManager.ApplyModifiedPropertiesWithoutUndo();
        }

        // ------------------------------------------------------------------ data

        public static T CreateOrLoad<T>(string path, Action<T> init = null) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;
            var asset = ScriptableObject.CreateInstance<T>();
            init?.Invoke(asset);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        static void CreateDataAssets()
        {
            CreateOrLoad<ProgressionConfig>(SettingsDir + "/ProgressionConfig.asset");
            CreateOrLoad<CombatConfig>(SettingsDir + "/CombatConfig.asset");

            string ev = DataDir + "/Events/";
            CreateOrLoad<GameStateEventChannel>(ev + "OnGameStateChanged.asset");
            CreateOrLoad<IntEventChannel>(ev + "OnLevelUp.asset");
            CreateOrLoad<IntEventChannel>(ev + "OnRankUp.asset");
            CreateOrLoad<IntEventChannel>(ev + "OnRepCounted.asset");
            CreateOrLoad<StringEventChannel>(ev + "OnEnemyKilled.asset");
            CreateOrLoad<StringEventChannel>(ev + "OnQuestUpdated.asset");
            CreateOrLoad<StringEventChannel>(ev + "OnQuestCompleted.asset");
            CreateOrLoad<StringEventChannel>(ev + "OnSkillUsed.asset");
            CreateOrLoad<FloatEventChannel>(ev + "OnDamageTaken.asset");
            CreateOrLoad<VoidEventChannel>(ev + "OnGateCleared.asset");

            string w = DataDir + "/Weapons/";
            CreateOrLoad<WeaponData>(w + "RustyDagger.asset", d => { d.id = "rusty_dagger"; d.displayName = "Rusty Dagger"; d.rank = Rank.E; d.baseDamage = 10f; });
            CreateOrLoad<WeaponData>(w + "SteelDagger.asset", d => { d.id = "steel_dagger"; d.displayName = "Steel Dagger"; d.rank = Rank.E; d.baseDamage = 12f; d.shopPrice = 150; });
            CreateOrLoad<WeaponData>(w + "KasakasVenomFang.asset", d => { d.id = "kasaka_venom_fang"; d.displayName = "Kasaka's Venom Fang"; d.rank = Rank.C; d.baseDamage = 22f; d.strengthRequirement = 15; d.special = "Poison: 4 dmg/s for 4 s"; });

            string e = DataDir + "/Enemies/";
            var goblin = CreateOrLoad<EnemyData>(e + "Goblin.asset", d => { d.id = "goblin"; d.displayName = "Goblin"; d.maxHp = 40f; d.damage = 8f; d.baseXp = 20; });
            var kobold = CreateOrLoad<EnemyData>(e + "Kobold.asset", d => { d.id = "kobold"; d.displayName = "Kobold"; d.maxHp = 25f; d.damage = 5f; d.moveSpeed = 3.5f; d.windupSeconds = 0.6f; d.baseXp = 12; d.behaviorNotes = "Very fast; swarms in groups of 4-6; flees at low HP."; });
            var chieftain = CreateOrLoad<EnemyData>(e + "GoblinChieftain.asset", d => { d.id = "goblin_chieftain"; d.displayName = "Goblin Chieftain"; d.maxHp = 400f; d.damage = 15f; d.windupSeconds = 1f; d.baseXp = 300; d.goldRange = new Vector2Int(60, 90); d.behaviorNotes = "Phase 2 at 50% HP: calls goblin archers to the ledges."; });

            CreateOrLoad<GateData>(DataDir + "/Gates/GoblinWarren.asset", d =>
            {
                d.id = "goblin_warren"; d.displayName = "Goblin Warren"; d.rank = Rank.E;
                d.recommendedLevels = new Vector2Int(1, 9); d.sceneName = "Gate_GoblinWarren";
                d.enemies = new[] { goblin, kobold }; d.boss = chieftain; d.combatRooms = 2;
                d.objective = "Defeat the Goblin Chieftain.";
            });

            CreateOrLoad<SkillData>(DataDir + "/Skills/Dash.asset", d =>
            {
                d.id = "dash"; d.displayName = "Dash"; d.fatigueCost = 15f; d.cooldownSeconds = 4f;
                d.gesture = "Hold grip and lunge (0.25 m in under 0.2 s); left stick + grip as fallback.";
                d.effect = "3 m blink with a comfort vignette.";
            });

            CreateOrLoad<QuestData>(DataDir + "/Quests/DailyPreparation.asset", d =>
            {
                d.id = "daily_preparation"; d.title = "Daily Quest: Preparation to Become Strong"; d.kind = QuestKind.Daily;
                d.objective = "Squats and punches, scaled to the player's level (GDD: Quest system)."; d.rewardStatPoints = 3;
            });
        }

        // ------------------------------------------------------------------ samples

        static void ImportXriStarterAssets()
        {
            var starter = Sample.FindByPackage("com.unity.xr.interaction.toolkit", null)
                .FirstOrDefault(s => s.displayName == "Starter Assets");
            if (string.IsNullOrEmpty(starter.displayName))
            {
                Debug.LogWarning("[ProjectConfigurator] XRI Starter Assets sample not found.");
                return;
            }
            if (!starter.isImported)
            {
                starter.Import(Sample.ImportOptions.OverridePreviousImports | Sample.ImportOptions.HideImportWindow);
                Debug.Log("[ProjectConfigurator] Imported XRI Starter Assets.");
            }
        }
    }
}
