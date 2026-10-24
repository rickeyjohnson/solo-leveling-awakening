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
        [Tooltip("Gate to enter. Leave empty for a portal that leads to Target Scene instead.")]
        [SerializeField] private GateData _gate;
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

            var loader = SceneLoader.Instance;
            if (loader == null || loader.IsLoading)
            {
                return;
            }

            _used = true;
            string scene = _gate != null ? _gate.sceneName : _targetScene;
            GameState state = _gate != null ? GameState.Gate : _targetState;

            GameStateManager.Instance?.ChangeState(state);
            loader.LoadContentScene(scene);
        }
    }
}
