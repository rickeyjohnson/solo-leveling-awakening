using UnityEngine;

namespace SLA.Core.Events
{
    /// <summary>An event carrying a string id, e.g. OnEnemyKilled (enemy id) or OnQuestCompleted (quest id).</summary>
    [CreateAssetMenu(menuName = "SLA/Events/String Event Channel")]
    public class StringEventChannel : EventChannel<string>
    {
    }
}
