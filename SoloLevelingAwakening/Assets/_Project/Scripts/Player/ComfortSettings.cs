using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace SLA.Player
{
    /// <summary>
    /// Applies the player's comfort settings (GameSession.Data.settings: comfort preset and left-handed)
    /// to the rig's locomotion, live, without reloading anything: on enable, whenever OnSettingsChanged is
    /// raised, and whenever a <see cref="StickLocomotionLock"/> appears or goes (MRH-13). Settings screens
    /// write GameSession.Data.settings and raise OnSettingsChanged; they never reference the rig.
    /// Lives on the PlayerRig prefab root.
    /// </summary>
    [AddComponentMenu("SLA/Comfort Settings")]
    public class ComfortSettings : MonoBehaviour
    {
        [Header("Presets")]
        [SerializeField] private ComfortPreset[] _presets;
        [Tooltip("Used when the saved preset name is empty or unknown.")]
        [SerializeField] private ComfortPreset _defaultPreset;
        [SerializeField] private VoidEventChannel _onSettingsChanged;

        [Header("Rig")]
        [SerializeField] private QuantizedMoveProvider _move;
        [SerializeField] private SnapTurnProvider _snapTurn;
        [SerializeField] private ContinuousTurnProvider _smoothTurn;
        [SerializeField] private TeleportationProvider _teleportation;
        [SerializeField] private TeleportFade _teleportFade;
        [SerializeField] private GravityProvider _gravity;
        [SerializeField] private TunnelingVignetteController _vignette;

        [Header("Hands (XRI Starter Assets input managers)")]
        [SerializeField] private ControllerInputActionManager _leftHand;
        [SerializeField] private ControllerInputActionManager _rightHand;
        [SerializeField] private GameObject _leftTeleportInteractor;
        [SerializeField] private GameObject _rightTeleportInteractor;

        private NearFarInteractor _leftNearFar;
        private NearFarInteractor _rightNearFar;

        /// <summary>The preset applied last.</summary>
        public ComfortPreset Current { get; private set; }

        private void Awake()
        {
            if (_move == null || _snapTurn == null || _smoothTurn == null || _teleportation == null || _teleportFade == null
                || _gravity == null || _vignette == null || _leftHand == null || _rightHand == null
                || _leftTeleportInteractor == null || _rightTeleportInteractor == null)
            {
                Debug.LogError("[ComfortSettings] A rig reference is missing; rebuild the PlayerRig prefab (Tools > SLA > Set Up Player Rig).", this);
                enabled = false;
                return;
            }

            _leftNearFar = _leftHand.GetComponentInChildren<NearFarInteractor>(true);
            _rightNearFar = _rightHand.GetComponentInChildren<NearFarInteractor>(true);
        }

        private void OnEnable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised += Apply;
            }
            else
            {
                Debug.LogError("[ComfortSettings] No OnSettingsChanged channel assigned; settings changes won't apply live.", this);
            }

            StickLocomotionLock.Changed += Apply;
            Apply();
        }

        private void OnDisable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised -= Apply;
            }

            StickLocomotionLock.Changed -= Apply;
        }

        /// <summary>Re-reads the settings and lock state and applies them to the rig.</summary>
        public void Apply()
        {
            var settings = GameSession.Data.settings;
            var preset = ComfortPresetLookup.Find(_presets, settings.comfortPreset, _defaultPreset);
            if (preset == null)
            {
                Debug.LogError("[ComfortSettings] No comfort presets assigned.", this);
                return;
            }

            Current = preset;
            var plan = LocomotionPlan.For(preset.movement, preset.turning, preset.vignette, settings.leftHanded, StickLocomotionLock.IsLocked);

            // CMF-2: speeds only ever come from the preset.
            _move.SetSpeeds(preset.walkSpeedMetersPerSecond, preset.sprintSpeedMetersPerSecond);
            _move.SprintOnLeftHand = plan.SprintOnLeftHand;
            _move.enabled = plan.MoveEnabled;

            _snapTurn.turnAmount = preset.snapTurnDegrees;
            _snapTurn.enabled = plan.SnapTurnEnabled;
            _smoothTurn.turnSpeed = preset.smoothTurnDegreesPerSecond;
            _smoothTurn.enabled = plan.SmoothTurnEnabled;

            _teleportFade.FadeSeconds = preset.teleportFadeSeconds;
            _teleportation.enabled = plan.TeleportEnabled;
            _gravity.useGravity = plan.GravityEnabled;

            ApplyHands(plan);
            ApplyVignette(preset, plan.VignetteEnabled);
        }

        private void ApplyHands(in LocomotionPlan plan)
        {
            // CMF-8: each hand's stick either moves, or teleports (forward) and turns (sideways).
            _leftHand.smoothMotionEnabled = plan.LeftHand.SmoothMotion;
            _leftHand.smoothTurnEnabled = plan.LeftHand.SmoothTurn;
            _rightHand.smoothMotionEnabled = plan.RightHand.SmoothMotion;
            _rightHand.smoothTurnEnabled = plan.RightHand.SmoothTurn;

            // Locked: the managers stop switching on teleport arcs, and any arc already up goes away so it
            // can't queue a teleport for when the lock lifts. Re-enabling a manager restores its actions.
            _leftHand.enabled = plan.HandInputEnabled;
            _rightHand.enabled = plan.HandInputEnabled;
            if (!plan.HandInputEnabled)
            {
                _leftTeleportInteractor.SetActive(false);
                _rightTeleportInteractor.SetActive(false);

                // Aiming a teleport hides the hand's UI/grab ray until the aim is cancelled; a lock that lands
                // mid-aim skips that cancel, so bring the rays back here.
                if (_leftNearFar != null) _leftNearFar.gameObject.SetActive(true);
                if (_rightNearFar != null) _rightNearFar.gameObject.SetActive(true);
            }
        }

        private void ApplyVignette(ComfortPreset preset, bool on)
        {
            var parameters = _vignette.defaultParameters;
            parameters.apertureSize = preset.vignetteApertureSize;
            parameters.featheringEffect = preset.vignetteFeathering;
            parameters.easeInTime = preset.vignetteEaseInSeconds;
            parameters.easeOutTime = preset.vignetteEaseOutSeconds;
            if (_vignette.gameObject.activeSelf != on)
            {
                _vignette.gameObject.SetActive(on);
            }
        }
    }
}
