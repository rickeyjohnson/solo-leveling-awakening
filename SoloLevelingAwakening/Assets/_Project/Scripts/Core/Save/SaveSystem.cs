using System;
using System.IO;
using UnityEngine;

namespace SLA.Core.Save
{
    /// <summary>
    /// Reads and writes the single JSON save file. Writes go to a temp file first and then
    /// replace the real one, so a crash mid-write can't corrupt the save.
    /// </summary>
    public static class SaveSystem
    {
        public const int CurrentVersion = 1;
        const string FileName = "save.json";

        public static string DefaultPath => Path.Combine(Application.persistentDataPath, FileName);

        public static bool Exists(string path = null) => File.Exists(path ?? DefaultPath);

        public static void Save(SaveData data, string path = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            path ??= DefaultPath;
            data.version = CurrentVersion;
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            string temp = path + ".tmp";
            File.WriteAllText(temp, json);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(temp, path);
        }

        /// <summary>Returns the saved data, or a fresh save if there is none or it can't be read.</summary>
        public static SaveData Load(string path = null)
        {
            path ??= DefaultPath;
            if (!File.Exists(path))
            {
                return SaveData.CreateNew();
            }

            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
                return Migrate(data) ?? SaveData.CreateNew();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveSystem] Could not read {path}: {e.Message}. Starting a new save.");
                return SaveData.CreateNew();
            }
        }

        static SaveData Migrate(SaveData data)
        {
            if (data == null)
            {
                return null;
            }

            // Add one block per version bump, e.g.:
            // if (data.version == 1) { /* convert to v2 */ data.version = 2; }
            data.version = CurrentVersion;
            return data;
        }
    }
}
