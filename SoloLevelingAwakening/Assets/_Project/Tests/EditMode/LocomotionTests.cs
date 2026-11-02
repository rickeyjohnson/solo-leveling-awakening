using NUnit.Framework;
using SLA.Data;
using SLA.Player;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

namespace SLA.Tests
{
    /// <summary>
    /// Player rig locomotion from docs/design/comfort-and-accessibility.md (CMF-2, CMF-5, CMF-8)
    /// and docs/design/mr-hub.md (MRH-13).
    /// </summary>
    public class LocomotionTests
    {
        private const float Walk = 1.5f;
        private const float Sprint = 3f;

        // ------------------------------------------------------------------ CMF-2: quantized speed

        [Test]
        public void CMF_2_StickInsideTheDeadZoneIsStopped()
        {
            // The Move action's StickDeadzone processor zeroes small deflections before the provider sees them.
            Vector2 small = new StickDeadzoneProcessor().Process(new Vector2(0.1f, 0f));

            Assert.AreEqual(0f, QuantizedMove.SpeedMetersPerSecond(small, false, Walk, Sprint));
            Assert.AreEqual(Vector2.zero, QuantizedMove.Direction(small));
        }

        [Test]
        public void CMF_2_CentredStickIsStopped()
        {
            Assert.IsFalse(QuantizedMove.IsPastDeadZone(Vector2.zero));
            Assert.AreEqual(0f, QuantizedMove.SpeedMetersPerSecond(Vector2.zero, true, Walk, Sprint));
        }

        [Test]
        public void CMF_2_HalfStickStillWalksAtFullWalkSpeed()
        {
            Assert.AreEqual(Walk, QuantizedMove.SpeedMetersPerSecond(new Vector2(0f, 0.5f), false, Walk, Sprint));
        }

        [Test]
        public void CMF_2_SlightDeflectionPastTheDeadZoneWalksAtFullSpeedImmediately()
        {
            // No ramp: the first frame past the dead zone is already walk speed.
            Assert.AreEqual(Walk, QuantizedMove.SpeedMetersPerSecond(new Vector2(0.02f, 0f), false, Walk, Sprint));
        }

        [Test]
        public void CMF_2_SprintingMovesAtExactlySprintSpeed()
        {
            Assert.AreEqual(Sprint, QuantizedMove.SpeedMetersPerSecond(new Vector2(0f, 0.3f), true, Walk, Sprint));
            Assert.AreEqual(Sprint, QuantizedMove.SpeedMetersPerSecond(new Vector2(0f, 1f), true, Walk, Sprint));
        }

        [Test]
        public void CMF_2_DirectionStaysAnalogButIsAlwaysUnitLength()
        {
            Vector2 direction = QuantizedMove.Direction(new Vector2(0.3f, 0.3f));

            Assert.AreEqual(1f, direction.magnitude, 1e-5f);
            Assert.AreEqual(direction.x, direction.y, 1e-5f);
        }

        [Test]
        public void CMF_2_DiagonalIsNotFasterThanStraight()
        {
            float straight = QuantizedMove.SpeedMetersPerSecond(new Vector2(0f, 1f), false, Walk, Sprint);
            float diagonal = QuantizedMove.SpeedMetersPerSecond(new Vector2(1f, 1f), false, Walk, Sprint);

            Assert.AreEqual(straight, diagonal);
            Assert.AreEqual(1f, QuantizedMove.Direction(new Vector2(1f, 1f)).magnitude, 1e-5f);
        }

        // ------------------------------------------------------------------ CMF-2: sprint toggle

        [Test]
        public void CMF_2_StickClickWhileMovingStartsSprinting()
        {
            var sprint = new SprintToggle();

            Assert.IsFalse(sprint.Update(false, true));
            Assert.IsTrue(sprint.Update(true, true));
        }

        [Test]
        public void CMF_2_SprintStaysOnWhileTheStickIsHeld()
        {
            var sprint = new SprintToggle();
            sprint.Update(true, true);

            for (int frame = 0; frame < 300; frame++)
            {
                Assert.IsTrue(sprint.Update(false, true));
            }
        }

        [Test]
        public void CMF_2_SecondStickClickGoesBackToWalking()
        {
            var sprint = new SprintToggle();
            sprint.Update(true, true);

            Assert.IsFalse(sprint.Update(true, true));
        }

        [Test]
        public void CMF_2_SprintEndsWhenTheStickReturnsToCentre()
        {
            var sprint = new SprintToggle();
            sprint.Update(true, true);

            Assert.IsFalse(sprint.Update(false, false));
            Assert.IsFalse(sprint.Update(false, true), "Pushing again walks until the next click.");
        }

        [Test]
        public void CMF_2_StickClickWithTheStickCentredDoesNotSprint()
        {
            var sprint = new SprintToggle();

            Assert.IsFalse(sprint.Update(true, false));
            Assert.IsFalse(sprint.Update(false, true));
        }

        // ------------------------------------------------------------------ CMF-8: left-handed mode

        [Test]
        public void CMF_8_RightHandedPlayersMoveWithTheLeftStick()
        {
            Assert.IsTrue(HandednessMap.MovementHandIsLeft(false));

            var left = HandednessMap.ForHand(true, false, MovementMode.Smooth, TurnMode.Snap);
            var right = HandednessMap.ForHand(false, false, MovementMode.Smooth, TurnMode.Snap);

            Assert.IsTrue(left.SmoothMotion);
            Assert.IsFalse(right.SmoothMotion, "The other stick turns (and teleports when pushed forward).");
        }

