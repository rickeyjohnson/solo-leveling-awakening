using System.IO;
using NUnit.Framework;
using SLA.Core.Save;

namespace SLA.Tests
{
    public class SaveSystemTests
    {
        private string _path;

        [SetUp]
        public void SetUp() => _path = Path.Combine(Path.GetTempPath(), $"sla_save_test_{System.Guid.NewGuid():N}.json");

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_path)) File.Delete(_path);
        }

        [Test]
        public void MissingFile_GivesAFreshSave()
        {
            var data = SaveSystem.Load(_path);
            Assert.AreEqual(1, data.player.level);
            Assert.AreEqual(10, data.player.stats.str);
            Assert.AreEqual(SaveSystem.CurrentVersion, data.version);
        }

        [Test]
        public void SaveThenLoad_RoundTrips()
        {
            var data = SaveData.CreateNew();
            data.player.level = 14;
            data.player.rank = "D";
            data.player.stats.@int = 13;
            data.gold = 640;
            data.inventory.Add(new ItemStack { id = "health_potion", count = 3 });
            data.quests.completed.Add("goblin_warren");

            SaveSystem.Save(data, _path);
            var loaded = SaveSystem.Load(_path);

            Assert.AreEqual(14, loaded.player.level);
            Assert.AreEqual("D", loaded.player.rank);
            Assert.AreEqual(13, loaded.player.stats.@int);
            Assert.AreEqual(640, loaded.gold);
            Assert.AreEqual("health_potion", loaded.inventory[0].id);
            Assert.AreEqual(3, loaded.inventory[0].count);
            Assert.Contains("goblin_warren", loaded.quests.completed);
        }

        [Test]
        public void CorruptFile_GivesAFreshSaveInsteadOfThrowing()
        {
            File.WriteAllText(_path, "{ this is not json");
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Error, new System.Text.RegularExpressions.Regex("Could not read"));
            var data = SaveSystem.Load(_path);
            Assert.AreEqual(1, data.player.level);
        }
    }
}
