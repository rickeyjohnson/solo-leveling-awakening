using System;
using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>An event with no payload, e.g. OnGateCleared.</summary>
    [CreateAssetMenu(menuName = "SLA/Events/Void Event Channel")]
    public class VoidEventChannel : ScriptableObject
    {
        public event Action Raised;

        public void Raise() => Raised?.Invoke();
    }
}
