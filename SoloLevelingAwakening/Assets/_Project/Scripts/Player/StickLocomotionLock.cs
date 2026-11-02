using System;
using UnityEngine;

namespace SLA.Player
{
    /// <summary>
    /// While any of these is enabled, the rig's stick locomotion (move, turn, teleport) and the vignette are
    /// off and the player moves only by walking. Hub_MR has one so the passthrough room and the portal
    /// on the real wall stay where the real room is (MRH-13); Title and Setup have one so nobody wanders
    /// off the menus, which have no floor. It goes away with its scene, which turns locomotion back on, whatever order
    /// scenes load in: ComfortSettings re-checks <see cref="IsLocked"/> on every change.
    /// </summary>
    [AddComponentMenu("SLA/Stick Locomotion Lock")]
    public class StickLocomotionLock : MonoBehaviour
    {
        private static int _activeCount;
        private static bool _quitting;

        /// <summary>True while at least one lock is enabled.</summary>
        public static bool IsLocked => _activeCount > 0;

        /// <summary>Raised when a lock is enabled or disabled (not while the app shuts down).</summary>
        public static event Action Changed;

        private void OnEnable()
        {
            _activeCount++;
            Notify();
        }

        private void OnDisable()
        {
            _activeCount = Mathf.Max(0, _activeCount - 1);
            Notify();
        }

        // On quit (or leaving Play mode) every scene tears down: re-enabling the rig's input then would only
        // touch objects that are being destroyed.
        private static void Notify()
        {
            if (!_quitting)
            {
                Changed?.Invoke();
            }
        }

        // Statics survive Enter Play Mode when domain reload is off; start every session unlocked.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _activeCount = 0;
            _quitting = false;
            Changed = null;
            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
        }

        private static void OnQuitting() => _quitting = true;
    }
}
