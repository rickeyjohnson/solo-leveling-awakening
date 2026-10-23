using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Item")]
    public class ItemData : ScriptableObject
    {
        public string id = "health_potion";
        public string displayName = "Health potion";
        public ItemKind kind = ItemKind.Consumable;
        public Rank rank = Rank.E;
        public int price = 50;
        public int sellValue;
        [TextArea] public string effect = "+50 HP over 2 s";
    }
}
