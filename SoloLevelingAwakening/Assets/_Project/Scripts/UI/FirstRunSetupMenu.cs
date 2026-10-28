using SLA.Core;
using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// First-run setup: pick a comfort preset, then go to the title screen. Leaving setup saves
    /// (GameStateRules). Placeholder until height calibration and the room check join it (#11, CMF-13, CMF-14).
    /// </summary>
    public class FirstRunSetupMenu : MonoBehaviour
    {
        // Names match the comfort spec's presets and SettingsSave.comfortPreset.
        private const string Recommended = "Recommended";
        private const string Comfortable = "Comfortable";
        private const string Advanced = "Advanced";

        public void ChooseRecommended() => ChooseAndFinish(Recommended);

        public void ChooseComfortable() => ChooseAndFinish(Comfortable);

        public void ChooseAdvanced() => ChooseAndFinish(Advanced);

        private static void ChooseAndFinish(string preset)
        {
            // Set before the state change: leaving setup saves, and the save must include the choice.
            GameSession.Data.settings.comfortPreset = preset;
            GameFlow.GoTo(GameState.Title, SceneNames.Title);
        }
    }
}
