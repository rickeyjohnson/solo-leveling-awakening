using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SLA.UI
{
    /// <summary>
    /// First-run setup: measure standing height while the System says [Stand still.] (CMF-13, QST-8), then
    /// pick a comfort preset (and optionally left-handed mode, CMF-8) and go to the title screen. Leaving
    /// setup saves (GameStateRules). Changes go to GameSession.Data.settings and OnSettingsChanged, so the
    /// rig applies them at once. The room scan check joins later (CMF-14).
    /// </summary>
    public class FirstRunSetupMenu : MonoBehaviour
    {
        private const string LeftHandedOn = "Left-handed: on";
        private const string LeftHandedOff = "Left-handed: off";

        [SerializeField] private HeightCalibrator _calibrator;
        [Tooltip("Shown while measuring: [Stand still.] and a progress bar.")]
        [SerializeField] private GameObject _calibrationStep;
        [Tooltip("Shown after measuring: the preset buttons.")]
        [SerializeField] private GameObject _presetStep;
        [Tooltip("Scaled from 0 to 1 across while measuring (pivot on its left edge).")]
        [SerializeField] private RectTransform _progressFill;
        [SerializeField] private Text _leftHandedLabel;
        [SerializeField] private VoidEventChannel _onSettingsChanged;

        private void OnEnable()
        {
            if (_calibrator != null)
            {
                _calibrator.Finished += OnCalibrated;
            }
        }

        private void OnDisable()
        {
            if (_calibrator != null)
            {
                _calibrator.Finished -= OnCalibrated;
            }
        }

        private void Start()
        {
            UpdateLeftHandedLabel();
            MeasureAgain();
        }

        private void Update()
        {
            if (_progressFill != null && _calibrator != null && _calibrator.IsRunning)
            {
                _progressFill.localScale = new Vector3(_calibrator.Progress01, 1f, 1f);
            }
        }

        /// <summary>Shows [Stand still.] and measures standing height (again).</summary>
        public void MeasureAgain()
        {
            if (_calibrator == null)
            {
                Debug.LogError("[FirstRunSetupMenu] No HeightCalibrator assigned; skipping height calibration.", this);
                ShowStep(false);
                return;
            }

            ShowStep(true);
            if (_progressFill != null)
            {
                _progressFill.localScale = new Vector3(0f, 1f, 1f);
            }
            _calibrator.Begin();
        }

        public void ChooseRecommended() => ChooseAndFinish(ComfortPresetNames.Recommended);

        public void ChooseComfortable() => ChooseAndFinish(ComfortPresetNames.Comfortable);

        public void ChooseAdvanced() => ChooseAndFinish(ComfortPresetNames.Advanced);

        /// <summary>CMF-8: swaps the move and turn sticks; the rig changes at once.</summary>
        public void ToggleLeftHanded()
        {
            var settings = GameSession.Data.settings;
            settings.leftHanded = !settings.leftHanded;
            RaiseSettingsChanged();
            UpdateLeftHandedLabel();
        }

        // Whether or not a height was stored, the player can go on (and measure again from the preset step).
        private void OnCalibrated(bool measured) => ShowStep(false);

        private void ShowStep(bool calibrating)
        {
            if (_calibrationStep != null) _calibrationStep.SetActive(calibrating);
            if (_presetStep != null) _presetStep.SetActive(!calibrating);
        }

        private void UpdateLeftHandedLabel()
        {
            if (_leftHandedLabel != null)
            {
                _leftHandedLabel.text = GameSession.Data.settings.leftHanded ? LeftHandedOn : LeftHandedOff;
            }
        }

        private void ChooseAndFinish(string preset)
        {
            // Set before the state change: leaving setup saves, and the save must include the choice.
            GameSession.Data.settings.comfortPreset = preset;
            RaiseSettingsChanged();
            GameFlow.GoTo(GameState.Title, SceneNames.Title);
        }

        private void RaiseSettingsChanged()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raise();
            }
        }
    }
}
