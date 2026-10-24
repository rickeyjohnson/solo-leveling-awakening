using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>Raised by GameStateManager whenever the game state changes.</summary>
    [CreateAssetMenu(menuName = "SLA/Events/Game State Event Channel")]
    public class GameStateEventChannel : EventChannel<GameState>
    {
    }
}
