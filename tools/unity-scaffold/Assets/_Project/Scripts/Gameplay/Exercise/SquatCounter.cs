namespace SLA.Exercise
{
    /// <summary>
    /// Counts squats from headset height, per the GDD's squat detection rules:
    /// down below 80% of standing height, back above 95%, no bowing (head pitch over 45 degrees)
    /// or walking (head over 0.3 m from the start spot), and at least 0.8 s per rep.
    /// Pure logic: feed it samples and it reports reps, so it can be tested against recordings.
    /// </summary>
    public sealed class SquatCounter
    {
        public float DownFraction = 0.80f;
        public float UpFraction = 0.95f;
        public float MaxPitchDownDegrees = 45f;
        public float MaxHorizontalMeters = 0.3f;
        public float MinRepSeconds = 0.8f;

        private bool _isDown;
        private bool _invalid;
        private float _lastStandingTime;
        private float _repStartTime;

        public float StandingHeight { get; private set; }
        public int Count { get; private set; }
        public bool IsCalibrated => StandingHeight > 0f;

        /// <summary>Call while the player stands still (the System says [Stand still.]).</summary>
        public void Calibrate(float standingHeight, float time = 0f)
        {
            StandingHeight = standingHeight;
            Count = 0;
            _isDown = false;
            _invalid = false;
            _lastStandingTime = time;
        }

        /// <summary>
        /// Feed one sample. Returns true on the sample that completes a valid rep.
        /// </summary>
        /// <param name="time">Seconds.</param>
        /// <param name="headHeight">Headset height above the floor, meters.</param>
        /// <param name="pitchDownDegrees">How far the head is tilted down, degrees.</param>
        /// <param name="horizontalOffsetMeters">Head distance from the calibration spot, meters.</param>
        public bool Sample(float time, float headHeight, float pitchDownDegrees, float horizontalOffsetMeters)
        {
            if (!IsCalibrated)
            {
                return false;
            }

            float up = StandingHeight * UpFraction;
            float down = StandingHeight * DownFraction;

            if (headHeight >= up)
            {
                bool counted = false;
                if (_isDown)
                {
                    counted = !_invalid && time - _repStartTime >= MinRepSeconds;
                    if (counted)
                    {
                        Count++;
                    }
                    _isDown = false;
                }

                _invalid = false;
                _lastStandingTime = time;
                return counted;
            }

            if (pitchDownDegrees > MaxPitchDownDegrees || horizontalOffsetMeters > MaxHorizontalMeters)
            {
                _invalid = true;
            }

            if (!_isDown && headHeight < down)
            {
                _isDown = true;
                _repStartTime = _lastStandingTime;
            }

            return false;
        }
    }
}
