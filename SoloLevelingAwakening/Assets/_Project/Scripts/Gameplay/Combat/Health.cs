using System;
using UnityEngine;

namespace SLA.Combat
{
    /// <summary>HP, armor and stagger for anything damageable.</summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHp = 100f;
        [SerializeField] private float _armor;

        private float _current;
        private float _staggerUntil;

        public event Action<HitInfo> Damaged;
        public event Action Died;

        public float Max => _maxHp;
        public float Current => _current;
        public float Armor => _armor;
        public bool IsStaggered => Time.time < _staggerUntil;
        public bool IsDead => _current <= 0f;

        private void Awake()
        {
            _current = _maxHp;
        }

        /// <summary>Sets values from data (e.g. EnemyData) when spawning.</summary>
        public void Initialize(float maxHp, float armor)
        {
            _maxHp = maxHp;
            _armor = armor;
            _current = maxHp;
        }

        public void ApplyDamage(in HitInfo hit)
        {
            if (IsDead)
            {
                return;
            }

            _current = Mathf.Max(0f, _current - hit.Damage);
            Damaged?.Invoke(hit);
            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (!IsDead)
            {
                _current = Mathf.Min(_maxHp, _current + amount);
            }
        }

        public void Stagger(float seconds)
        {
            _staggerUntil = Mathf.Max(_staggerUntil, Time.time + seconds);
        }
    }
}
