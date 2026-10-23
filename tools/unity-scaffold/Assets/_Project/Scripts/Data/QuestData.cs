using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Quest")]
    public class QuestData : ScriptableObject
    {
        public string id = "daily_preparation";
        [Tooltip("Shown in the System's voice, in brackets.")]
        public string title = "Daily Quest: Preparation to Become Strong";
        public QuestKind kind = QuestKind.Daily;
        [TextArea] public string objective = "";
        public int rewardXp;
        public int rewardStatPoints;
        public int rewardGold;
        public string nextQuestId = "";
    }
}
