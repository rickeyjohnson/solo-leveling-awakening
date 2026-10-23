using UnityEditor;
using UnityEngine;

namespace SLA.EditorTools.Bootstrap
{
    /// <summary>
    /// Imports everything (so every asset gets its .meta file) and saves.
    /// Headless: -executeMethod SLA.EditorTools.Bootstrap.ProjectSaver.SaveAll
    /// </summary>
    public static class ProjectSaver
    {
        public static void SaveAll()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();
            Debug.Log("[ProjectSaver] Assets imported and saved.");
            EditorApplication.Exit(0);
        }
    }
}
