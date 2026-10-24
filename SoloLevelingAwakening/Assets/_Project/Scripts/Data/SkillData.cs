using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Skill")]
    public class SkillData : ScriptableObject
    {
        public string id = "dash";
        public string displayName = "Dash";
        public SkillActivation activation = SkillActivation.GestureAndButton;
        [Tooltip("Player level that unlocks it (0 = story unlock).")]
        public int unlockLevel;
        public float mpCost;
        public float fatigueCost;
        public float cooldownSeconds;
        [TextArea] public string gesture = "";
        [TextArea] public string effect = "";
    }
}
