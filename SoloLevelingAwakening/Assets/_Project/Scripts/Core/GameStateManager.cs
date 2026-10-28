using SLA.Core.Events;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// Owns the current GameState and only allows the changes in the screen flow (GameStateRules).
    /// Lives in the Bootstrap scene, which stays loaded all session.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameStateManager : MonoBehaviour
    {
        /// <summary>The manager in Bootstrap; null when Bootstrap isn't loaded (e.g. a scene played on its own).</summary>
        public static GameStateManager Instance { get; private set; }

        [SerializeField] private GameStateEventChannel _onGameStateChanged;

        private GameState _stateBeforePause;

        /// <summary>The state the game is in now.</summary>
        public GameState Current { get; private set; } = GameState.Boot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameStateManager] A second instance was destroyed. Only Bootstrap should have one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            if (Current == GameState.Paused)
            {
                Time.timeScale = 1f;
            }
            Instance = null;
        }

        /// <summary>
        /// Moves to <paramref name="next"/> and raises OnGameStateChanged. Returns false, and logs an error,
        /// if the screen flow doesn't allow it. Asking for the current state does nothing and returns true.
        /// </summary>
        public bool ChangeState(GameState next)
        {
            if (next == Current)
            {
                return true;
            }

            if (!GameStateRules.CanTransition(Current, next))
            {
                Debug.LogError($"[GameStateManager] {Current} -> {next} is not in the screen flow; ignored.");
                return false;
            }

            if (Current == GameState.Paused)
            {
                Time.timeScale = 1f; // quitting from the pause menu
            }
            SetState(next);
            return true;
        }

        /// <summary>
        /// Opens the pause overlay: stops AI and timers. Head tracking is unaffected, because the camera
        /// always belongs to the player (CMF-1). Returns false if this state can't be paused.
        /// </summary>
        public bool Pause()
        {
            if (!GameStateRules.CanPause(Current))
            {
                return false;
            }

            _stateBeforePause = Current;
            Time.timeScale = 0f;
            SetState(GameState.Paused);
            return true;
        }

        /// <summary>Closes the pause overlay and returns to the state underneath.</summary>
        public void Resume()
        {
            if (Current != GameState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            SetState(_stateBeforePause);
        }

        private void SetState(GameState next)
        {
            Debug.Log($"[GameStateManager] {Current} -> {next}");
            Current = next;
            _onGameStateChanged?.Raise(next);
        }
    }
}
