namespace SLA.Core
{
    /// <summary>
    /// Which state changes the screen flow allows, and which state exits save the game
    /// (docs/ARCHITECTURE.md#game-states). Pure rules, so they're unit-tested.
    /// </summary>
    public static class GameStateRules
    {
        /// <summary>
        /// True if the game may go from <paramref name="from"/> to <paramref name="to"/>.
        /// Pausing and resuming are not transitions: use <see cref="CanPause"/> and GameStateManager.Resume.
        /// </summary>
        public static bool CanTransition(GameState from, GameState to)
        {
            switch (from)
            {
                case GameState.Boot:
                    return to == GameState.FirstRunSetup || to == GameState.Title;
                case GameState.FirstRunSetup:
                    return to == GameState.Title;
                case GameState.Title:
                    // Continue goes home; New game starts in the Prologue gate.
                    return to == GameState.Hub || to == GameState.Gate;
                case GameState.Hub:
                    // Title = "End the day" (MRH-10).
                    return to == GameState.DailyQuest || to == GameState.Gate || to == GameState.Title;
                case GameState.DailyQuest:
                    // PenaltyZone = the quest was abandoned (QST-14).
                    return to == GameState.Hub || to == GameState.PenaltyZone;
                case GameState.PenaltyZone:
                    return to == GameState.Hub;
                case GameState.Gate:
                    // Results = boss defeated (GTE-5); Hub = Return stone, or the Prologue waking up at home (GTE-6).
                    return to == GameState.Results || to == GameState.Hub;
                case GameState.Results:
                    return to == GameState.Hub;
                case GameState.Paused:
                    // Quit from the pause menu. Going back to the paused state is Resume, not a transition.
                    return to == GameState.Title;
                default:
                    return false;
            }
        }

        /// <summary>True if the pause menu may open in this state.</summary>
        public static bool CanPause(GameState state) => state != GameState.Boot && state != GameState.Paused;

        /// <summary>
        /// True if leaving this state writes the save file. Saves happen only on these exits, never mid-gate;
        /// pausing is an overlay, not an exit.
        /// </summary>
        public static bool ShouldSaveOnExit(GameState exited)
        {
            switch (exited)
            {
                case GameState.FirstRunSetup: // settings, standing height
                case GameState.Hub:           // "End the day", or leaving for a gate
                case GameState.DailyQuest:    // rewards and streak
                case GameState.PenaltyZone:
                case GameState.Gate:          // only the boss-shortcut flag changes inside a gate
                case GameState.Results:       // XP, loot, rank-up
                    return true;
                default:
                    return false;
            }
        }
    }
}
