using System;
using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using SLA.Gates;
using SLA.UI;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SLA.EditorTools
{
    /// <summary>
    /// Wires the game flow (issue #15) into the scenes: the screen fader and save-on-exit in Bootstrap,
    /// placeholder menus in Title and Setup, and the first gate on the hub portals. Re-runnable: anything
    /// already there is left alone.
    /// Headless: -executeMethod SLA.EditorTools.GameFlowSceneSetup.ApplyBatch
    /// </summary>
    public static class GameFlowSceneSetup
    {
        const string FadeShaderName = "SLA/ScreenFade";
        const string FadeMaterialPath = ProjectConfigurator.MaterialsDir + "/M_ScreenFade.mat";
        const string FirstGatePath = ProjectConfigurator.DataDir + "/Gates/GoblinWarren.asset";

        // UI-1 palette (docs/ART_PIPELINE.md): navy panel, System blue, off-white text.
        static readonly Color Navy = new Color32(0x0B, 0x1A, 0x2E, 0xCC);
        static readonly Color SystemBlue = new Color32(0x3F, 0xB8, 0xFF, 0xFF);
        static readonly Color ButtonFill = new Color32(0x3F, 0xB8, 0xFF, 0x40);
        static readonly Color OffWhite = new Color32(0xDA, 0xDA, 0xDA, 0xFF);

        [MenuItem("Tools/SLA/Set Up Game Flow In Scenes")]
        public static void ApplyMenu() => ApplyToScenes();

        public static void ApplyBatch()
        {
            try
            {
                ApplyToScenes();
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }

        public static void ApplyToScenes()
        {
            var fadeMaterial = EnsureFadeMaterial();
            EditScene(SceneNames.Bootstrap, () => SetUpBootstrap(fadeMaterial));
            EditScene(SceneNames.Title, SetUpTitle);
            EditScene(SceneNames.Setup, SetUpSetup);
            EditScene(SceneNames.HubMR, SetUpHubPortals);
            EditScene(SceneNames.HubVirtual, SetUpHubPortals);
            AssetDatabase.SaveAssets();
            Debug.Log("[GameFlowSceneSetup] Bootstrap, Title, Setup and the hubs are wired for the game flow.");
        }

        // Saves only when something was added, so re-running leaves scene files untouched.
        static void EditScene(string name, Func<bool> edit)
        {
            var scene = EditorSceneManager.OpenScene($"{ProjectConfigurator.ScenesDir}/{name}.unity", OpenSceneMode.Single);
            if (!edit()) return;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
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

        // ------------------------------------------------------------------ Hubs

        // A hub portal with no gate would lead back to the hub. The gate is loaded after the scene opens:
        // opening a scene unloads assets nothing references, which is how SceneScaffolder lost it.
        static bool SetUpHubPortals()
        {
            var gate = AssetDatabase.LoadAssetAtPath<GateData>(FirstGatePath);
            if (gate == null) throw new InvalidOperationException($"{FirstGatePath} not found.");

            bool changed = false;
            foreach (var portal in UnityEngine.Object.FindObjectsByType<GatePortal>(FindObjectsSortMode.None))
            {
                if (portal.Gate != null) continue;
                Assign(portal, "_gate", gate);
                changed = true;
            }

            return changed;
        }

        // ------------------------------------------------------------------ Bootstrap

        static bool SetUpBootstrap(Material fadeMaterial)
        {
            bool changed = false;
            var systems = GameObject.Find("Systems");
            if (systems == null) throw new InvalidOperationException("Bootstrap has no Systems object.");

            if (systems.GetComponent<SaveOnStateExit>() == null)
            {
                var channel = AssetDatabase.LoadAssetAtPath<GameStateEventChannel>(ProjectConfigurator.DataDir + "/Events/OnGameStateChanged.asset");
                Assign(systems.AddComponent<SaveOnStateExit>(), "_onGameStateChanged", channel);
                changed = true;
            }

            if (systems.GetComponent<ScreenFader>() == null)
            {
                var origin = UnityEngine.Object.FindAnyObjectByType<XROrigin>();
                if (origin == null || origin.Camera == null) throw new InvalidOperationException("Bootstrap has no XR Origin camera for the fade.");

                var fader = systems.AddComponent<ScreenFader>();
                Assign(fader, "_fadeRenderer", CreateFadeSphere(origin.Camera, fadeMaterial));
                Assign(fader, "_sceneLoader", systems.GetComponent<SceneLoader>());
                changed = true;
            }

            return changed;
        }

        static MeshRenderer CreateFadeSphere(Camera camera, Material material)
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Screen Fade";
            UnityEngine.Object.DestroyImmediate(sphere.GetComponent<Collider>());
            sphere.transform.SetParent(camera.transform, false);
            // A sphere around the eyes covers every direction; it only has to sit beyond the near clip plane.
            float radius = Mathf.Max(0.25f, camera.nearClipPlane * 4f);
            sphere.transform.localScale = Vector3.one * (radius * 2f);

            var renderer = sphere.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            return renderer;
        }

        // ------------------------------------------------------------------ Title and Setup

        static bool SetUpTitle()
        {
            if (UnityEngine.Object.FindAnyObjectByType<TitleMenu>() != null) return false;

            EnsureSpawn();
            var menu = new GameObject("Title Menu").AddComponent<TitleMenu>();
            var panel = CreatePanel("Title Canvas", "SOLO LEVELING: AWAKENING", "[Placeholder title screen]");
            AddButton(panel, "Continue", menu.Continue);
            AddButton(panel, "Continue in the virtual room", menu.ContinueInVirtualRoom);
            AddButton(panel, "New game", menu.NewGame);
            return true;
        }

        static bool SetUpSetup()
        {
            if (UnityEngine.Object.FindAnyObjectByType<FirstRunSetupMenu>() != null) return false;

            EnsureSpawn();
            var menu = new GameObject("Setup Menu").AddComponent<FirstRunSetupMenu>();
            var panel = CreatePanel("Setup Canvas", "[Choose how you move]", "Recommended is the default.");
            AddButton(panel, "Recommended: smooth, snap turn", menu.ChooseRecommended);
            AddButton(panel, "Comfortable: teleport", menu.ChooseComfortable);
            AddButton(panel, "Advanced: smooth turning", menu.ChooseAdvanced);
            return true;
        }

        // The rig keeps its position between scenes, so menus need a spawn that faces them.
        static void EnsureSpawn()
        {
            if (UnityEngine.Object.FindAnyObjectByType<PlayerSpawn>() != null) return;
            new GameObject("Player Spawn").AddComponent<PlayerSpawn>();
        }

        static Transform CreatePanel(string name, string title, string subtitle)
        {
            var canvasObject = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

            var rect = (RectTransform)canvasObject.transform;
            rect.sizeDelta = new Vector2(720f, 540f);
            // 1.6 m ahead and a little below eye level (UI-3, UI-11); 1 canvas unit = 1 mm.
            rect.position = new Vector3(0f, 1.35f, 1.6f);
            rect.localScale = Vector3.one * 0.001f;

            canvasObject.AddComponent<Image>().color = Navy;
            canvasObject.AddComponent<Outline>().effectColor = SystemBlue;

            var layout = canvasObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 40, 40);
            layout.spacing = 20f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            AddText(canvasObject.transform, title, 44, SystemBlue, 64f);
            AddText(canvasObject.transform, subtitle, 28, OffWhite, 44f);
            return canvasObject.transform;
        }

        static void AddButton(Transform panel, string label, UnityAction onClick)
        {
            var buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.transform.SetParent(panel, false);
            buttonObject.GetComponent<Image>().color = ButtonFill;
            // UI-10: primary buttons at least 60 dp tall; these are 88 mm at 1.6 m.
            buttonObject.GetComponent<LayoutElement>().preferredHeight = 88f;

            var button = buttonObject.GetComponent<Button>();
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            UnityEventTools.AddPersistentListener(button.onClick, onClick);

            var text = AddText(buttonObject.transform, label, 34, Color.white, 0f);
            var textRect = (RectTransform)text.transform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
        }

        static Text AddText(Transform parent, string value, int size, Color color, float preferredHeight)
        {
            var textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            if (preferredHeight > 0f)
            {
                textObject.AddComponent<LayoutElement>().preferredHeight = preferredHeight;
            }

            var text = textObject.GetComponent<Text>();
            // Built-in font: placeholder menus shouldn't need TextMesh Pro's essential resources imported.
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            return text;
        }

        static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException($"{target.GetType().Name} has no field {field}.");
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
