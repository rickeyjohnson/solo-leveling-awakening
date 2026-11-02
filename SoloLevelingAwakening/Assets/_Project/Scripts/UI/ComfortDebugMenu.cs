using SLA.Core;
using SLA.Core.Events;
using SLA.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SLA.UI
{
    /// <summary>
    /// Sandbox-only test panel: switches the comfort preset and left-handed mode live, the same way a
    /// settings screen will (write GameSession.Data.settings, raise OnSettingsChanged). Built into the
    /// Sandbox_* scenes by PlayerRigSetup; never put it in a shipped scene.
    /// </summary>
    public class ComfortDebugMenu : MonoBehaviour
    {
        [SerializeField] private VoidEventChannel _onSettingsChanged;
        [SerializeField] private Text _status;

        private void OnEnable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raised -= Refresh;
            }
        }

        public void UseRecommended() => UsePreset(ComfortPresetNames.Recommended);

        public void UseComfortable() => UsePreset(ComfortPresetNames.Comfortable);

        public void UseAdvanced() => UsePreset(ComfortPresetNames.Advanced);

        public void ToggleLeftHanded()
        {
            var settings = GameSession.Data.settings;
            settings.leftHanded = !settings.leftHanded;
            Raise();
        }

        private void UsePreset(string id)
        {
            GameSession.Data.settings.comfortPreset = id;
            Raise();
        }

        private void Raise()
        {
            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raise();
            }
            else
            {
                Debug.LogError("[ComfortDebugMenu] No OnSettingsChanged channel assigned.", this);
            }
        }

        // Only on a button press, never per frame, so the string building is fine.
        private void Refresh()
        {
            if (_status == null)
            {
                return;
            }

            var settings = GameSession.Data.settings;
            _status.text = $"{settings.comfortPreset}, {(settings.leftHanded ? "left-handed" : "right-handed")}";
        }
    }
}
