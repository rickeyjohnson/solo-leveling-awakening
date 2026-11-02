using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// QST-8 / CMF-13: the player's standing head height H₀, averaged over a time window while the
    /// System says [Stand still.]. Time-weighted, so a hitch doesn't skew it, and clipped to the window.
    /// Frames where the head reads lower than a minimum are the headset not tracking (it reports the head
    /// on the floor): they still use up the window but don't count, and if no frame was tracked there is
    /// no height. Pure, so it's unit-tested.
    /// </summary>
    public sealed class StandingHeightCalibration
    {
        /// <summary>Default for "the headset is tracking": a head lower than this is treated as untracked (meters).</summary>
        public const float DefaultMinTrackedHeightMeters = 0.5f;

        private readonly float _windowSeconds;
        private readonly float _minTrackedHeightMeters;
        private float _elapsedSeconds;
        private float _trackedSeconds;
        private float _weightedSum;

        public StandingHeightCalibration(float windowSeconds, float minTrackedHeightMeters)
        {
            _windowSeconds = Mathf.Max(0f, windowSeconds);
            _minTrackedHeightMeters = minTrackedHeightMeters;
        }

        public bool IsRunning { get; private set; }

        /// <summary>True once a measurement started with <see cref="Begin"/> has covered the whole window.</summary>
        public bool IsDone { get; private set; }

        public float Progress01 => IsDone ? 1f : _windowSeconds > 0f ? Mathf.Clamp01(_elapsedSeconds / _windowSeconds) : 0f;

        /// <summary>Average of the tracked frames so far; 0 if none were tracked.</summary>
        public float AverageHeightMeters => _trackedSeconds > 0f ? _weightedSum / _trackedSeconds : 0f;

        /// <summary>Starts (or restarts) a measurement, dropping any earlier samples.</summary>
        public void Begin()
        {
            _elapsedSeconds = 0f;
            _trackedSeconds = 0f;
            _weightedSum = 0f;
            IsDone = false;
            IsRunning = true;
            if (_windowSeconds <= 0f)
            {
                Finish();
            }
        }

        /// <summary>Adds one frame's head height (meters above the floor). Ignored unless running.</summary>
        public void Sample(float headHeightMeters, float deltaSeconds)
        {
            if (!IsRunning || deltaSeconds <= 0f)
            {
                return;
            }

            float remainingSeconds = _windowSeconds - _elapsedSeconds;
            float weight = Mathf.Min(deltaSeconds, remainingSeconds);
            _elapsedSeconds += weight;
            if (headHeightMeters >= _minTrackedHeightMeters)
            {
                _weightedSum += headHeightMeters * weight;
                _trackedSeconds += weight;
            }

            // Decided on the unclipped frame time, so float rounding in the sum can't leave it one sample short.
            if (deltaSeconds >= remainingSeconds)
            {
                Finish();
            }
        }

        /// <summary>The measured standing height, if the measurement is done and the headset was tracking.</summary>
        public bool TryGetStandingHeight(out float meters)
        {
            meters = AverageHeightMeters;
            return IsDone && _trackedSeconds > 0f;
        }

        private void Finish()
        {
            IsRunning = false;
            IsDone = true;
        }
    }
}
