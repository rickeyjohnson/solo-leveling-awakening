using SLA.Core;
using SLA.Core.Events;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// Puts a world-space menu in front of the player's standing spot at their eye level (CMF-13): the
    /// calibrated standing height once there is one, the live head height before that. It places the menu
    /// once and again when OnSettingsChanged is raised (calibration finished), never every frame, so the
    /// menu is never head-locked (UI-3). Without tracking or calibration the menu stays where it was built.
    /// </summary>
    public class EyeLevelPlacement : MonoBehaviour
    {
        [Tooltip("UI-11: panels sit 1.2-2.0 m away.")]
        [SerializeField] private float _distanceMeters = 1.6f;
        [Tooltip("UI-3: windows sit 10-15 degrees below eye level.")]
        [SerializeField] private float _degreesBelowEyeLevel = 10f;
        [Tooltip("Head heights below this count as untracked (meters).")]
        [SerializeField] private float _minTrackedHeightMeters = StandingHeightCalibration.DefaultMinTrackedHeightMeters;
        [SerializeField] private VoidEventChannel _onSettingsChanged;

        private XROrigin _origin;
        private bool _needsPlacing;

        private void Awake()
        {
            _origin = FindAnyObjectByType<XROrigin>();
        }

        private void OnEnable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised += RequestPlacing;
            }

            RequestPlacing();
        }

        private void OnDisable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised -= RequestPlacing;
            }
        }

        private void RequestPlacing() => _needsPlacing = true;

        // LateUpdate: after PlayerSpawn has moved the rig to this scene's spawn.
        private void LateUpdate()
        {
            if (!_needsPlacing || _origin == null)
            {
                return;
            }

            var source = MenuPlacement.ChooseEyeHeight(
                GameSession.Data.player.standingHeight, _origin.CameraInOriginSpaceHeight, _minTrackedHeightMeters, out float eyeHeight);

            // Untracked: keep the built position and try again next frame, as tracking can start late.
            if (source == EyeHeightSource.None)
            {
                return;
            }

            Place(eyeHeight);
            _needsPlacing = false;
        }

        private void Place(float eyeHeightMeters)
        {
            Transform rig = _origin.Origin.transform;
            Vector3 forward = Vector3.ProjectOnPlane(rig.forward, Vector3.up).normalized;
            Vector3 eye = rig.position + Vector3.up * eyeHeightMeters;
            float drop = MenuPlacement.DropBelowEyeMeters(_distanceMeters, _degreesBelowEyeLevel);

            Vector3 position = eye + forward * _distanceMeters - Vector3.up * drop;
            // A world-space canvas reads from behind its forward, so face away from the eye (tilted up to it).
            transform.SetPositionAndRotation(position, Quaternion.LookRotation(position - eye, Vector3.up));
        }
    }
}
