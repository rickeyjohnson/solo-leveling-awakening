using System;
using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>
    /// Base for ScriptableObject event channels. A channel is an asset any system can raise or
    /// listen to, so systems never hold references to each other. Listeners subscribe in
    /// OnEnable and unsubscribe in OnDisable.
    /// </summary>
    public abstract class EventChannel<T> : ScriptableObject
    {
        public event Action<T> Raised;

        public void Raise(T value) => Raised?.Invoke(value);
    }
}
