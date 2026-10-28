namespace SLA.Core
{
    /// <summary>
    /// Moves the game to a state and loads that state's scene as one step, so the two can't get out
    /// of step: if either isn't possible, nothing changes.
    /// </summary>
    public static class GameFlow
    {
        /// <summary>
        /// Changes to <paramref name="state"/> and loads <paramref name="sceneName"/> behind a fade.
        /// Returns false, changing nothing, if Bootstrap isn't loaded, a load is running, the scene isn't
        /// in the build, or the screen flow doesn't allow the state change.
        /// </summary>
        public static bool GoTo(GameState state, string sceneName)
        {
            var states = GameStateManager.Instance;
            var loader = SceneLoader.Instance;
            if (states == null || loader == null || !loader.CanLoad(sceneName))
            {
                return false;
            }

            return states.ChangeState(state) && loader.LoadContentScene(sceneName);
        }
    }
}
