using SLA.Core;
using SLA.Core.Save;
using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// Title screen buttons: Continue, Continue in the virtual room (always offered, MRH-12), New game.
    /// Placeholder look until the System-window title screen; the flow is final.
    /// </summary>
    public class TitleMenu : MonoBehaviour
    {
        public void Continue() => GoHome(SceneNames.HubMR);

        public void ContinueInVirtualRoom() => GoHome(SceneNames.HubVirtual);

        /// <summary>Starts over but keeps comfort settings and calibrated height.</summary>
        public void NewGame()
        {
            var fresh = SaveData.NewGameFrom(GameSession.Data);
            // New game should start in the Prologue gate; until that scene exists it starts at home.
            if (GoHome(SceneNames.HubMR))
            {
                GameSession.Data = fresh;
            }
        }

        // GameFlow ignores the press while a load is fading, so one click can't start two loads.
        private static bool GoHome(string hubScene)
        {
            if (!GameFlow.GoTo(GameState.Hub, hubScene))
            {
                return false;
            }

            GameSession.HubScene = hubScene;
            return true;
        }
    }
}
