namespace SLA.Core
{
    /// <summary>
    /// Every screen in the screen-flow diagram is one of these states (docs/ARCHITECTURE.md#game-states).
    /// Saves happen only when a state exits, never mid-gate.
    /// </summary>
    public enum GameState
    {
        Boot,
        FirstRunSetup,
        Title,
        Hub,
        DailyQuest,
        PenaltyZone,
        Gate,
        Results,
        Paused,
    }
}
