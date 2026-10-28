using System;
using SLA.Core.Events;
using SLA.Core.Save;
using UnityEngine;

namespace SLA.Core
{
    /// <summary>
    /// Writes GameSession.Data to disk whenever the game leaves a state that saves on exit
    /// (GameStateRules.ShouldSaveOnExit). Pausing isn't an exit, so nothing is saved mid-gate.
    /// Lives in Bootstrap.
    /// </summary>
    public class SaveOnStateExit : MonoBehaviour
    {
        [SerializeField] private GameStateEventChannel _onGameStateChanged;

        private StateExitTracker _tracker;

        private void OnEnable()
        {
            var states = GameStateManager.Instance;
            _tracker = new StateExitTracker(states != null ? states.Current : GameState.Boot);

            if (_onGameStateChanged != null)
            {
                _onGameStateChanged.Raised += OnStateChanged;
            }
            else
            {
                Debug.LogError("[SaveOnStateExit] No OnGameStateChanged channel assigned; the game will never save.");
            }
        }

        private void OnDisable()
        {
            if (_onGameStateChanged != null)
            {
                _onGameStateChanged.Raised -= OnStateChanged;
            }
        }

        private void OnStateChanged(GameState next)
        {
            if (!_tracker.TryExit(next, out GameState exited) || !GameStateRules.ShouldSaveOnExit(exited))
            {
                return;
            }

            try
            {
                SaveSystem.Save(GameSession.Data);
                Debug.Log($"[SaveOnStateExit] Saved on leaving {exited}.");
            }
            catch (Exception e)
            {
                // A full or read-only disk must not stop the player from moving on.
                Debug.LogError($"[SaveOnStateExit] Save failed on leaving {exited}: {e.Message}");
            }
        }
    }
}
