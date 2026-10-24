using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SLA.EditorTools.Bootstrap
{
    /// <summary>
    /// Installs the project's packages through the Package Manager Client API.
    /// Headless: run the Editor in -batchmode WITHOUT -quit, with
    /// -executeMethod SLA.EditorTools.Bootstrap.PackageInstaller.Install
    /// The method keeps the Editor alive until UPM resolves, then exits with 0 (ok), 1 (error) or 2 (timeout).
    /// Package choices are explained in docs/ARCHITECTURE.md.
    /// </summary>
    public static class PackageInstaller
    {
        static readonly string[] PackagesToAdd =
        {
            "com.unity.render-pipelines.universal",
            "com.unity.inputsystem",
            "com.unity.ugui",
            "com.unity.xr.management",
            "com.unity.xr.openxr",
            "com.unity.xr.meta-openxr",
            "com.unity.xr.arfoundation",
            "com.unity.xr.interaction.toolkit",
            "com.unity.xr.hands",
            "com.unity.ai.navigation",
            "com.unity.behavior",
            "com.unity.probuilder",
            "com.unity.test-framework",
        };

        // Template defaults the project doesn't use. Only the ones present in the manifest are removed.
        static readonly string[] PackagesToRemove =
        {
            "com.unity.visualscripting",
            "com.unity.multiplayer.center",
        };

        const double TimeoutSeconds = 900;

        static AddAndRemoveRequest _request;
        static double _deadline;

        public static void Install()
        {
            string manifest = File.ReadAllText("Packages/manifest.json");
            string[] toRemove = PackagesToRemove.Where(p => manifest.Contains($"\"{p}\"")).ToArray();
            Debug.Log($"[PackageInstaller] Adding: {string.Join(", ", PackagesToAdd)}; removing: {string.Join(", ", toRemove)}");
            _request = Client.AddAndRemove(PackagesToAdd, toRemove);
            _deadline = EditorApplication.timeSinceStartup + TimeoutSeconds;
            EditorApplication.update += Poll;
        }

        static void Poll()
        {
            if (_request == null)
            {
                return;
            }

            if (!_request.IsCompleted)
            {
                if (EditorApplication.timeSinceStartup > _deadline)
                {
                    EditorApplication.update -= Poll;
                    Debug.LogError("[PackageInstaller] Timed out waiting for UPM.");
                    EditorApplication.Exit(2);
                }
                return;
            }

            EditorApplication.update -= Poll;

            if (_request.Status == StatusCode.Success)
            {
                var names = _request.Result.Select(p => $"{p.name}@{p.version}");
                Debug.Log($"[PackageInstaller] Resolved: {string.Join(", ", names)}");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[PackageInstaller] Failed: {_request.Error?.message}");
                EditorApplication.Exit(1);
            }
        }
    }
}
