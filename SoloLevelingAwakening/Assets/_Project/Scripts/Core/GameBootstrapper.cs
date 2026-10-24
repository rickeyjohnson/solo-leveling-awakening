using SLA.Core.Save;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// First thing that runs: loads the save, then sends the player to first-run setup or the title screen.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        public static SaveData Save { get; private set; }

        private void Start()
        {
            bool hasSave = SaveSystem.Exists();
            Save = SaveSystem.Load();

            var states = GameStateManager.Instance;
            var loader = SceneLoader.Instance;
            if (states == null || loader == null)
            {
                Debug.LogError("[GameBootstrapper] GameStateManager and SceneLoader must be in the Bootstrap scene.");
                return;
            }

            if (hasSave)
            {
                states.ChangeState(GameState.Title);
                loader.LoadContentScene(SceneNames.Title);
            }
            else
            {
                states.ChangeState(GameState.FirstRunSetup);
                loader.LoadContentScene(SceneNames.Setup);
            }
        }
    }
}
