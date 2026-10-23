using SLA.Data;
using UnityEngine;

namespace SLA.Combat
{
    public struct DamageInput
    {
        public float BaseDamage;
        /// <summary>Relative speed between weapon and target at contact, m/s.</summary>
        public float HitSpeed;
        public int Strength;
        public HitZone Zone;
        public bool IsCritical;
        public float Armor;
        /// <summary>True when the target is staggered (after a parry): hits ignore armor.</summary>
        public bool IgnoreArmor;
    }

    /// <summary>
    /// The GDD damage formula as pure functions, so it can be unit-tested and tuned in one place:
    /// Damage = base x swing factor x (1 + 0.02 x STR) x zone x crit - armor.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>0 below minHitSpeed (a tap does nothing), linear to 1.0 at fullHitSpeed, capped.</summary>
        public static float SwingFactor(float hitSpeed, CombatConfig config)
        {
            float range = Mathf.Max(0.01f, config.fullHitSpeed - config.minHitSpeed);
            return Mathf.Clamp((hitSpeed - config.minHitSpeed) / range, 0f, config.swingFactorCap);
        }

        public static float Compute(in DamageInput input, CombatConfig config)
        {
            float swing = SwingFactor(input.HitSpeed, config);
            if (swing <= 0f)
            {
                return 0f;
            }

            float raw = input.BaseDamage
                        * swing
                        * (1f + config.strengthDamagePerPoint * input.Strength)
                        * config.ZoneMultiplier(input.Zone)
                        * (input.IsCritical ? config.critMultiplier : 1f);

            float armor = input.IgnoreArmor ? 0f : input.Armor;
            return Mathf.Max(0f, raw - armor);
        }

        /// <summary>Perfect-parry window in seconds: base + per point of Perception above the starting 10.</summary>
        public static float ParryWindow(int perception, CombatConfig config, int baseStat = 10)
        {
            return config.parryWindowBase + config.parryWindowPerPerception * Mathf.Max(0, perception - baseStat);
        }

        /// <summary>Dash cooldown after Agility: -1% per point above 10, floored at 40% of base.</summary>
        public static float DashCooldown(int agility, CombatConfig config, int baseStat = 10)
        {
            float fraction = 1f - config.dashCooldownReductionPerAgility * Mathf.Max(0, agility - baseStat);
            return config.dashCooldown * Mathf.Max(config.dashMinCooldownFraction, fraction);
        }
    }
}
