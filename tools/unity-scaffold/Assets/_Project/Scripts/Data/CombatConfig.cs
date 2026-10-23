using UnityEngine;

namespace SLA.Data
{
    /// <summary>
    /// Combat tuning from the GDD's Combat design section. Damage =
    /// base x swing factor x (1 + strengthDamagePerPoint x STR) x zone x crit - armor.
    /// </summary>
    [CreateAssetMenu(menuName = "SLA/Config/Combat Config")]
    public class CombatConfig : ScriptableObject
    {
        [Header("Swing factor (relative hit speed, m/s)")]
        [Tooltip("Below this speed a hit does nothing (a tap).")]
        public float minHitSpeed = 1.5f;
        [Tooltip("Speed at which the swing factor reaches 1.0.")]
        public float fullHitSpeed = 5f;
        public float swingFactorCap = 1.2f;

        [Header("Damage")]
        public float strengthDamagePerPoint = 0.02f;
        public float headMultiplier = 2f;
        public float torsoMultiplier = 1f;
        public float limbMultiplier = 0.6f;
        public float weakPointMultiplier = 2.5f;
        public float critMultiplier = 1.5f;
        [Tooltip("Hits with the flat or pommel deal this fraction as blunt damage.")]
        public float bluntFraction = 0.3f;

        [Header("Anti-cheese")]
        [Tooltip("The weapon tip must travel at least this far (m)...")]
        public float minSwingArc = 0.3f;
        [Tooltip("...within this many seconds for a hit to count.")]
        public float swingArcWindow = 0.25f;
        [Tooltip("A weapon can damage the same target at most once per this many seconds.")]
        public float perTargetCooldown = 0.25f;
        [Tooltip("Follow strength drops for this long after a hit, so blades drag instead of passing through.")]
        public float contactDragSeconds = 0.1f;

        [Header("Defense")]
        public float blockDamageReduction = 0.8f;
        public float blockFatigueCost = 10f;
        public float parryWindowBase = 0.15f;
        public float parryWindowPerPerception = 0.004f;
        public float parryMinSpeed = 2f;
        public float staggerSeconds = 1.5f;

        [Header("Dash")]
        public float dashDistance = 3f;
        public float dashCooldown = 4f;
        public float dashFatigueCost = 15f;
        public float dashCooldownReductionPerAgility = 0.01f;
        public float dashMinCooldownFraction = 0.4f;
        [Tooltip("Controller lunge that triggers Dash: distance (m) within lungeSeconds.")]
        public float lungeDistance = 0.25f;
        public float lungeSeconds = 0.2f;

        [Header("Crowd control")]
        public int maxAttackTokens = 2;

        public float ZoneMultiplier(HitZone zone)
        {
            switch (zone)
            {
                case HitZone.Head: return headMultiplier;
                case HitZone.Limb: return limbMultiplier;
                case HitZone.WeakPoint: return weakPointMultiplier;
                default: return torsoMultiplier;
            }
        }
    }
}
