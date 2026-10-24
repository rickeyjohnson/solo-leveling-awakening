using UnityEngine;

namespace SLA.Data
{
    /// <summary>
    /// Leveling numbers from the GDD's Player character &amp; progression section.
    /// XP to next level = round(xpBase * level ^ xpExponent).
    /// </summary>
    [CreateAssetMenu(menuName = "SLA/Config/Progression Config")]
    public class ProgressionConfig : ScriptableObject
    {
        [Header("Experience curve")]
        public float xpBase = 50f;
        public float xpExponent = 1.6f;
        public int levelCap = 50;

        [Tooltip("Enemy XP multiplier by gate rank: E, D, C, B, A, S.")]
        public float[] rankXpMultipliers = { 1f, 3f, 8f, 20f, 45f, 100f };

        [Header("Points")]
        public int statPointsPerLevel = 5;
        [Tooltip("Skill points are earned for each level gained above this one (40 points by Level 50).")]
        public int skillPointsAfterLevel = 10;
        public int skillPointsPerLevel = 1;

        [Header("Stats (every stat starts at baseStat)")]
        public int baseStat = 10;
        public int baseHp = 100;
        public int hpPerVitality = 10;
        public int baseMp = 20;
        public int mpPerIntelligence = 2;

        public float RankMultiplier(Rank rank)
        {
            int i = (int)rank;
            return i >= 0 && i < rankXpMultipliers.Length ? rankXpMultipliers[i] : 1f;
        }
    }
}
