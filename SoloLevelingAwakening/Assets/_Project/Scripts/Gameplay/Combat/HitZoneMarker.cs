using SLA.Data;
using UnityEngine;

namespace SLA.Combat
{
    /// <summary>Put on an enemy's collider (head, limb, weak point) to set its damage multiplier. No marker = torso.</summary>
    public class HitZoneMarker : MonoBehaviour
    {
        [SerializeField] private HitZone _zone = HitZone.Torso;

        public HitZone Zone => _zone;
    }
}
