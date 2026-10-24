using NUnit.Framework;
using SLA.AI;
using SLA.Combat;
using UnityEngine;

namespace SLA.Tests
{
    public class CombatSupportTests
    {
        [Test]
        public void AttackTokens_AllowOnlyTwoAttackersAtOnce()
        {
            var pool = new AttackTokenPool(2);
            Assert.IsTrue(pool.TryAcquire(1));
            Assert.IsTrue(pool.TryAcquire(2));
            Assert.IsFalse(pool.TryAcquire(3));

            pool.Release(1);
            Assert.IsTrue(pool.TryAcquire(3));
            Assert.AreEqual(2, pool.InUse);
        }

        [Test]
        public void AttackTokens_ReacquiringIsHarmless()
        {
            var pool = new AttackTokenPool(1);
            Assert.IsTrue(pool.TryAcquire(7));
            Assert.IsTrue(pool.TryAcquire(7));
            Assert.AreEqual(1, pool.InUse);
        }

        [Test]
        public void SwingTracker_MeasuresTipTravelInsideTheWindow()
        {
            var swing = new SwingTracker { WindowSeconds = 0.25f };
            swing.AddSample(0.00f, Vector3.zero);
            swing.AddSample(0.10f, new Vector3(0.2f, 0f, 0f));
            swing.AddSample(0.20f, new Vector3(0.4f, 0f, 0f));
            Assert.AreEqual(0.4f, swing.ArcLength(), 0.0001f);
        }

        [Test]
        public void SwingTracker_ForgetsOldSamples()
        {
            var swing = new SwingTracker { WindowSeconds = 0.25f };
            swing.AddSample(0.00f, Vector3.zero);
            swing.AddSample(0.10f, new Vector3(1f, 0f, 0f));
            swing.AddSample(1.00f, new Vector3(1f, 0f, 0f));
            Assert.AreEqual(0f, swing.ArcLength(), 0.0001f);
        }
    }
}
