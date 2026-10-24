using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>An event carrying a float, e.g. OnDamageTaken (amount).</summary>
    [CreateAssetMenu(menuName = "SLA/Events/Float Event Channel")]
    public class FloatEventChannel : EventChannel<float>
    {
    }
}
