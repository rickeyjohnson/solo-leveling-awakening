using SLA.Core.Save;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// First thing that runs: loads the save into GameSession, then sends the player to first-run
    /// setup (no save yet) or the title screen.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            bool hasSave = SaveSystem.Exists();
            GameSession.Data = SaveSystem.Load();
            GameSession.HubScene = SceneNames.HubMR;

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
