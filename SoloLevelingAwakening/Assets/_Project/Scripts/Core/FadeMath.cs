using UnityEngine;

namespace SLA.Core
{
    /// <summary>Screen-fade timing, kept pure so it's unit-tested.</summary>
    public static class FadeMath
    {
        /// <summary>
        /// Moves <paramref name="current"/> alpha toward <paramref name="target"/> so a full 0-to-1 fade
        /// takes <paramref name="durationSeconds"/>. A duration of 0 or less jumps straight to the target.
        /// </summary>
        public static float Step(float current, float target, float durationSeconds, float deltaSeconds)
        {
            if (durationSeconds <= 0f)
            {
                return target;
            }

            return Mathf.MoveTowards(current, target, deltaSeconds / durationSeconds);
        }
    }
}
