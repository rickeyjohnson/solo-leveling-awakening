using System.Collections.Generic;

namespace SLA.AI
{
    /// <summary>
    /// Only N enemies may attack at once (2 by default); the rest circle and wait. This keeps a
    /// 360-degree VR fight fair. An enemy must hold a token before it starts its windup.
    /// </summary>
    public sealed class AttackTokenPool
    {
        private readonly HashSet<int> _holders = new HashSet<int>();

        public AttackTokenPool(int capacity)
        {
            Capacity = capacity < 1 ? 1 : capacity;
        }

        public int Capacity { get; }
        public int InUse => _holders.Count;

        /// <summary>True if the enemy now holds (or already held) a token.</summary>
        public bool TryAcquire(int enemyId)
        {
            if (_holders.Contains(enemyId))
            {
                return true;
            }
            if (_holders.Count >= Capacity)
            {
                return false;
            }
            _holders.Add(enemyId);
            return true;
        }

        public void Release(int enemyId) => _holders.Remove(enemyId);

        public void Clear() => _holders.Clear();
    }
}
