using SLA.Data;
using UnityEngine;

namespace SLA.Combat
{
    public readonly struct HitInfo
    {
        public readonly float Damage;
        public readonly Vector3 Point;
        public readonly Vector3 Normal;
        public readonly HitZone Zone;
        public readonly float HitSpeed;

        public HitInfo(float damage, Vector3 point, Vector3 normal, HitZone zone, float hitSpeed)
        {
            Damage = damage;
            Point = point;
            Normal = normal;
            Zone = zone;
            HitSpeed = hitSpeed;
        }
    }

    /// <summary>Anything a weapon can hurt: enemies, the player, destructible props.</summary>
    public interface IDamageable
    {
        float Armor { get; }
        bool IsStaggered { get; }
        bool IsDead { get; }
        void ApplyDamage(in HitInfo hit);
    }
}
