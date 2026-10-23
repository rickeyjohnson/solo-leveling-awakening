using SLA.Data;
using UnityEngine;

namespace SLA.Progression
{
    /// <summary>XP math from the GDD: XP to next level = round(50 x L^1.6), scaled by the gate rank's multiplier.</summary>
    public static class XpCurve
    {
        public static int XpToNext(int level, ProgressionConfig config)
        {
            if (level < 1)
            {
                level = 1;
            }
            return Mathf.RoundToInt(config.xpBase * Mathf.Pow(level, config.xpExponent));
        }

        /// <summary>XP an enemy gives: its base XP times its gate rank multiplier.</summary>
        public static int EnemyXp(int baseXp, Rank gateRank, ProgressionConfig config)
        {
            return Mathf.RoundToInt(baseXp * config.RankMultiplier(gateRank));
        }

        /// <summary>
        /// Effort for a level in base-XP units: XP needed divided by the multiplier of the rank the
        /// player is at. Drops at every rank-up by design (the sawtooth in the GDD's XP chart).
        /// </summary>
        public static float EffortToNext(int level, Rank playerRank, ProgressionConfig config)
        {
            return XpToNext(level, config) / config.RankMultiplier(playerRank);
        }

        /// <summary>The rank a level belongs to on the main path: E 1-9, D 10-19 ... A 40-49, S 50.</summary>
        public static Rank RankForLevel(int level)
        {
            if (level >= 50) return Rank.S;
            if (level >= 40) return Rank.A;
            if (level >= 30) return Rank.B;
            if (level >= 20) return Rank.C;
            if (level >= 10) return Rank.D;
            return Rank.E;
        }
    }
}
