using System;
using System.IO;
using System.Linq;
using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using SLA.Player;
using SLA.UI;
using Unity.AI.Navigation;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Object = UnityEngine.Object;

namespace SLA.EditorTools
{
    /// <summary>
    /// The player rig (issue #11) and where it goes:
    /// - comfort preset assets (Settings/Comfort), the OnSettingsChanged channel, the Teleport interaction layer;
    /// - Prefabs/Player/PlayerRig.prefab, made from the XRI Starter Assets rig (only when it doesn't exist yet);
    /// - the rig in Bootstrap and every sandbox, replacing Starter rig instances and wired to Bootstrap's systems;
    /// - a NavMesh and NavMesh-only teleport targets in the sandboxes, Hub_Virtual and the Goblin Warren greybox;
    /// - a StickLocomotionLock in Hub_MR (MRH-13), Title and Setup; a comfort debug panel in each sandbox.
    /// Re-runnable: anything already there is left alone, and a scene is saved only when something changed.
    /// Headless: -executeMethod SLA.EditorTools.PlayerRigSetup.ApplyBatch (also runs GameFlowSceneSetup first).
    /// </summary>
    public static class PlayerRigSetup
    {
        public const string RigPrefabPath = ProjectConfigurator.Root + "/Prefabs/Player/PlayerRig.prefab";
        public const string ComfortDir = ProjectConfigurator.SettingsDir + "/Comfort";
        public const string OnSettingsChangedPath = ProjectConfigurator.DataDir + "/Events/OnSettingsChanged.asset";
        public const string FadeMaterialPath = ProjectConfigurator.MaterialsDir + "/M_ScreenFade.mat";

        const string FadeShaderName = "SLA/ScreenFade";
        const string SamplesDir = "Assets/Samples";
        const string SandboxDir = ProjectConfigurator.ScenesDir + "/Sandbox";
        const string WarrenNavMeshNote =
            "NavMesh baked by PlayerRigSetup from the Environment-layer greybox so teleport works (CMF-7). " +
            "Placeholder: the level designer rebakes it with the real layout.";

        // The Starter rig parts we don't use: jump, climbing, grab-move, and eye-gaze (Quest 3/3S have no eye tracking).
        static readonly string[] UnusedRigParts =
        {
            "Locomotion/Jump", "Locomotion/Climb", "Locomotion/Grab Move", "Camera Offset/Gaze Interactor", "Camera Offset/Gaze Stabilized",
        };

        // CMF-3: only enough step to roll over floor seams. Unity's 0.5 m default lifts the view onto anything
        // knee-high in one frame. Rig physics, the same for every preset, so it isn't in ComfortPreset.
        const float StepOffsetMeters = 0.05f;

        // #71: an arc that can't teleport is grey (the Starter's red/yellow hue removed, its alpha kept).
        static readonly Color NoTeleportGrey = new Color32(0x80, 0x80, 0x80, 0xFF);

        [MenuItem("Tools/SLA/Set Up Player Rig")]
        public static void ApplyMenu()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            ApplyAll();
        }

