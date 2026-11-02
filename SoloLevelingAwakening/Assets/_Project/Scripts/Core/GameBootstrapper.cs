using SLA.Core.Events;
using SLA.Core.Save;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// First thing that runs: loads the save into GameSession, tells listeners the settings changed (the
    /// rig may have applied the defaults before the save loaded), then sends the player to first-run
    /// setup (no save yet) or the title screen.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        [Tooltip("Raised once the save is loaded, so the rig applies the saved comfort settings.")]
        [SerializeField] private VoidEventChannel _onSettingsChanged;

        private void Start()
        {
            bool hasSave = SaveSystem.Exists();
            GameSession.Data = SaveSystem.Load();
            GameSession.HubScene = SceneNames.HubMR;

            if (_onSettingsChanged != null)
            {
                _onSettingsChanged.Raise();
            }
            else
            {
                Debug.LogError("[GameBootstrapper] No OnSettingsChanged channel assigned; saved comfort settings won't reach the rig.");
            }

            if (GameStateManager.Instance == null || SceneLoader.Instance == null)
            {
                Debug.LogError("[GameBootstrapper] GameStateManager and SceneLoader must be in the Bootstrap scene.");
                return;
            }

            if (hasSave)
            {
                GameFlow.GoTo(GameState.Title, SceneNames.Title);
            }
            else
            {
                GameFlow.GoTo(GameState.FirstRunSetup, SceneNames.Setup);
            }
        }
    }
}
