using System;
using SLA.Core;
using SLA.Core.Events;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// Measures the player's standing height in Setup (CMF-13, QST-8): averages the head's height above the
    /// floor (camera height in XR Origin space) over the calibration window, stores it in
    /// GameSession.Data.player.standingHeight and raises OnSettingsChanged so eye-level menus move. If the
    /// headset isn't tracking (Editor without a headset), it keeps the previous height and logs a warning.
    /// </summary>
    public class HeightCalibrator : MonoBehaviour
    {
        [Tooltip("QST-8: how long the player stands still, in seconds.")]
        [SerializeField] private float _windowSeconds = 2f;
        [Tooltip("An average head height below this means the headset isn't tracking (meters).")]
        [SerializeField] private float _minTrackedHeightMeters = StandingHeightCalibration.DefaultMinTrackedHeightMeters;
        [SerializeField] private VoidEventChannel _onSettingsChanged;

        private StandingHeightCalibration _calibration;
        private XROrigin _origin;

        /// <summary>Raised when a measurement ends: true if a height was stored, false if there was no tracking.</summary>
        public event Action<bool> Finished;

        public bool IsRunning => _calibration != null && _calibration.IsRunning;

        public float Progress01 => _calibration != null ? _calibration.Progress01 : 0f;

        private void Awake()
        {
            _calibration = new StandingHeightCalibration(_windowSeconds, _minTrackedHeightMeters);
            _origin = FindAnyObjectByType<XROrigin>();
        }

        /// <summary>Starts (or restarts) a measurement. The System should be saying [Stand still.].</summary>
        public void Begin()
        {
            if (_origin == null)
            {
                _origin = FindAnyObjectByType<XROrigin>();
            }

            _calibration.Begin();

            // A zero-length window is done at once, and Update only finishes running measurements.
            if (_calibration.IsDone)
            {
                Finish();
            }
        }

        private void Update()
        {
            if (!_calibration.IsRunning)
            {
                return;
            }

            // An origin-less scene (a sandbox without the rig) measures nothing, which counts as untracked.
            float head = _origin != null ? _origin.CameraInOriginSpaceHeight : 0f;
            _calibration.Sample(head, Time.unscaledDeltaTime);
            if (_calibration.IsDone)
            {
                Finish();
            }
        }

        private void Finish()
        {
            var player = GameSession.Data.player;
            bool measured = _calibration.TryGetStandingHeight(out float height);
            if (measured)
            {
                player.standingHeight = height;
                if (_onSettingsChanged != null)
                {
                    _onSettingsChanged.Raise();
                }
            }
            else
            {
                Debug.LogWarning($"[HeightCalibrator] No head tracking (average head height {_calibration.AverageHeightMeters:0.00} m); kept the standing height at {player.standingHeight:0.00} m.");
            }

            Finished?.Invoke(measured);
        }
    }
}
