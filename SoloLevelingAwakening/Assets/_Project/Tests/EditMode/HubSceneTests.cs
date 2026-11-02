using NUnit.Framework;
using SLA.Core;
using SLA.Gates;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace SLA.Tests
{
    /// <summary>Checks on the saved hub scenes (docs/design/mr-hub.md).</summary>
    public class HubSceneTests
    {
        [TestCase(SceneNames.HubMR)]
        [TestCase(SceneNames.HubVirtual)]
        public void MRH_8_EveryHubPortalOpensAGate(string sceneName)
        {
            string path = $"Assets/_Project/Scenes/{sceneName}.unity";

            // Additive, so a scene open in the Editor (and its unsaved changes) is never replaced.
            var scene = SceneManager.GetSceneByPath(path);
            bool wasOpen = scene.isLoaded;
            if (!wasOpen)
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }

            try
            {
                int portals = 0;
                foreach (var root in scene.GetRootGameObjects())
                {
                    foreach (var portal in root.GetComponentsInChildren<GatePortal>(true))
                    {
                        portals++;
                        Assert.IsNotNull(portal.Gate, $"{portal.name} in {sceneName} has no gate, so it would lead back to the hub.");
                    }
                }

                Assert.Greater(portals, 0, $"{sceneName} has no gate portal.");
            }
            finally
            {
                if (!wasOpen)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
    }
}
