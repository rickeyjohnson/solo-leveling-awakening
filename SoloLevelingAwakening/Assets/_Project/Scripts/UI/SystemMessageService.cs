using System.Collections.Generic;
using SLA.Core.Events;
using UnityEngine;

namespace SLA.UI
{
    /// <summary>
    /// Queues System messages and shows them one at a time in a <see cref="SystemWindow"/>.
    /// Listens to game events (level-up for now) so gameplay code never touches UI directly.
    /// </summary>
    public class SystemMessageService : MonoBehaviour
    {
        [SerializeField] private SystemWindow _window;
        [SerializeField] private IntEventChannel _onLevelUp;

        private readonly Queue<SystemMessage> _queue = new Queue<SystemMessage>();

        public static SystemMessageService Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            if (_onLevelUp != null) _onLevelUp.Raised += OnLevelUp;
        }

        private void OnDisable()
        {
            if (_onLevelUp != null) _onLevelUp.Raised -= OnLevelUp;
        }

        public void Post(in SystemMessage message) => _queue.Enqueue(message);

        private void OnLevelUp(int newLevel) => Post(SystemMessage.LevelUp(newLevel));

        private void Update()
        {
            if (_window == null || _queue.Count == 0 || _window.gameObject.activeSelf)
            {
                return;
            }
            _window.Show(_queue.Dequeue());
        }
    }
}
