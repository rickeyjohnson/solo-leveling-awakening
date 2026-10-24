using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Gate")]
    public class GateData : ScriptableObject
    {
        public string id = "goblin_warren";
        public string displayName = "Goblin Warren";
        public Rank rank = Rank.E;
        [Tooltip("Shown on the gate panel above the portal.")]
        public Vector2Int recommendedLevels = new Vector2Int(1, 9);
        public bool isRedGate;
        [Tooltip("Scene loaded when the player walks through the portal.")]
        public string sceneName = "Gate_GoblinWarren";
        public EnemyData[] enemies;
        public EnemyData boss;
        public int combatRooms = 2;
        [TextArea] public string objective = "Defeat the Goblin Chieftain.";
    }
}
