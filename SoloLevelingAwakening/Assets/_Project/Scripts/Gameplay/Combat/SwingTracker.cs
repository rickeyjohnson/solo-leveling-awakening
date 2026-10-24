using System.Collections.Generic;
using UnityEngine;

namespace SLA.Combat
{
    /// <summary>
    /// Tracks how far a weapon tip has travelled recently. Anti-cheese rule 1: a hit only counts
    /// if the tip moved at least minSwingArc within swingArcWindow, which stops wrist-waggling.
    /// </summary>
    public sealed class SwingTracker
    {
        private readonly Queue<Sample> _samples = new Queue<Sample>(64);

        public float WindowSeconds { get; set; } = 0.25f;

        public void AddSample(float time, Vector3 tipPosition)
        {
            _samples.Enqueue(new Sample(time, tipPosition));
            while (_samples.Count > 0 && time - _samples.Peek().Time > WindowSeconds)
            {
                _samples.Dequeue();
            }
        }

        /// <summary>Path length of the tip over the window, in meters.</summary>
        public float ArcLength()
        {
            float length = 0f;
            bool first = true;
            Vector3 previous = default;
            foreach (var sample in _samples)
            {
                if (!first)
                {
                    length += Vector3.Distance(previous, sample.Position);
                }
                previous = sample.Position;
                first = false;
            }
            return length;
        }

        public void Clear() => _samples.Clear();

        private readonly struct Sample
        {
            public readonly float Time;
            public readonly Vector3 Position;

            public Sample(float time, Vector3 position)
            {
                Time = time;
                Position = position;
            }
        }
    }
}
