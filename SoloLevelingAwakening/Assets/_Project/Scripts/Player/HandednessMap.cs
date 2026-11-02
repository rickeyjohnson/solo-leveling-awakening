using SLA.Data;

namespace SLA.Player
{
    /// <summary>
    /// What one controller's thumbstick does, in the XRI Starter Assets' ControllerInputActionManager
    /// terms: with smooth motion the stick moves the player; without it, pushing forward aims the
    /// teleport arc and sideways turns (snap or smooth).
    /// </summary>
    public readonly struct HandLocomotion
    {
        public readonly bool SmoothMotion;
        public readonly bool SmoothTurn;

        public HandLocomotion(bool smoothMotion, bool smoothTurn)
        {
            SmoothMotion = smoothMotion;
            SmoothTurn = smoothTurn;
        }
    }

    /// <summary>
    /// CMF-8 left-handed mode: right-handed players move with the left stick and turn with the right;
    /// left-handed mode swaps the two roles. Pure, so it's unit-tested.
    /// </summary>
    public static class HandednessMap
    {
        public static bool MovementHandIsLeft(bool leftHanded) => !leftHanded;

        /// <summary>The role of the left (<paramref name="leftHand"/> true) or right controller.</summary>
        public static HandLocomotion ForHand(bool leftHand, bool leftHanded, MovementMode movement, TurnMode turning)
        {
            bool isMovementHand = leftHand == MovementHandIsLeft(leftHanded);
            return new HandLocomotion(isMovementHand && movement == MovementMode.Smooth, turning == TurnMode.Smooth);
        }
    }
}
