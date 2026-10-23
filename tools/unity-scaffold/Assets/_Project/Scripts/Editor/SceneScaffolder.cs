using System;
using System.Collections.Generic;
using System.Linq;
using SLA.AI;
using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using SLA.Gates;
using SLA.MR;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SLA.EditorTools
{
    /// <summary>
    /// Builds the starting scenes. Re-running overwrites them, so only use it on a fresh project.
    /// Headless: -executeMethod SLA.EditorTools.SceneScaffolder.BuildScenes
    /// </summary>
    public static class SceneScaffolder
    {
        static readonly string[] TeamSandboxes = { "Ciara", "Elijah", "Rickey", "Samaii", "Tiara" };

        const string Dir = ProjectConfigurator.ScenesDir;

        static Material _greybox;
        static Material _floor;
        static Material _sand;
        static Material _portal;

        [MenuItem("Tools/SLA/Rebuild Starting Scenes (overwrites)")]
        public static void BuildScenesMenu()
        {
            if (EditorUtility.DisplayDialog("Rebuild scenes", "This overwrites the starting scenes. Continue?", "Rebuild", "Cancel"))
            {
                Run(false);
            }
        }

        public static void BuildScenes() => Run(true);

        static void Run(bool exitWhenDone)
        {
            try
            {
                ProjectConfigurator.EnsureFolders();
                CreateMaterials();
                var origin = FindXrOriginPrefab();
                var gate = AssetDatabase.LoadAssetAtPath<GateData>(ProjectConfigurator.DataDir + "/Gates/GoblinWarren.asset");

                var scenes = new List<string>
                {
                    BuildBootstrap(origin),
                    BuildEmpty(SceneNames.Title, "Title screen: Continue / New game (MR & UI engineer)."),
                    BuildEmpty(SceneNames.Setup, "First-run setup: height calibration, comfort preset, room scan check."),
                    BuildHubMR(gate),
                    BuildHubVirtual(gate),
                    BuildPenaltyDesert(),
                    BuildGoblinWarren(),
                };

                foreach (string member in TeamSandboxes)
                {
                    BuildSandbox(member, origin);
                }

                EditorBuildSettings.scenes = scenes.Select(p => new EditorBuildSettingsScene(p, true)).ToArray();
                AssetDatabase.SaveAssets();
                Debug.Log($"[SceneScaffolder] Built {scenes.Count} build scenes and {TeamSandboxes.Length} sandboxes.");
                if (exitWhenDone) EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (exitWhenDone) EditorApplication.Exit(1);
            }
        }

        // ------------------------------------------------------------------ scenes

        static string BuildBootstrap(GameObject originPrefab)
        {
            var scene = NewScene();

            var origin = InstantiateOrigin(originPrefab);
            if (origin != null)
            {
                var xrOrigin = origin.GetComponent<XROrigin>();
                var planes = origin.AddComponent<ARPlaneManager>();
                planes.enabled = false;
                if (xrOrigin != null && xrOrigin.Camera != null)
                {
                    var arCamera = xrOrigin.Camera.gameObject.AddComponent<ARCameraManager>();
                    arCamera.enabled = false;
                }
            }

            var session = new GameObject("AR Session", typeof(ARSession));
            session.GetComponent<ARSession>().enabled = false;

            var systems = new GameObject("Systems");
            var states = systems.AddComponent<GameStateManager>();
            Assign(states, "_onGameStateChanged", Load<GameStateEventChannel>("Events/OnGameStateChanged"));
            systems.AddComponent<SceneLoader>();
            systems.AddComponent<GameBootstrapper>();

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem.AddComponent<XRUIInputModule>();

            return Save(scene, SceneNames.Bootstrap);
        }

        static string BuildEmpty(string name, string note)
        {
            var scene = NewScene();
            AddNote(note);
            return Save(scene, name);
        }

        static string BuildHubMR(GateData gate)
        {
            var scene = NewScene();
            AddLight();
            new GameObject("Player Spawn").AddComponent<PlayerSpawn>();

            var portal = CreatePortal("Gate Portal", gate, SceneNames.HubMR, GameState.Hub);

            var hub = new GameObject("MR Hub");
            var passthrough = hub.AddComponent<PassthroughController>();
            var finder = hub.AddComponent<KeyWallFinder>();
            var controller = hub.AddComponent<MRHubController>();
            Assign(controller, "_passthrough", passthrough);
            Assign(controller, "_wallFinder", finder);
            Assign(controller, "_portal", portal.transform);

            return Save(scene, SceneNames.HubMR);
        }

        static string BuildHubVirtual(GateData gate)
        {
            var scene = NewScene();
            AddLight();
            new GameObject("Player Spawn").AddComponent<PlayerSpawn>();
            Box("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(6f, 0.1f, 6f), _floor);
            Box("Back Wall", new Vector3(0f, 1.5f, 3f), new Vector3(6f, 3f, 0.2f), _greybox);
            var portal = CreatePortal("Gate Portal", gate, SceneNames.HubVirtual, GameState.Hub);
            portal.transform.SetPositionAndRotation(new Vector3(0f, 1.05f, 2.88f), Quaternion.identity);
            AddNote("Virtual apartment fallback for players without a room scan (or at the expo).");
            return Save(scene, SceneNames.HubVirtual);
        }

        static string BuildPenaltyDesert()
        {
            var scene = NewScene();
            AddLight();
            new GameObject("Player Spawn").AddComponent<PlayerSpawn>();
            Box("Sand", new Vector3(0f, -0.05f, 0f), new Vector3(60f, 0.1f, 60f), _sand);
            AddNote("Penalty Zone: survive giant centipedes for 3 minutes (GDD: Quest system).");
            return Save(scene, SceneNames.PenaltyDesert);
        }

        /// <summary>Greybox of the gate template: entrance, two combat rooms, antechamber, boss room.</summary>
        static string BuildGoblinWarren()
        {
            var scene = NewScene();
            AddLight();

            var spawn = new GameObject("Player Spawn").AddComponent<PlayerSpawn>();
            spawn.transform.position = Vector3.zero;

            var root = new GameObject("Greybox").transform;
            Box("Entrance (6x6)", new Vector3(0f, -0.1f, 0f), new Vector3(6f, 0.2f, 6f), _floor, root);
            Box("Corridor 1", new Vector3(0f, -0.1f, 5f), new Vector3(2.5f, 0.2f, 4f), _greybox, root);
            Box("Room 1 (8x8)", new Vector3(0f, -0.1f, 11f), new Vector3(8f, 0.2f, 8f), _floor, root);
            Box("Corridor 2", new Vector3(0f, -0.1f, 17f), new Vector3(2.5f, 0.2f, 4f), _greybox, root);
            Box("Room 2 (8x8)", new Vector3(0f, -0.1f, 23f), new Vector3(8f, 0.2f, 8f), _floor, root);
            Box("Corridor 3", new Vector3(0f, -0.1f, 29f), new Vector3(2.5f, 0.2f, 4f), _greybox, root);
            Box("Antechamber (5x5)", new Vector3(0f, -0.1f, 33.5f), new Vector3(5f, 0.2f, 5f), _floor, root);
            Box("Boss Room (12x12)", new Vector3(0f, -0.1f, 42f), new Vector3(12f, 0.2f, 12f), _floor, root);

            var director = new GameObject("Enemy Director").AddComponent<EnemyDirector>();
            Assign(director, "_config", AssetDatabase.LoadAssetAtPath<CombatConfig>(ProjectConfigurator.SettingsDir + "/CombatConfig.asset"));

            var exit = CreatePortal("Exit Portal (to hub)", null, SceneNames.HubMR, GameState.Hub);
            exit.transform.SetPositionAndRotation(new Vector3(0f, 1.05f, 47.5f), Quaternion.identity);

            AddNote("Greybox only. Next: ProBuilder walls, NavMeshSurface bake, enemy spawners from GateData.");
            return Save(scene, SceneNames.GateGoblinWarren, "Gates");
        }

        static void BuildSandbox(string member, GameObject originPrefab)
        {
            var scene = NewScene();
            AddLight();
            InstantiateOrigin(originPrefab);
            Box("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(10f, 0.1f, 10f), _floor);
            AddNote($"{member}'s sandbox. Never shipped; test prefabs here instead of editing shared scenes.");
            Save(scene, $"Sandbox_{member}", "Sandbox");
        }

        // ------------------------------------------------------------------ helpers

        static Scene NewScene() => EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        static string Save(Scene scene, string name, string subfolder = null)
        {
            string folder = subfolder == null ? Dir : $"{Dir}/{subfolder}";
            ProjectConfigurator.EnsureFolder(folder);
            string path = $"{folder}/{name}.unity";
            EditorSceneManager.SaveScene(scene, path);
            return path;
        }

        static GameObject FindXrOriginPrefab()
        {
            string guid = AssetDatabase.FindAssets("XR Origin (XR Rig) t:Prefab", new[] { "Assets/Samples" }).FirstOrDefault();
            if (guid == null)
            {
                Debug.LogWarning("[SceneScaffolder] XR Origin (XR Rig) prefab not found; run ConfigureSettings first to import XRI Starter Assets.");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
        }

        static GameObject InstantiateOrigin(GameObject prefab)
        {
            if (prefab == null) return null;
            var origin = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            origin.tag = "Player";
            return origin;
        }

        static GameObject CreatePortal(string name, GateData gate, string targetScene, GameState targetState)
        {
            var portal = GameObject.CreatePrimitive(PrimitiveType.Quad);
            portal.name = name;
            portal.transform.localScale = new Vector3(1.2f, 2f, 1f);
            UnityEngine.Object.DestroyImmediate(portal.GetComponent<MeshCollider>());
            var trigger = portal.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, 1f, 0.6f);
            portal.GetComponent<MeshRenderer>().sharedMaterial = _portal;

            var gatePortal = portal.AddComponent<GatePortal>();
            Assign(gatePortal, "_gate", gate);
            AssignString(gatePortal, "_targetScene", targetScene);
            AssignEnum(gatePortal, "_targetState", (int)targetState);
            return portal;
        }

        static GameObject Box(string name, Vector3 position, Vector3 size, Material material, Transform parent = null)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.position = position;
            box.transform.localScale = size;
            box.GetComponent<MeshRenderer>().sharedMaterial = material;
            box.isStatic = true;
            int layer = LayerMask.NameToLayer("Environment");
            if (layer >= 0) box.layer = layer;
            return box;
        }

        static void AddLight()
        {
            var light = new GameObject("Directional Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        static void AddNote(string text)
        {
            var note = new GameObject("_README (scene notes)");
            note.tag = "EditorOnly";
            var comment = note.AddComponent<SceneNote>();
            comment.text = text;
        }

        static void CreateMaterials()
        {
            _greybox = MakeMaterial("M_Greybox", "Universal Render Pipeline/Lit", new Color(0.55f, 0.57f, 0.6f));
            _floor = MakeMaterial("M_Greybox_Floor", "Universal Render Pipeline/Lit", new Color(0.35f, 0.37f, 0.4f));
            _sand = MakeMaterial("M_Sand", "Universal Render Pipeline/Lit", new Color(0.76f, 0.64f, 0.42f));
            _portal = MakeMaterial("M_Portal", "Universal Render Pipeline/Unlit", new Color(0.247f, 0.722f, 1f));
        }

        static Material MakeMaterial(string name, string shaderName, Color color)
        {
            string path = $"{ProjectConfigurator.MaterialsDir}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogWarning($"[SceneScaffolder] Shader {shaderName} not found; using the default material for {name}.");
                return null;
            }
            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        static T Load<T>(string relativeDataPath) where T : UnityEngine.Object =>
            AssetDatabase.LoadAssetAtPath<T>($"{ProjectConfigurator.DataDir}/{relativeDataPath}.asset");

        static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) { Debug.LogWarning($"[SceneScaffolder] {target.GetType().Name}.{field} not found."); return; }
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void AssignString(UnityEngine.Object target, string field, string value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) return;
            prop.stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void AssignEnum(UnityEngine.Object target, string field, int value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            if (prop == null) return;
            prop.enumValueIndex = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
