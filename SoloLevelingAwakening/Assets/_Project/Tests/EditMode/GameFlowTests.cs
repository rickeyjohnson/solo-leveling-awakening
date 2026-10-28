using NUnit.Framework;
using SLA.Core;
using SLA.Core.Save;

namespace SLA.Tests
{
    /// <summary>Game flow from docs/ARCHITECTURE.md#game-states and the screen-flow diagram.</summary>
    public class GameFlowTests
    {
        // ------------------------------------------------------------------ transitions

        [TestCase(GameState.Boot, GameState.FirstRunSetup)]
        [TestCase(GameState.Boot, GameState.Title)]
        [TestCase(GameState.FirstRunSetup, GameState.Title)]
        [TestCase(GameState.Title, GameState.Hub)]
        [TestCase(GameState.Title, GameState.Gate)] // new game starts in the Prologue gate
        [TestCase(GameState.Hub, GameState.DailyQuest)]
        [TestCase(GameState.Hub, GameState.Gate)]
        [TestCase(GameState.Hub, GameState.Title)] // end the day
        [TestCase(GameState.DailyQuest, GameState.Hub)]
        [TestCase(GameState.DailyQuest, GameState.PenaltyZone)]
        [TestCase(GameState.PenaltyZone, GameState.Hub)]
        [TestCase(GameState.Gate, GameState.Results)]
        [TestCase(GameState.Gate, GameState.Hub)]
        [TestCase(GameState.Results, GameState.Hub)]
        [TestCase(GameState.Paused, GameState.Title)] // quit from the pause menu
        public void ScreenFlowTransitionsAreAllowed(GameState from, GameState to)
        {
            Assert.IsTrue(GameStateRules.CanTransition(from, to));
        }

        [TestCase(GameState.Boot, GameState.Gate)]
        [TestCase(GameState.Title, GameState.Results)]
        [TestCase(GameState.Hub, GameState.Results)]
        [TestCase(GameState.Hub, GameState.PenaltyZone)]
        [TestCase(GameState.Results, GameState.Gate)]
        [TestCase(GameState.PenaltyZone, GameState.Gate)]
        [TestCase(GameState.Gate, GameState.Title)]
        [TestCase(GameState.Hub, GameState.Boot)]
        [TestCase(GameState.Paused, GameState.Gate)] // leaving pause any other way is Resume()
        public void TransitionsOutsideTheScreenFlowAreRejected(GameState from, GameState to)
        {
            Assert.IsFalse(GameStateRules.CanTransition(from, to));
        }

        [Test]
        public void ChangingToTheSameStateIsNotATransition()
        {
            Assert.IsFalse(GameStateRules.CanTransition(GameState.Hub, GameState.Hub));
        }

        [Test]
        public void GTE_5_BossDefeatGoesToResultsThenHome()
        {
            Assert.IsTrue(GameStateRules.CanTransition(GameState.Gate, GameState.Results));
            Assert.IsTrue(GameStateRules.CanTransition(GameState.Results, GameState.Hub));
        }

        [Test]
        public void GTE_6_ReturnStoneLeavesTheGateForTheHub()
        {
            Assert.IsTrue(GameStateRules.CanTransition(GameState.Gate, GameState.Hub));
        }

        [Test]
        public void EveryStateExceptBootCanBePaused()
        {
            Assert.IsFalse(GameStateRules.CanPause(GameState.Boot));
            Assert.IsFalse(GameStateRules.CanPause(GameState.Paused));
            Assert.IsTrue(GameStateRules.CanPause(GameState.Hub));
            Assert.IsTrue(GameStateRules.CanPause(GameState.Gate));
        }

        // ------------------------------------------------------------------ saving

        [TestCase(GameState.FirstRunSetup, true)]
        [TestCase(GameState.Hub, true)]
        [TestCase(GameState.DailyQuest, true)]
        [TestCase(GameState.PenaltyZone, true)]
        [TestCase(GameState.Gate, true)]
        [TestCase(GameState.Results, true)]
        [TestCase(GameState.Boot, false)]
        [TestCase(GameState.Title, false)]
        [TestCase(GameState.Paused, false)]
        public void SavesOnlyWhenTheseStatesExit(GameState exited, bool saves)
        {
            Assert.AreEqual(saves, GameStateRules.ShouldSaveOnExit(exited));
        }

        [Test]
        public void PausingAndResumingIsNotAnExit()
        {
            var tracker = new StateExitTracker(GameState.Gate);

            Assert.IsFalse(tracker.TryExit(GameState.Paused, out _));
            Assert.IsFalse(tracker.TryExit(GameState.Gate, out _));
        }

        [Test]
        public void PausingMidGateNeverSaves()
        {
            var tracker = new StateExitTracker(GameState.Gate);

            bool exited = tracker.TryExit(GameState.Paused, out GameState state);

            Assert.IsFalse(exited && GameStateRules.ShouldSaveOnExit(state));
        }

        [Test]
        public void QuittingToTitleFromPauseExitsTheStateUnderThePause()
        {
            var tracker = new StateExitTracker(GameState.Hub);
            tracker.TryExit(GameState.Paused, out _);

            Assert.IsTrue(tracker.TryExit(GameState.Title, out GameState exited));
            Assert.AreEqual(GameState.Hub, exited);
        }

        [Test]
        public void LeavingAGateReportsTheGateAsExited()
        {
            var tracker = new StateExitTracker(GameState.Hub);
            tracker.TryExit(GameState.Gate, out _);

            Assert.IsTrue(tracker.TryExit(GameState.Results, out GameState exited));
            Assert.AreEqual(GameState.Gate, exited);
        }

        // ------------------------------------------------------------------ new game

        [Test]
        public void NewGameResetsProgressButKeepsSettingsAndHeight()
        {
            var old = SaveData.CreateNew();
            old.player.level = 14;
            old.gold = 640;
            old.player.standingHeight = 1.82f;
            old.settings.comfortPreset = "Comfortable";
            old.settings.leftHanded = true;

            var fresh = SaveData.NewGameFrom(old);

            Assert.AreEqual(1, fresh.player.level);
            Assert.AreEqual(0, fresh.gold);
            Assert.AreEqual(1.82f, fresh.player.standingHeight);
            Assert.AreEqual("Comfortable", fresh.settings.comfortPreset);
            Assert.IsTrue(fresh.settings.leftHanded);
            Assert.AreNotSame(old.settings, fresh.settings);
        }

        // ------------------------------------------------------------------ fades

        [Test]
        public void FadeMovesTowardTheTargetAtAFixedRate()
        {
            Assert.AreEqual(0.5f, FadeMath.Step(0f, 1f, 0.25f, 0.125f), 1e-5f);
            Assert.AreEqual(1f, FadeMath.Step(0.9f, 1f, 0.25f, 0.125f), 1e-5f);
            Assert.AreEqual(0.5f, FadeMath.Step(1f, 0f, 0.25f, 0.125f), 1e-5f);
        }

        [Test]
        public void ZeroLengthFadeJumpsStraightToTheTarget()
        {
            Assert.AreEqual(1f, FadeMath.Step(0f, 1f, 0f, 0.016f));
        }
    }
}
