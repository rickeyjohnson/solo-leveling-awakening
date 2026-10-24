using UnityEngine;

namespace SLA.Data
{
    [CreateAssetMenu(menuName = "SLA/Data/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public string id = "rusty_dagger";
        public string displayName = "Rusty Dagger";
        public WeaponType type = WeaponType.Dagger;
        public Rank rank = Rank.E;
        public float baseDamage = 10f;
        [Tooltip("Below this STR the weapon follows the hand sluggishly.")]
        public int strengthRequirement;
        public float massKg = 0.5f;
        public bool twoHanded;
        [TextArea] public string special = "";
        public int shopPrice;
    }
}
