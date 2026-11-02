using SLA.Data;

namespace SLA.Player
{
    /// <summary>
    /// Which parts of the rig's locomotion are on for a comfort preset, handedness and lock state.
    /// ComfortSettings applies it to the components; keeping the decisions here makes them testable.
    /// </summary>
    public readonly struct LocomotionPlan
    {
        public readonly bool MoveEnabled;
        public readonly bool SnapTurnEnabled;
        public readonly bool SmoothTurnEnabled;
        public readonly bool TeleportEnabled;
        public readonly bool GravityEnabled;
        /// <summary>The per-hand input managers (teleport arc, stick roles) run.</summary>
        public readonly bool HandInputEnabled;
        public readonly bool VignetteEnabled;
        public readonly bool SprintOnLeftHand;
        public readonly HandLocomotion LeftHand;
        public readonly HandLocomotion RightHand;

        private LocomotionPlan(MovementMode movement, TurnMode turning, bool vignette, bool leftHanded, bool locked)
        {
            // A StickLocomotionLock (Hub_MR for MRH-13; the menu scenes) leaves only real walking, and no vignette.
            MoveEnabled = !locked && movement == MovementMode.Smooth;
            SnapTurnEnabled = !locked && turning == TurnMode.Snap;
            SmoothTurnEnabled = !locked && turning == TurnMode.Smooth;
            TeleportEnabled = !locked;
            // CMF-1/CMF-3: every floor is flat and no gate has levels yet, so gravity stays off and nothing can drop
            // the view (open floor edges, leaning over one, empty Bootstrap during loads). Turn it on together with
            // a fall guard when a gate gets elevation changes. The gravity provider itself keeps running, so the
            // body collider still follows the head.
            GravityEnabled = false;
            HandInputEnabled = !locked;
            VignetteEnabled = vignette && !locked;
            SprintOnLeftHand = HandednessMap.MovementHandIsLeft(leftHanded);
            LeftHand = HandednessMap.ForHand(true, leftHanded, movement, turning);
            RightHand = HandednessMap.ForHand(false, leftHanded, movement, turning);
        }

        public static LocomotionPlan For(MovementMode movement, TurnMode turning, bool vignette, bool leftHanded, bool stickLocomotionLocked) =>
            new LocomotionPlan(movement, turning, vignette, leftHanded, stickLocomotionLocked);
    }
}
