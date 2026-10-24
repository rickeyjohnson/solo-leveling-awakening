using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>An event carrying an int, e.g. OnLevelUp (new level) or OnRepCounted (rep total).</summary>
    [CreateAssetMenu(menuName = "SLA/Events/Int Event Channel")]
    public class IntEventChannel : EventChannel<int>
    {
    }
}
