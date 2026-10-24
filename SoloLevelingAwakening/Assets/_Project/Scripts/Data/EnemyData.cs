using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Enemy")]
    public class EnemyData : ScriptableObject
    {
        public string id = "goblin";
        public string displayName = "Goblin";
        public Rank rank = Rank.E;
        public float maxHp = 40f;
        public float armor;
        [Tooltip("Damage per hit against the player.")]
        public float damage = 8f;
        public float moveSpeed = 2.5f;
        public float attackRange = 1.2f;
        [Tooltip("Windup before the hit lands: the tell. 0.6-1.0 s.")]
        public float windupSeconds = 0.8f;
        public float recoverSeconds = 1f;
        [Tooltip("Base XP before the rank multiplier.")]
        public int baseXp = 20;
        public Vector2Int goldRange = new Vector2Int(5, 10);
        [TextArea] public string behaviorNotes = "Fast jabs in packs of 2-3.";
        public GameObject prefab;
    }
}
