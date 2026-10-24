using SLA.Core.Events;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// Owns the current GameState. Lives in the Bootstrap scene, which stays loaded all session.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        [SerializeField] private GameStateEventChannel _onGameStateChanged;

        private GameState _stateBeforePause;

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
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void ChangeState(GameState next)
        {
            if (next == Current)
            {
                return;
            }

            Debug.Log($"[GameStateManager] {Current} -> {next}");
            Current = next;
            _onGameStateChanged?.Raise(next);
        }

        /// <summary>Pauses AI and timers. Head tracking is unaffected: the camera always belongs to the player.</summary>
        public void Pause()
        {
            if (Current == GameState.Paused)
            {
                return;
            }

            _stateBeforePause = Current;
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        public void Resume()
        {
            if (Current != GameState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            ChangeState(_stateBeforePause);
        }
    }
}
