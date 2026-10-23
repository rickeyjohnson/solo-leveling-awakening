using NUnit.Framework;
using SLA.Data;
using SLA.Progression;
using UnityEngine;

namespace SLA.Tests
{
    public class ProgressionTests
    {
        private ProgressionConfig _config;

        [SetUp]
        public void SetUp() => _config = ScriptableObject.CreateInstance<ProgressionConfig>();

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_config);

        [TestCase(1, 50)]
        [TestCase(10, 1991)]
        [TestCase(30, 11544)]
        [TestCase(49, 25310)]
        public void XpToNext_MatchesTheGddCurve(int level, int expected)
        {
            Assert.AreEqual(expected, XpCurve.XpToNext(level, _config));
        }

        [Test]
        public void Effort_DropsAtEachRankUp()
        {
            float lastEffortAtE = XpCurve.EffortToNext(9, Rank.E, _config);
            float firstEffortAtD = XpCurve.EffortToNext(10, Rank.D, _config);
            Assert.Less(firstEffortAtD, lastEffortAtE);
        }

        [TestCase(1, Rank.E)]
        [TestCase(19, Rank.D)]
        [TestCase(20, Rank.C)]
        [TestCase(45, Rank.A)]
        [TestCase(50, Rank.S)]
        public void RankForLevel_FollowsTheMainPath(int level, Rank expected)
        {
            Assert.AreEqual(expected, XpCurve.RankForLevel(level));
        }

        [Test]
        public void AddXp_LevelsUpAndGrantsFivePointsPerLevel()
        {
            var stats = new PlayerStats();
            int gained = stats.AddXp(50 + 152, _config); // exactly level 1->2 and 2->3
            Assert.AreEqual(2, gained);
            Assert.AreEqual(3, stats.level);
            Assert.AreEqual(0, stats.xp);
            Assert.AreEqual(10, stats.unspentPoints);
            Assert.AreEqual(0, stats.skillPoints);
        }

        [Test]
        public void SkillPoints_TotalFortyByTheLevelCap()
        {
            var stats = new PlayerStats();
            stats.AddXp(int.MaxValue / 2, _config);
            Assert.AreEqual(50, stats.level);
            Assert.AreEqual(40, stats.skillPoints);
            Assert.AreEqual(0, stats.xp);
        }

        [Test]
        public void TryAllocate_SpendsPointsOnlyWhenAvailable()
        {
            var stats = new PlayerStats { unspentPoints = 1 };
            Assert.IsTrue(stats.TryAllocate(StatType.Strength));
            Assert.IsFalse(stats.TryAllocate(StatType.Strength));
            Assert.AreEqual(11, stats.strength);
        }

        [Test]
        public void Vitality_AddsTenHpPerPoint()
        {
            var stats = new PlayerStats { vitality = 15 };
            Assert.AreEqual(150, stats.MaxHp(_config));
        }

        [Test]
        public void EnemyXp_UsesTheGateRankMultiplier()
        {
            Assert.AreEqual(20, XpCurve.EnemyXp(20, Rank.E, _config));
            Assert.AreEqual(160, XpCurve.EnemyXp(20, Rank.C, _config));
        }
    }
}
