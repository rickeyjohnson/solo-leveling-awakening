using NUnit.Framework;
using SLA.Exercise;

namespace SLA.Tests
{
    public class SquatCounterTests
    {
        private SquatCounter _counter;

        [SetUp]
        public void SetUp()
        {
            _counter = new SquatCounter();
            _counter.Calibrate(1.70f);
        }

        // (time, head height) pairs with a level, still head.
        private void Feed(params (float t, float h)[] samples)
        {
            foreach (var (t, h) in samples)
            {
                _counter.Sample(t, h, 0f, 0f);
            }
        }

        [Test]
        public void AFullSquat_Counts()
        {
            Feed((0f, 1.70f), (0.3f, 1.50f), (0.6f, 1.30f), (0.9f, 1.50f), (1.2f, 1.65f));
            Assert.AreEqual(1, _counter.Count);
        }

        [Test]
        public void SmallBobs_DoNotCount()
        {
            Feed((0f, 1.70f), (0.5f, 1.60f), (1.0f, 1.70f), (1.5f, 1.45f), (2.0f, 1.70f));
            Assert.AreEqual(0, _counter.Count);
        }

        [Test]
        public void TooFastReps_AreRejected()
        {
            Feed((0f, 1.70f), (0.2f, 1.30f), (0.4f, 1.65f));
            Assert.AreEqual(0, _counter.Count);
        }

        [Test]
        public void BowingInsteadOfSquatting_IsRejected()
        {
            _counter.Sample(0f, 1.70f, 0f, 0f);
            _counter.Sample(0.5f, 1.30f, 60f, 0f); // head pitched 60 degrees down
            _counter.Sample(1.2f, 1.65f, 0f, 0f);
            Assert.AreEqual(0, _counter.Count);
        }

        [Test]
        public void WalkingAway_IsRejected()
        {
            _counter.Sample(0f, 1.70f, 0f, 0f);
            _counter.Sample(0.5f, 1.30f, 0f, 0.5f); // 0.5 m from the start spot
            _counter.Sample(1.2f, 1.65f, 0f, 0f);
            Assert.AreEqual(0, _counter.Count);
        }

        [Test]
        public void ConsecutiveReps_AllCount()
        {
            for (int i = 0; i < 5; i++)
            {
                float t = i * 2f;
                Feed((t, 1.70f), (t + 0.6f, 1.30f), (t + 1.2f, 1.68f));
            }
            Assert.AreEqual(5, _counter.Count);
        }

        [Test]
        public void Uncalibrated_CountsNothing()
        {
            var fresh = new SquatCounter();
            Assert.IsFalse(fresh.Sample(1f, 1.0f, 0f, 0f));
            Assert.AreEqual(0, fresh.Count);
        }
    }
}
