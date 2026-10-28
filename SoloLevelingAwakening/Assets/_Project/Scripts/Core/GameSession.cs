using SLA.Core.Save;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// What lasts for one play session: the loaded save and which hub the player calls home.
    /// GameBootstrapper fills it at launch; SaveOnStateExit writes Data back on state exits.
    /// </summary>
    public static class GameSession
    {
        /// <summary>The save loaded at launch. Systems change it; it reaches disk only on state exits.</summary>
        public static SaveData Data { get; set; } = SaveData.CreateNew();

        /// <summary>
        /// Hub_MR or Hub_Virtual. Portals home lead here, so a player who started in the virtual
        /// room (no room scan, or the expo option) comes back to it (MRH-4, MRH-12).
        /// </summary>
        public static string HubScene { get; set; } = SceneNames.HubMR;

        // Statics survive Enter Play Mode when domain reload is off; start every session clean.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            Data = SaveData.CreateNew();
            HubScene = SceneNames.HubMR;
        }
    }
}
