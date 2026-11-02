using SLA.Core;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace SLA.Player
{
    /// <summary>
    /// A short fade to black around each teleport: the teleport waits (TeleportationProvider.delayTime)
    /// while the view fades out, happens on black, then the view fades back in. Uses the rig's
    /// ScreenFader overlay layer, so it never fights a scene-load fade. Duration from the comfort preset.
    /// </summary>
    [AddComponentMenu("SLA/Teleport Fade")]
    public class TeleportFade : MonoBehaviour
    {
        [SerializeField] private TeleportationProvider _teleportation;
        [SerializeField] private ScreenFader _fader;

        private float _fadeSeconds;
        private int _fadeInAfterFrame = -1;

        /// <summary>Fade out (and in) time; also the teleport's delay, so it lands on black. 0 = no fade.</summary>
        public float FadeSeconds
        {
            get => _fadeSeconds;
            set
            {
                _fadeSeconds = Mathf.Max(0f, value);
                if (_teleportation != null)
                {
                    _teleportation.delayTime = _fadeSeconds;
                }
            }
        }

        private void OnEnable()
        {
            if (_teleportation == null || _fader == null)
            {
                Debug.LogError("[TeleportFade] Teleportation provider or screen fader not assigned; teleports won't fade.", this);
                return;
            }

            _teleportation.locomotionStateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            if (_teleportation != null)
            {
                _teleportation.locomotionStateChanged -= OnStateChanged;
            }

            // Never leave the view black if the rig changes mid-teleport.
            _fadeInAfterFrame = -1;
            if (_fader != null)
            {
                _fader.FadeOverlay(0f, _fadeSeconds);
            }
        }

        private void Update()
        {
            // The provider runs before this script, so the landing frame itself is skipped by frame number.
            if (_fadeInAfterFrame < 0 || Time.frameCount <= _fadeInAfterFrame)
            {
                return;
            }

            _fadeInAfterFrame = -1;
            _fader.FadeOverlay(0f, _fadeSeconds);
        }

        private void OnStateChanged(LocomotionProvider provider, LocomotionState state)
        {
            switch (state)
            {
                case LocomotionState.Preparing:
                case LocomotionState.Moving:
                    _fadeInAfterFrame = -1;
                    _fader.FadeOverlay(1f, _fadeSeconds);
                    break;
                case LocomotionState.Ended:
                    // Ended comes in the frame the move is applied; clearing then would show that frame
                    // part-faded, so the first frame at the new spot stays black.
                    _fadeInAfterFrame = Time.frameCount;
                    break;
            }
        }
    }
}
