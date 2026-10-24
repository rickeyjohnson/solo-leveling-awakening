using System;
using System.Collections.Generic;

namespace SLA.Core.Save
{
    /// <summary>
    /// Everything persisted between sessions, written as JSON (docs/ARCHITECTURE.md#save-data).
    /// JsonUtility can't serialize dictionaries, so everything is plain classes and lists.
    /// Bump SaveSystem.CurrentVersion whenever this shape changes, and add a migration.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int version = SaveSystem.CurrentVersion;
        public PlayerSave player = new PlayerSave();
        public List<ItemStack> inventory = new List<ItemStack>();
        public EquipmentSave equipped = new EquipmentSave();
        public int gold;
        public QuestSave quests = new QuestSave();
        public SettingsSave settings = new SettingsSave();

        public static SaveData CreateNew() => new SaveData();
    }

    [Serializable]
    public class PlayerSave
    {
        public int level = 1;
        public int xp;
        public string rank = "E";
        public string job = "None";
        public string title = "";
        public StatsSave stats = new StatsSave();
        public int unspent;
        public int skillPoints;
        public float standingHeight;
    }

    [Serializable]
    public class StatsSave
    {
        public int str = 10;
        public int vit = 10;
        public int agi = 10;
        public int per = 10;
        public int @int = 10; // serialized as "int"
    }

    [Serializable]
    public class ItemStack
    {
        public string id;
        public int count;
    }

    [Serializable]
    public class EquipmentSave
    {
        public string hipL = "";
        public string hipR = "";
        public string back = "";
        public string gloves = "";
    }

    [Serializable]
    public class QuestSave
    {
        public List<string> completed = new List<string>();
        public string active = "";
        public int dailyStreak;
    }

    [Serializable]
    public class SettingsSave
    {
        public string comfortPreset = "Recommended";
        public string uiMode = "Diegetic";
        public bool leftHanded;
    }
}