        [Test]
        public void CMF_8_LeftHandedModeSwapsTheMoveAndTurnSticks()
        {
            Assert.IsFalse(HandednessMap.MovementHandIsLeft(true));

            var left = HandednessMap.ForHand(true, true, MovementMode.Smooth, TurnMode.Snap);
            var right = HandednessMap.ForHand(false, true, MovementMode.Smooth, TurnMode.Snap);

            Assert.IsFalse(left.SmoothMotion);
            Assert.IsTrue(right.SmoothMotion);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CMF_8_TeleportPresetAimsTheArcFromTheMovementHandInEitherMode(bool leftHanded)
        {
            bool movementHandIsLeft = HandednessMap.MovementHandIsLeft(leftHanded);
            var movementHand = HandednessMap.ForHand(movementHandIsLeft, leftHanded, MovementMode.Teleport, TurnMode.Snap);

            Assert.IsFalse(movementHand.SmoothMotion, "Without smooth motion the stick aims the teleport arc.");
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CMF_8_SprintClickIsOnTheMovementHand(bool leftHanded, bool sprintOnLeft)
        {
            var plan = LocomotionPlan.For(MovementMode.Smooth, TurnMode.Snap, true, leftHanded, false);

            Assert.AreEqual(sprintOnLeft, plan.SprintOnLeftHand);
        }

        [Test]
        public void CMF_8_SmoothTurnSettingAppliesToBothHands()
        {
            Assert.IsTrue(HandednessMap.ForHand(true, true, MovementMode.Smooth, TurnMode.Smooth).SmoothTurn);
            Assert.IsTrue(HandednessMap.ForHand(false, true, MovementMode.Smooth, TurnMode.Smooth).SmoothTurn);
            Assert.IsFalse(HandednessMap.ForHand(true, false, MovementMode.Smooth, TurnMode.Snap).SmoothTurn);
        }

        // ------------------------------------------------------------------ presets on the rig

        [TestCase(TurnMode.Snap, true, false)]
        [TestCase(TurnMode.Smooth, false, true)]
        public void CMF_5_OnlyOneTurnProviderIsOnAtATime(TurnMode turning, bool snap, bool smooth)
        {
            var plan = LocomotionPlan.For(MovementMode.Smooth, turning, true, false, false);

            Assert.AreEqual(snap, plan.SnapTurnEnabled);
            Assert.AreEqual(smooth, plan.SmoothTurnEnabled);
        }

        [Test]
        public void CMF_7_TeleportPresetTurnsSmoothMovementOffAndKeepsTeleport()
        {
            var plan = LocomotionPlan.For(MovementMode.Teleport, TurnMode.Snap, true, false, false);

            Assert.IsFalse(plan.MoveEnabled);
            Assert.IsTrue(plan.TeleportEnabled);
            Assert.IsFalse(plan.LeftHand.SmoothMotion);
            Assert.IsFalse(plan.RightHand.SmoothMotion);
        }

        [Test]
        public void SmoothPresetMovesWithHandInputOn()
        {
            var plan = LocomotionPlan.For(MovementMode.Smooth, TurnMode.Snap, true, false, false);

            Assert.IsTrue(plan.MoveEnabled);
            Assert.IsTrue(plan.HandInputEnabled);
        }

        [TestCase(MovementMode.Smooth, TurnMode.Snap, false)]
        [TestCase(MovementMode.Teleport, TurnMode.Snap, false)]
        [TestCase(MovementMode.Smooth, TurnMode.Smooth, false)]
        [TestCase(MovementMode.Smooth, TurnMode.Snap, true)]
        public void CMF_1_GravityIsOffSoNothingDropsTheView(MovementMode movement, TurnMode turning, bool locked)
        {
            Assert.IsFalse(LocomotionPlan.For(movement, turning, true, false, locked).GravityEnabled);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CMF_7_VignetteFollowsThePreset(bool vignette)
        {
            Assert.AreEqual(vignette, LocomotionPlan.For(MovementMode.Smooth, TurnMode.Snap, vignette, false, false).VignetteEnabled);
        }

        // ------------------------------------------------------------------ MRH-13: real walking only in Hub_MR

        [TestCase(MovementMode.Smooth, TurnMode.Snap)]
        [TestCase(MovementMode.Teleport, TurnMode.Snap)]
        [TestCase(MovementMode.Smooth, TurnMode.Smooth)]
        public void MRH_13_LockTurnsOffStickMoveTurnTeleportAndGravity(MovementMode movement, TurnMode turning)
        {
            var plan = LocomotionPlan.For(movement, turning, true, false, true);

            Assert.IsFalse(plan.MoveEnabled);
            Assert.IsFalse(plan.SnapTurnEnabled);
            Assert.IsFalse(plan.SmoothTurnEnabled);
            Assert.IsFalse(plan.TeleportEnabled);
            Assert.IsFalse(plan.GravityEnabled, "Nothing may move the origin; the passthrough room has no floor collider.");
            Assert.IsFalse(plan.HandInputEnabled, "No teleport arc either.");
            Assert.IsFalse(plan.VignetteEnabled, "Nothing moves by stick, so no vignette in passthrough.");
        }

        [Test]
        public void MRH_13_UnlockingRestoresThePreset()
        {
            var locked = LocomotionPlan.For(MovementMode.Smooth, TurnMode.Snap, true, true, true);
            var unlocked = LocomotionPlan.For(MovementMode.Smooth, TurnMode.Snap, true, true, false);

            Assert.IsFalse(locked.MoveEnabled);
            Assert.IsTrue(unlocked.MoveEnabled);
            Assert.IsTrue(unlocked.SnapTurnEnabled);
            Assert.IsTrue(unlocked.RightHand.SmoothMotion, "Left-handed survives the lock.");
        }
    }
}
