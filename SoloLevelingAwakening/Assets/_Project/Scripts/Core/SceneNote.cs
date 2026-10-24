using UnityEngine;

namespace SLA.Core
{
    /// <summary>A sticky note for a scene: who owns it and what's next. Put it on an EditorOnly object.</summary>
    public class SceneNote : MonoBehaviour
    {
        [TextArea(3, 10)] public string text;
    }
}
