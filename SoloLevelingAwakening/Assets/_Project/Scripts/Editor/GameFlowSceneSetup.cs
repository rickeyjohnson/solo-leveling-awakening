using System;
using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using SLA.Gates;
using SLA.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SLA.EditorTools
{
    /// <summary>
    /// Wires the game flow (issue #15) into the scenes: save-on-exit in Bootstrap, placeholder menus in
    /// Title (with the standing-play note, CMF-13) and Setup (height calibration, then the comfort preset,
    /// issue #11), and the first gate on the hub portals. The screen fader comes with the PlayerRig prefab
    /// (PlayerRigSetup). Re-runnable: anything already there is left alone.
    /// Headless: -executeMethod SLA.EditorTools.GameFlowSceneSetup.ApplyBatch
    /// </summary>
    public static class GameFlowSceneSetup
    {
        const string FirstGatePath = ProjectConfigurator.DataDir + "/Gates/GoblinWarren.asset";

        // UI-1 palette (docs/ART_PIPELINE.md): navy panel, System blue, off-white text.
        internal static readonly Color Navy = new Color32(0x0B, 0x1A, 0x2E, 0xCC);
        internal static readonly Color SystemBlue = new Color32(0x3F, 0xB8, 0xFF, 0xFF);
        internal static readonly Color ButtonFill = new Color32(0x3F, 0xB8, 0xFF, 0x40);
        internal static readonly Color OffWhite = new Color32(0xDA, 0xDA, 0xDA, 0xFF);

        const string StandingOnly = "Standing play only — seated play isn't supported.";

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
            ProjectConfigurator.CreateOrLoad<VoidEventChannel>(PlayerRigSetup.OnSettingsChangedPath);
            EditScene(SceneNames.Bootstrap, SetUpBootstrap);
            EditScene(SceneNames.Title, SetUpTitle);
            EditScene(SceneNames.Setup, SetUpSetup);
            EditScene(SceneNames.HubMR, SetUpHubPortals);
            EditScene(SceneNames.HubVirtual, SetUpHubPortals);
            AssetDatabase.SaveAssets();
            Debug.Log("[GameFlowSceneSetup] Bootstrap, Title, Setup and the hubs are wired for the game flow.");
        }

        /// <summary>
        /// Loads an asset by path. Call it inside an EditScene edit: opening a scene unloads assets that
        /// only a local variable holds, which would turn an earlier reference into null.
        /// </summary>
        internal static T Load<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null) throw new InvalidOperationException($"{path} is missing.");
            return asset;
        }

        /// <summary>
        /// Opens Scenes/<paramref name="name"/>.unity (name may include a subfolder) and saves it only when
        /// <paramref name="edit"/> returns true, so re-running leaves scene files untouched.
        /// </summary>
        internal static void EditScene(string name, Func<bool> edit)
        {
            var scene = EditorSceneManager.OpenScene($"{ProjectConfigurator.ScenesDir}/{name}.unity", OpenSceneMode.Single);
            if (!edit()) return;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // ------------------------------------------------------------------ Hubs

        // A hub portal with no gate would lead back to the hub. The gate is loaded inside the edit (see Load):
        // SceneScaffolder loaded it before opening scenes, which is how the portals lost it.
        static bool SetUpHubPortals()
        {
            var gate = Load<GateData>(FirstGatePath);
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

        static bool SetUpBootstrap()
        {
            var systems = GameObject.Find("Systems");
            if (systems == null) throw new InvalidOperationException("Bootstrap has no Systems object.");
            if (systems.GetComponent<SaveOnStateExit>() != null) return false;

            var channel = Load<GameStateEventChannel>(ProjectConfigurator.DataDir + "/Events/OnGameStateChanged.asset");
            Assign(systems.AddComponent<SaveOnStateExit>(), "_onGameStateChanged", channel);
            return true;
        }

        // ------------------------------------------------------------------ Title and Setup

        static bool SetUpTitle()
        {
            var existing = UnityEngine.Object.FindAnyObjectByType<TitleMenu>();
            if (existing != null && UnityEngine.Object.FindAnyObjectByType<EyeLevelPlacement>() != null) return false;

            // Title scenes built for #15 lack the standing-play line and eye-level placement: rebuild the placeholder.
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
                DestroyNamed("Title Canvas");
            }

            var settingsChanged = Load<VoidEventChannel>(PlayerRigSetup.OnSettingsChangedPath);
            EnsureSpawn();
            var menu = new GameObject("Title Menu").AddComponent<TitleMenu>();
            var panel = CreatePanel("Title Canvas", new Vector2(720f, 620f), "SOLO LEVELING: AWAKENING", "[Placeholder title screen]");
            AddButton(panel, "Continue", menu.Continue);
            AddButton(panel, "Continue in the virtual room", menu.ContinueInVirtualRoom);
            AddButton(panel, "New game", menu.NewGame);
            AddText(panel, StandingOnly, 28, OffWhite, 44f); // CMF-13
            AddEyeLevelPlacement(panel, settingsChanged);
            return true;
        }

        static bool SetUpSetup()
        {
            var existing = UnityEngine.Object.FindAnyObjectByType<FirstRunSetupMenu>();
            if (existing != null && UnityEngine.Object.FindAnyObjectByType<HeightCalibrator>() != null) return false;

            // Setup scenes built for #15 only had the preset buttons: rebuild the placeholder with calibration first.
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
                DestroyNamed("Setup Canvas");
            }

            var settingsChanged = Load<VoidEventChannel>(PlayerRigSetup.OnSettingsChangedPath);
            EnsureSpawn();
            var menuObject = new GameObject("Setup Menu");
            var calibrator = menuObject.AddComponent<HeightCalibrator>();
            var menu = menuObject.AddComponent<FirstRunSetupMenu>();

            var panel = CreateCanvas("Setup Canvas", new Vector2(720f, 760f));
            // The panel's height follows whichever step is showing.
            panel.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // CMF-13, QST-8: the System line comes from the narrative voice library.
            var calibration = AddGroup(panel, "Calibration Step");
            AddText(calibration, "[Stand still.]", 44, SystemBlue, 64f);
            AddText(calibration, "Measuring your standing height.", 28, OffWhite, 44f);
            var progressFill = AddProgressBar(calibration);

            var presets = AddGroup(panel, "Preset Step");
            AddText(presets, "[Choose how you move]", 44, SystemBlue, 64f);
            AddText(presets, "Recommended is the default.", 28, OffWhite, 44f);
            AddButton(presets, "Recommended: smooth, snap turn", menu.ChooseRecommended);
            AddButton(presets, "Comfortable: teleport", menu.ChooseComfortable);
            AddButton(presets, "Advanced: smooth turning", menu.ChooseAdvanced);
            var leftHanded = AddButton(presets, "Left-handed: off", menu.ToggleLeftHanded);
            AddButton(presets, "Measure again", menu.MeasureAgain);
            presets.gameObject.SetActive(false);

            Assign(calibrator, "_onSettingsChanged", settingsChanged);
            Assign(menu, "_calibrator", calibrator);
            Assign(menu, "_calibrationStep", calibration.gameObject);
            Assign(menu, "_presetStep", presets.gameObject);
            Assign(menu, "_progressFill", progressFill);
            Assign(menu, "_leftHandedLabel", leftHanded.GetComponentInChildren<Text>());
            Assign(menu, "_onSettingsChanged", settingsChanged);
            AddEyeLevelPlacement(panel, settingsChanged);
            return true;
        }

        // The rig keeps its position between scenes, so menus need a spawn that faces them.
        static void EnsureSpawn()
        {
            if (UnityEngine.Object.FindAnyObjectByType<PlayerSpawn>() != null) return;
            new GameObject("Player Spawn").AddComponent<PlayerSpawn>();
        }

        static void DestroyNamed(string name)
        {
            var found = GameObject.Find(name);
            if (found != null) UnityEngine.Object.DestroyImmediate(found);
        }

        // CMF-13: menus at the player's eye level, calibrated or live; the built position is the fallback.
        static void AddEyeLevelPlacement(Transform panel, VoidEventChannel settingsChanged)
        {
            Assign(panel.gameObject.AddComponent<EyeLevelPlacement>(), "_onSettingsChanged", settingsChanged);
        }

        // ------------------------------------------------------------------ UI building (shared with PlayerRigSetup)

        /// <summary>A navy world-space canvas, ray-clickable, with a vertical layout. 1 canvas unit = 1 mm.</summary>
        internal static Transform CreateCanvas(string name, Vector2 sizeMillimeters)
        {
            var canvasObject = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;

            var rect = (RectTransform)canvasObject.transform;
            rect.sizeDelta = sizeMillimeters;
            // 1.6 m ahead and a little below eye level (UI-3, UI-11) until EyeLevelPlacement knows the player's height.
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
            return canvasObject.transform;
        }

        internal static Transform CreatePanel(string name, Vector2 sizeMillimeters, string title, string subtitle)
        {
            var panel = CreateCanvas(name, sizeMillimeters);
            AddText(panel, title, 44, SystemBlue, 64f);
            AddText(panel, subtitle, 28, OffWhite, 44f);
            return panel;
        }

        /// <summary>A vertical group inside a panel, e.g. one step of a flow that is shown on its own.</summary>
        internal static Transform AddGroup(Transform panel, string name)
        {
            var group = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
            group.transform.SetParent(panel, false);
            var layout = group.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            return group.transform;
        }

        internal static Button AddButton(Transform panel, string label, UnityAction onClick)
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
            return button;
        }

        internal static Text AddText(Transform parent, string value, int size, Color color, float preferredHeight)
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

        /// <summary>A thin track with a fill whose x scale goes 0 to 1 (pivot on the left), so updating it never allocates.</summary>
        static RectTransform AddProgressBar(Transform parent)
        {
            var track = new GameObject("Progress", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            track.transform.SetParent(parent, false);
            var trackImage = track.GetComponent<Image>();
            trackImage.color = ButtonFill;
            trackImage.raycastTarget = false;
            track.GetComponent<LayoutElement>().preferredHeight = 16f;

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(track.transform, false);
            var fillImage = fill.GetComponent<Image>();
            fillImage.color = SystemBlue;
            fillImage.raycastTarget = false;

            var rect = (RectTransform)fill.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.localScale = new Vector3(0f, 1f, 1f);
            return rect;
        }

        internal static void Assign(UnityEngine.Object target, string field, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            var property = serialized.FindProperty(field);
            if (property == null) throw new InvalidOperationException($"{target.GetType().Name} has no field {field}.");
            property.objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