        public static void ApplyBatch()
        {
            try
            {
                ApplyAll();
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }

        static void ApplyAll()
        {
            GameFlowSceneSetup.ApplyToScenes();
            ApplyToScenes();
        }

        /// <summary>Makes the assets and the prefab if missing, then puts the rig and its scene parts in the scenes.</summary>
        public static void ApplyToScenes()
        {
            EnsureAssets();
            if (EnsureRigPrefab() == null)
            {
                throw new InvalidOperationException("No PlayerRig prefab: the XRI Starter Assets sample is missing (Tools > SLA > Configure Project Settings imports it).");
            }

            // Assets are loaded inside each edit, after its scene opens (see GameFlowSceneSetup.Load).
            GameFlowSceneSetup.EditScene(SceneNames.Bootstrap, SetUpBootstrap);
            foreach (string sandbox in FindSandboxScenes())
            {
                GameFlowSceneSetup.EditScene(sandbox, SetUpSandbox);
            }

            GameFlowSceneSetup.EditScene(SceneNames.HubVirtual, () => EnsureNavMeshTeleport("Floor"));
            GameFlowSceneSetup.EditScene($"Gates/{SceneNames.GateGoblinWarren}", () => EnsureNavMeshTeleport("Greybox") | AppendSceneNote(WarrenNavMeshNote));
            GameFlowSceneSetup.EditScene(SceneNames.HubMR, EnsureLocomotionLock);
            GameFlowSceneSetup.EditScene(SceneNames.Title, EnsureLocomotionLock);
            GameFlowSceneSetup.EditScene(SceneNames.Setup, EnsureLocomotionLock);
            AssetDatabase.SaveAssets();
            Debug.Log("[PlayerRigSetup] PlayerRig is in Bootstrap and the sandboxes; NavMesh teleport, locks and debug panels are set up.");
        }

        // ------------------------------------------------------------------ assets

        /// <summary>Comfort presets (spec table values), the OnSettingsChanged channel, the fade material, the Teleport interaction layer.</summary>
        public static void EnsureAssets()
        {
            ProjectConfigurator.EnsureFolder(ComfortDir);
            ProjectConfigurator.EnsureFolder(ProjectConfigurator.DataDir + "/Events");
            ProjectConfigurator.CreateOrLoad<VoidEventChannel>(OnSettingsChangedPath);
            ProjectConfigurator.ConfigureInteractionLayers();
            EnsureFadeMaterial();

            // docs/design/comfort-and-accessibility.md, presets table. Every field is set, so the table is all here.
            ProjectConfigurator.CreateOrLoad<ComfortPreset>(PresetPath(ComfortPresetNames.Recommended), p =>
                SetPreset(p, ComfortPresetNames.Recommended, MovementMode.Smooth, TurnMode.Snap, true, 0.7f, 0.2f, true));
            ProjectConfigurator.CreateOrLoad<ComfortPreset>(PresetPath(ComfortPresetNames.Comfortable), p =>
                SetPreset(p, ComfortPresetNames.Comfortable, MovementMode.Teleport, TurnMode.Snap, true, 0.55f, 0.15f, true));
            ProjectConfigurator.CreateOrLoad<ComfortPreset>(PresetPath(ComfortPresetNames.Advanced), p =>
                SetPreset(p, ComfortPresetNames.Advanced, MovementMode.Smooth, TurnMode.Smooth, false, 0.7f, 0.2f, false));
        }

        static string PresetPath(string id) => $"{ComfortDir}/{id}.asset";

        static void SetPreset(ComfortPreset p, string id, MovementMode movement, TurnMode turning, bool vignette, float aperture, float feathering, bool dashFade)
        {
            p.id = id;
            p.movement = movement;
            p.walkSpeedMetersPerSecond = 1.5f;   // CMF-2
            p.sprintSpeedMetersPerSecond = 3f;   // CMF-2
            p.turning = turning;
            p.snapTurnDegrees = 45f;
            p.smoothTurnDegreesPerSecond = 60f;  // Advanced default; the player slider comes with the settings menu
            p.vignette = vignette;
            p.vignetteApertureSize = aperture;
            p.vignetteFeathering = feathering;
            p.vignetteEaseInSeconds = 0.3f;
            p.vignetteEaseOutSeconds = 0.3f;
            p.teleportFadeSeconds = 0.1f;
            p.dashFade = dashFade;
        }

        static Material EnsureFadeMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(FadeMaterialPath);
            if (material != null) return material;

            var shader = Shader.Find(FadeShaderName);
            if (shader == null)
            {
                throw new InvalidOperationException($"Shader {FadeShaderName} not found. Is Art/Shaders/ScreenFade.shader imported?");
            }

            ProjectConfigurator.EnsureFolder(ProjectConfigurator.MaterialsDir);
            material = new Material(shader);
            material.SetColor("_Color", Color.black);
            AssetDatabase.CreateAsset(material, FadeMaterialPath);
            return material;
        }

        // ------------------------------------------------------------------ prefab

        /// <summary>The PlayerRig prefab; built from the Starter rig when it doesn't exist. Null if the Starter rig is missing.</summary>
        public static GameObject EnsureRigPrefab()
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(RigPrefabPath);
            if (existing != null)
            {
                EnsureRigSettings();
                return existing;
            }

            var starter = FindStarterAsset("XR Origin (XR Rig) t:Prefab");
            if (starter == null)
            {
                Debug.LogWarning("[PlayerRigSetup] XR Origin (XR Rig) prefab not found; run ConfigureSettings first to import XRI Starter Assets.");
                return null;
            }

