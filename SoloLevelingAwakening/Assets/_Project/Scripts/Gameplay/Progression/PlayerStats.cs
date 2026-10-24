using System;
using SLA.Data;
using UnityEngine;

namespace SLA.Progression
{
    /// <summary>
    /// The player's level, XP, points and five stats. Plain C# so it's unit-testable and maps
    /// straight onto the save file.
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        public int level = 1;
        public int xp;
        public int unspentPoints;
        public int skillPoints;
        public int strength = 10;
        public int vitality = 10;
        public int agility = 10;
        public int perception = 10;
        public int intelligence = 10;

        public int Get(StatType stat)
        {
            switch (stat)
            {
                case StatType.Strength: return strength;
                case StatType.Vitality: return vitality;
                case StatType.Agility: return agility;
                case StatType.Perception: return perception;
                default: return intelligence;
            }
        }

        public bool TryAllocate(StatType stat)
        {
            if (unspentPoints <= 0)
            {
                return false;
            }

            switch (stat)
            {
                case StatType.Strength: strength++; break;
                case StatType.Vitality: vitality++; break;
                case StatType.Agility: agility++; break;
                case StatType.Perception: perception++; break;
                default: intelligence++; break;
            }
            unspentPoints--;
            return true;
        }

        /// <summary>Adds XP and applies any level-ups. Returns how many levels were gained.</summary>
        public int AddXp(int amount, ProgressionConfig config)
        {
            if (amount <= 0 || level >= config.levelCap)
            {
                return 0;
            }

            xp += amount;
            int gained = 0;
            while (level < config.levelCap)
            {
                int needed = XpCurve.XpToNext(level, config);
                if (xp < needed)
                {
                    break;
                }

                xp -= needed;
                level++;
                gained++;
                unspentPoints += config.statPointsPerLevel;
                if (level > config.skillPointsAfterLevel)
                {
                    skillPoints += config.skillPointsPerLevel;
                }
            }

            if (level >= config.levelCap)
            {
                xp = 0;
            }
            return gained;
        }

        public int MaxHp(ProgressionConfig config) =>
            config.baseHp + Mathf.Max(0, vitality - config.baseStat) * config.hpPerVitality;

        public int MaxMp(ProgressionConfig config) =>
            config.baseMp + Mathf.Max(0, intelligence - config.baseStat) * config.mpPerIntelligence;
    }
}
