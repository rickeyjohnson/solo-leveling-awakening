using SLA.Core;
using SLA.Data;
using UnityEngine;

namespace SLA.Gates
{
    /// <summary>
    /// A portal the player walks into. In the hub it opens a gate; inside a gate it leads home.
    /// Needs a trigger collider; the XR Origin root must be tagged "Player".
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GatePortal : MonoBehaviour
    {
        [Tooltip("Gate to enter. Leave empty for a portal that leads to Target State instead.")]
        [SerializeField] private GateData _gate;
        [Tooltip("Not used when Target State is Hub: portals home go to the hub the player came from (MRH-12).")]
        [SerializeField] private string _targetScene = SceneNames.HubMR;
        [SerializeField] private GameState _targetState = GameState.Hub;

        private bool _used;

        public GateData Gate
        {
            get => _gate;
            set => _gate = value;
        }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_used || !other.CompareTag("Player"))
            {
                return;
            }

            GameState state = _gate != null ? GameState.Gate : _targetState;
            string scene = _gate != null ? _gate.sceneName
                : state == GameState.Hub ? GameSession.HubScene
                : _targetScene;

            _used = GameFlow.GoTo(state, scene);
        }
    }
}