            EnsureAssets();
            var built = BuildRigPrefab(starter);
            EnsureRigSettings();
            return built;
        }

        /// <summary>
        /// Values the rig must have whether the prefab was just built or already existed; saves only on a change.
        /// </summary>
        static void EnsureRigSettings()
        {
            var root = PrefabUtility.LoadPrefabContents(RigPrefabPath);
            try
            {
                bool changed = false;
                foreach (var body in root.GetComponentsInChildren<CharacterController>(true))
                {
                    if (!Mathf.Approximately(body.stepOffset, StepOffsetMeters))
                    {
                        body.stepOffset = StepOffsetMeters;
                        changed = true;
                    }
                }

                // The spec turns by snap 45° or smoothly; pulling the stick back must not flip the player 180°.
                foreach (var turn in root.GetComponentsInChildren<SnapTurnProvider>(true))
                {
                    changed |= turn.enableTurnAround;
                    turn.enableTurnAround = false;
                }
                foreach (var turn in root.GetComponentsInChildren<ContinuousTurnProvider>(true))
                {
                    changed |= turn.enableTurnAround;
                    turn.enableTurnAround = false;
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, RigPrefabPath);
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        static GameObject FindStarterAsset(string search)
        {
            string guid = AssetDatabase.FindAssets(search, new[] { SamplesDir }).FirstOrDefault();
            return guid == null ? null : AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
        }

        static GameObject BuildRigPrefab(GameObject starter)
        {
            var vignettePrefab = FindStarterAsset("TunnelingVignette t:Prefab");
            if (vignettePrefab == null) throw new InvalidOperationException("XRI Starter Assets TunnelingVignette prefab not found.");

            // Work in a scratch scene; the next EditScene replaces it.
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = (GameObject)PrefabUtility.InstantiatePrefab(starter);
            // Unpack only the outer rig, so the interactor and controller prefabs inside stay linked to the sample.
            PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            root.name = "PlayerRig";
            root.tag = "Player"; // GatePortal and PlayerSpawn find the rig by this tag

            var origin = root.GetComponent<XROrigin>();
            origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
            Transform camera = origin.Camera.transform;

            var gazeAssistance = root.GetComponent<XRGazeAssistance>();
            if (gazeAssistance != null)
            {
                Object.DestroyImmediate(gazeAssistance);
            }
            foreach (string part in UnusedRigParts)
            {
                Object.DestroyImmediate(Find(root, part).gameObject);
            }

            // Bootstrap used to add these to the rig instance; the rig carries them now (MRH-1 turns them on in Hub_MR).
            root.AddComponent<ARPlaneManager>().enabled = false;
            camera.gameObject.AddComponent<ARCameraManager>().enabled = false;

            var move = ReplaceMoveProvider(Find(root, "Locomotion/Move").gameObject, camera);
            var snapTurn = Find(root, "Locomotion/Turn").GetComponent<SnapTurnProvider>();
            var smoothTurn = Find(root, "Locomotion/Turn").GetComponent<ContinuousTurnProvider>();
            smoothTurn.enabled = false; // Recommended snaps; ComfortSettings switches at runtime
            var teleportation = Find(root, "Locomotion/Teleportation").GetComponent<TeleportationProvider>();

            // Ground check must see the Environment layer, where every floor lives.
            var gravity = Find(root, "Locomotion/Gravity").GetComponent<GravityProvider>();
            gravity.sphereCastLayerMask |= EnvironmentMask();

            var fader = CreateScreenFade(camera);
            var teleportFade = teleportation.gameObject.AddComponent<TeleportFade>();
            Wire(teleportFade, ("_teleportation", teleportation), ("_fader", fader));

            var vignette = AddVignette(vignettePrefab, camera, move, snapTurn, smoothTurn);

            var leftHand = Find(root, "Camera Offset/Left Controller").GetComponent<ControllerInputActionManager>();
            var rightHand = Find(root, "Camera Offset/Right Controller").GetComponent<ControllerInputActionManager>();
            var leftTeleport = Find(root, "Camera Offset/Left Controller/Teleport Interactor").gameObject;
            var rightTeleport = Find(root, "Camera Offset/Right Controller/Teleport Interactor").gameObject;
            SetUpTeleportRay(leftTeleport);
            SetUpTeleportRay(rightTeleport);

            var presets = new[]
            {
                AssetDatabase.LoadAssetAtPath<ComfortPreset>(PresetPath(ComfortPresetNames.Recommended)),
                AssetDatabase.LoadAssetAtPath<ComfortPreset>(PresetPath(ComfortPresetNames.Comfortable)),
                AssetDatabase.LoadAssetAtPath<ComfortPreset>(PresetPath(ComfortPresetNames.Advanced)),
            };
            var comfort = root.AddComponent<ComfortSettings>();
            var serialized = new SerializedObject(comfort);
            var presetList = serialized.FindProperty("_presets");
            presetList.arraySize = presets.Length;
            for (int i = 0; i < presets.Length; i++)
            {
                presetList.GetArrayElementAtIndex(i).objectReferenceValue = presets[i];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            Wire(comfort,
                ("_defaultPreset", presets[0]),
                ("_onSettingsChanged", AssetDatabase.LoadAssetAtPath<VoidEventChannel>(OnSettingsChangedPath)),
                ("_move", move), ("_snapTurn", snapTurn), ("_smoothTurn", smoothTurn), ("_teleportation", teleportation),
                ("_teleportFade", teleportFade), ("_gravity", gravity), ("_vignette", vignette),
                ("_leftHand", leftHand), ("_rightHand", rightHand),
                ("_leftTeleportInteractor", leftTeleport), ("_rightTeleportInteractor", rightTeleport));

            ProjectConfigurator.EnsureFolder(Path.GetDirectoryName(RigPrefabPath)?.Replace('\\', '/'));
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, RigPrefabPath, out bool saved);
            if (!saved) throw new InvalidOperationException($"Could not save {RigPrefabPath}.");
            Object.DestroyImmediate(root);
            Debug.Log($"[PlayerRigSetup] Built {RigPrefabPath} from {AssetDatabase.GetAssetPath(starter)}.");
            return prefab;
        }

        // The Starter's DynamicMoveProvider becomes our QuantizedMoveProvider (CMF-2), keeping its inputs and head-relative forward.
        static QuantizedMoveProvider ReplaceMoveProvider(GameObject moveObject, Transform camera)
        {
            var old = moveObject.GetComponent<ContinuousMoveProvider>();
            var move = moveObject.AddComponent<QuantizedMoveProvider>();
            var from = new SerializedObject(old);
            var to = new SerializedObject(move);
            foreach (string field in new[] { "m_Mediator", "m_TransformationPriority", "m_LeftHandMoveInput", "m_RightHandMoveInput" })
            {
                to.CopyFromSerializedProperty(from.FindProperty(field));
            }
            to.ApplyModifiedPropertiesWithoutUndo();
            Object.DestroyImmediate(old);

            move.forwardSource = camera;
            move.enableStrafe = true;
            move.enableFly = false;
            // Full air control: the base class otherwise eases air velocity in, an acceleration curve (CMF-2).
            move.inAirControlModifier = 1f;
            return move;
        }

        static ScreenFader CreateScreenFade(Transform camera)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Screen Fade";
            Object.DestroyImmediate(sphere.GetComponent<Collider>());
            sphere.transform.SetParent(camera, false);
            // A sphere around the eyes covers every direction; it only has to sit beyond the near clip plane.
            float radius = Mathf.Max(0.25f, camera.GetComponent<Camera>().nearClipPlane * 4f);
            sphere.transform.localScale = Vector3.one * (radius * 2f);

            var renderer = sphere.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = EnsureFadeMaterial();
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            var fader = sphere.AddComponent<ScreenFader>();
            Wire(fader, ("_fadeRenderer", renderer));
            return fader;
        }

        // #72: the vignette shows only while a stick moves or turns the player; strength comes from the preset at runtime.
        static TunnelingVignetteController AddVignette(GameObject prefab, Transform camera, params LocomotionProvider[] providers)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, camera);
            var vignette = instance.GetComponent<TunnelingVignetteController>();
            vignette.locomotionVignetteProviders.Clear();
            foreach (var provider in providers)
            {
                vignette.locomotionVignetteProviders.Add(new LocomotionVignetteProvider { locomotionProvider = provider, enabled = true });
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(vignette);
            return vignette;
        }

        // #71: the teleport ray hits Environment floors; an arc that can't teleport (blocked or invalid) is grey.
        static void SetUpTeleportRay(GameObject teleportInteractor)
        {
            var ray = teleportInteractor.GetComponent<XRRayInteractor>();
            ray.raycastMask |= EnvironmentMask();
            PrefabUtility.RecordPrefabInstancePropertyModifications(ray);

            var line = teleportInteractor.GetComponent<XRInteractorLineVisual>();
            line.invalidColorGradient = Grey(line.invalidColorGradient);
            line.blockedColorGradient = Grey(line.blockedColorGradient);
            PrefabUtility.RecordPrefabInstancePropertyModifications(line);
        }

        static Gradient Grey(Gradient source)
        {
            var colorKeys = source.colorKeys;
            for (int i = 0; i < colorKeys.Length; i++)
            {
                colorKeys[i].color = NoTeleportGrey;
            }

            var grey = new Gradient();
            grey.SetKeys(colorKeys, source.alphaKeys);
            return grey;
        }

        static int EnvironmentMask()
        {
            int layer = LayerMask.NameToLayer("Environment");
            if (layer < 0) throw new InvalidOperationException("No Environment layer; run Tools > SLA > Configure Project Settings.");
            return 1 << layer;
        }

        static Transform Find(GameObject root, string path)
        {
            var found = root.transform.Find(path);
            if (found == null) throw new InvalidOperationException($"The Starter rig has no {path}; has the XRI sample changed?");
            return found;
        }

        // ------------------------------------------------------------------ scenes

        static string[] FindSandboxScenes() =>
            AssetDatabase.FindAssets("t:Scene", new[] { SandboxDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => Path.GetDirectoryName(path)?.Replace('\\', '/') == SandboxDir)
                .Select(path => "Sandbox/" + Path.GetFileNameWithoutExtension(path))
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

        static GameObject LoadRig() => GameFlowSceneSetup.Load<GameObject>(RigPrefabPath);

        static VoidEventChannel LoadSettingsChanged() => GameFlowSceneSetup.Load<VoidEventChannel>(OnSettingsChangedPath);

        static bool SetUpBootstrap()
        {
            bool changed = EnsureRig(LoadRig(), out GameObject rig);

            var systems = GameObject.Find("Systems");
            if (systems == null) throw new InvalidOperationException("Bootstrap has no Systems object.");

            // The scene fader used to live on Systems with its sphere on the Starter rig; it comes with the rig now.
            var oldFader = systems.GetComponent<ScreenFader>();
            if (oldFader != null)
            {
                Object.DestroyImmediate(oldFader);
                changed = true;
            }

            changed |= WireIfChanged(rig.GetComponentInChildren<ScreenFader>(true), "_sceneLoader", systems.GetComponent<SceneLoader>());
            changed |= WireIfChanged(systems.GetComponent<GameBootstrapper>(), "_onSettingsChanged", LoadSettingsChanged());
            return changed;
        }

        static bool SetUpSandbox()
        {
            bool changed = EnsureRig(LoadRig(), out _);
            changed |= EnsureNavMeshTeleport("Floor");
            changed |= EnsureEventSystem();
            changed |= EnsureDebugPanel();
            return changed;
        }

        /// <summary>Puts one PlayerRig instance in the open scene, replacing any other XR Origin (the Starter rig) where it stood.</summary>
        static bool EnsureRig(GameObject rigPrefab, out GameObject rig)
        {
            rig = null;
            bool changed = false;
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;

            foreach (var origin in Object.FindObjectsByType<XROrigin>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var root = PrefabUtility.GetOutermostPrefabInstanceRoot(origin.gameObject) ?? origin.gameObject;
                if (rig == null && PrefabUtility.GetCorrespondingObjectFromSource(root) == rigPrefab)
                {
                    rig = root;
                    continue;
                }

                position = root.transform.position;
                rotation = root.transform.rotation;
                Object.DestroyImmediate(root);
                changed = true;
            }

            if (rig == null)
            {
                rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPrefab, SceneManager.GetActiveScene());
                rig.transform.SetPositionAndRotation(position, rotation);
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// #71: a NavMesh baked from the Environment layer (saved next to the scene) and a NavMesh-only teleport
        /// area on <paramref name="floorRootName"/> and its child colliders.
        /// </summary>
        static bool EnsureNavMeshTeleport(string floorRootName)
        {
            bool changed = false;
            var floor = GameObject.Find(floorRootName);
            if (floor == null) throw new InvalidOperationException($"{SceneManager.GetActiveScene().name} has no {floorRootName}.");

            if (floor.GetComponent<NavMeshTeleportationArea>() == null)
            {
                var area = floor.AddComponent<NavMeshTeleportationArea>();
                area.interactionLayers = 1 << ProjectConfigurator.TeleportInteractionLayer;
                area.filterSelectionByHitNormal = true; // floors only, never walls
                area.matchOrientation = MatchOrientation.WorldSpaceUp; // CMF-5: the horizon stays level
                changed = true;
            }

            var surface = Object.FindAnyObjectByType<NavMeshSurface>();
            if (surface == null)
            {
                surface = new GameObject("NavMesh Surface").AddComponent<NavMeshSurface>();
                surface.collectObjects = CollectObjects.All;
                surface.layerMask = EnvironmentMask();
                surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
                changed = true;
            }

            if (surface.navMeshData == null)
            {
                BakeNavMesh(surface);
                changed = true;
            }

            return changed;
        }

        // Like the NavMeshSurface Bake button: the data goes in a folder named after the scene, next to it.
        static void BakeNavMesh(NavMeshSurface surface)
        {
            surface.BuildNavMesh();
            if (surface.navMeshData == null) throw new InvalidOperationException($"NavMesh bake failed in {surface.gameObject.scene.name}.");

            string scenePath = surface.gameObject.scene.path;
            string folder = $"{Path.GetDirectoryName(scenePath)?.Replace('\\', '/')}/{Path.GetFileNameWithoutExtension(scenePath)}";
            ProjectConfigurator.EnsureFolder(folder);
            string assetPath = $"{folder}/NavMesh-{surface.gameObject.name}.asset";
            if (AssetDatabase.LoadAssetAtPath<NavMeshData>(assetPath) != null)
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
            AssetDatabase.CreateAsset(surface.navMeshData, assetPath);
            EditorUtility.SetDirty(surface);
        }

        static bool EnsureLocomotionLock()
        {
            if (Object.FindAnyObjectByType<StickLocomotionLock>(FindObjectsInactive.Include) != null) return false;
            new GameObject("Stick Locomotion Lock").AddComponent<StickLocomotionLock>();
            return true;
        }

        // Sandboxes have no Bootstrap, so ray clicks on the debug panel need their own EventSystem.
        static bool EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include) != null) return false;
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem.AddComponent<XRUIInputModule>();
            return true;
        }

        static bool EnsureDebugPanel()
        {
            if (Object.FindAnyObjectByType<ComfortDebugMenu>(FindObjectsInactive.Include) != null) return false;

            var panel = GameFlowSceneSetup.CreateCanvas("Comfort Debug Panel (sandbox only)", new Vector2(640f, 690f));
            // To the left of the spawn, 1.7 m away (UI-11), turned to face it, clear of the floor ahead for teleport tests.
            var position = new Vector3(-1.3f, 1.3f, 1.1f);
            panel.SetPositionAndRotation(position, Quaternion.LookRotation(new Vector3(position.x, 0f, position.z)));

            var menu = panel.gameObject.AddComponent<ComfortDebugMenu>();
            GameFlowSceneSetup.AddText(panel, "[Comfort: sandbox only]", 40, GameFlowSceneSetup.SystemBlue, 56f);
            var status = GameFlowSceneSetup.AddText(panel, "Recommended, right-handed", 28, GameFlowSceneSetup.OffWhite, 44f);
            GameFlowSceneSetup.AddButton(panel, "Recommended", menu.UseRecommended);
            GameFlowSceneSetup.AddButton(panel, "Comfortable (teleport)", menu.UseComfortable);
            GameFlowSceneSetup.AddButton(panel, "Advanced (smooth turn)", menu.UseAdvanced);
            GameFlowSceneSetup.AddButton(panel, "Left-handed on/off", menu.ToggleLeftHanded);
            Wire(menu, ("_onSettingsChanged", LoadSettingsChanged()), ("_status", status));
            return true;
        }

        static bool AppendSceneNote(string line)
        {
            var note = Object.FindAnyObjectByType<SceneNote>(FindObjectsInactive.Include);
            if (note == null || (note.text != null && note.text.Contains(line))) return false;
            note.text = string.IsNullOrEmpty(note.text) ? line : $"{note.text.TrimEnd()}\n{line}";
            EditorUtility.SetDirty(note);
            return true;
        }

        // ------------------------------------------------------------------ helpers

        static void Wire(Object target, params (string field, Object value)[] fields)
        {
            var serialized = new SerializedObject(target);
            foreach (var (field, value) in fields)
            {
                var property = serialized.FindProperty(field);
                if (property == null) throw new InvalidOperationException($"{target.GetType().Name} has no field {field}.");
                property.objectReferenceValue = value;
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        static bool WireIfChanged(Object target, string field, Object value)
        {
            if (target == null) throw new InvalidOperationException($"Nothing to set {field} on.");
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException($"{target.GetType().Name} has no field {field}.");
            if (property.objectReferenceValue == value) return false;
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            return true;
        }
    }
}
