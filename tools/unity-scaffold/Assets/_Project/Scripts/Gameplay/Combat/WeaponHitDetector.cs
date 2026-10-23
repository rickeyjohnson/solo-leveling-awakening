using System;
using System.Collections.Generic;
using SLA.Data;
using UnityEngine;

namespace SLA.Combat
{
    /// <summary>
    /// Goes on a held weapon (an XRGrabInteractable using Velocity Tracking). Turns physical
    /// collisions into damage using the GDD formula and the anti-cheese rules.
    /// Haptics, VFX and audio subscribe to <see cref="Hit"/>.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class WeaponHitDetector : MonoBehaviour
    {
        [SerializeField] private WeaponData _weapon;
        [SerializeField] private CombatConfig _config;
        [Tooltip("A child transform at the blade tip.")]
        [SerializeField] private Transform _tip;
        [Tooltip("Until the stats service exists, the wielder's STR comes from here.")]
        [SerializeField] private int _strength = 10;

        private readonly SwingTracker _swing = new SwingTracker();
        private readonly Dictionary<IDamageable, float> _lastHitTime = new Dictionary<IDamageable, float>();

        public event Action<HitInfo> Hit;

        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        private void FixedUpdate()
        {
            if (_tip == null || _config == null)
            {
                return;
            }

            _swing.WindowSeconds = _config.swingArcWindow;
            _swing.AddSample(Time.time, _tip.position);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_weapon == null || _config == null)
            {
                return;
            }

            var target = collision.collider.GetComponentInParent<IDamageable>();
            if (target == null || target.IsDead)
            {
                return;
            }

            // Rule 2: per-target cooldown.
            if (_lastHitTime.TryGetValue(target, out float last) && Time.time - last < _config.perTargetCooldown)
            {
                return;
            }

            // Rule 1: minimum swing arc.
            if (_swing.ArcLength() < _config.minSwingArc)
            {
                return;
            }

            // Rule 3 (edge/tip only, flat = 30% blunt) and rule 4 (contact drag) are next on the
            // combat engineer's list; see the GDD's anti-cheese rules.

            var marker = collision.collider.GetComponent<HitZoneMarker>();
            var zone = marker != null ? marker.Zone : HitZone.Torso;
            float speed = collision.relativeVelocity.magnitude;

            var input = new DamageInput
            {
                BaseDamage = _weapon.baseDamage,
                HitSpeed = speed,
                Strength = _strength,
                Zone = zone,
                IsCritical = false, // TODO: crit chance from Perception
                Armor = target.Armor,
                IgnoreArmor = target.IsStaggered,
            };

            float damage = DamageCalculator.Compute(input, _config);
            if (damage <= 0f)
            {
                return;
            }

            _lastHitTime[target] = Time.time;
            var contact = collision.GetContact(0);
            var info = new HitInfo(damage, contact.point, contact.normal, zone, speed);
            target.ApplyDamage(info);
            Hit?.Invoke(info);
        }
    }
}
