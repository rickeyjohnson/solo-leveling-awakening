using SLA.Data;
using UnityEngine;

namespace SLA.AI
{
    /// <summary>One per gate scene: owns the attack tokens every enemy in the gate shares.</summary>
    public class EnemyDirector : MonoBehaviour
    {
        [SerializeField] private CombatConfig _config;

        public static EnemyDirector Current { get; private set; }

        public AttackTokenPool Tokens { get; private set; }

        private void Awake()
        {
            Current = this;
            Tokens = new AttackTokenPool(_config != null ? _config.maxAttackTokens : 2);
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                Current = null;
            }
        }
    }
}
