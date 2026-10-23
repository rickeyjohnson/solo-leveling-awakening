using NUnit.Framework;
using SLA.Combat;
using SLA.Data;
using UnityEngine;

namespace SLA.Tests
{
    public class DamageCalculatorTests
    {
        private CombatConfig _config;

        [SetUp]
        public void SetUp() => _config = ScriptableObject.CreateInstance<CombatConfig>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_config);

        private static DamageInput Hit(float speed, HitZone zone = HitZone.Torso, float armor = 0f) => new DamageInput
        {
            BaseDamage = 12f,
            HitSpeed = speed,
            Strength = 10,
            Zone = zone,
            Armor = armor,
        };

        [Test]
        public void TapBelowMinimumSpeed_DoesNoDamage()
        {
            Assert.AreEqual(0f, DamageCalculator.Compute(Hit(1.4f), _config));
        }

        [Test]
        public void FullSpeedTorsoHit_MatchesGddExample()
        {
            // 12 base x 1.0 swing x (1 + 0.02 x 10) x 1.0 torso = 14.4
            Assert.AreEqual(14.4f, DamageCalculator.Compute(Hit(5f), _config), 0.001f);
        }

        [Test]
        public void SwingFactor_IsCappedAt1Point2()
        {
            Assert.AreEqual(1.2f, DamageCalculator.SwingFactor(50f, _config), 0.0001f);
        }

        [Test]
        public void Headshot_DoublesDamage()
        {
            float torso = DamageCalculator.Compute(Hit(5f), _config);
            float head = DamageCalculator.Compute(Hit(5f, HitZone.Head), _config);
            Assert.AreEqual(torso * 2f, head, 0.001f);
        }

        [Test]
        public void Armor_IsSubtracted_AndNeverGoesNegative()
        {
            Assert.AreEqual(4.4f, DamageCalculator.Compute(Hit(5f, armor: 10f), _config), 0.001f);
            Assert.AreEqual(0f, DamageCalculator.Compute(Hit(5f, armor: 100f), _config));
        }

        [Test]
        public void StaggeredTargets_IgnoreArmor()
        {
            var input = Hit(5f, armor: 10f);
            input.IgnoreArmor = true;
            Assert.AreEqual(14.4f, DamageCalculator.Compute(input, _config), 0.001f);
        }

        [Test]
        public void ParryWindow_GrowsWithPerception()
        {
            Assert.AreEqual(0.15f, DamageCalculator.ParryWindow(10, _config), 0.0001f);
            Assert.AreEqual(0.19f, DamageCalculator.ParryWindow(20, _config), 0.0001f);
        }

        [Test]
        public void DashCooldown_IsFlooredAt40Percent()
        {
            Assert.AreEqual(4f, DamageCalculator.DashCooldown(10, _config), 0.0001f);
            Assert.AreEqual(1.6f, DamageCalculator.DashCooldown(200, _config), 0.0001f);
        }
    }
}
