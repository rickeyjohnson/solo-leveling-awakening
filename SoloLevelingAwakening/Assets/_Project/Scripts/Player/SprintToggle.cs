namespace SLA.Player
{
    /// <summary>
    /// CMF-2 sprint: clicking the movement stick while moving toggles sprint; letting the stick return
    /// to centre ends it. A click with the stick centred does nothing. Pure, so it's unit-tested.
    /// </summary>
    public sealed class SprintToggle
    {
        public bool IsSprinting { get; private set; }

        /// <summary>Call once per frame. Returns whether the player is sprinting this frame.</summary>
        public bool Update(bool stickClickedThisFrame, bool stickPastDeadZone)
        {
            if (!stickPastDeadZone)
            {
                IsSprinting = false;
            }
            else if (stickClickedThisFrame)
            {
                IsSprinting = !IsSprinting;
            }

            return IsSprinting;
        }

        public void Reset() => IsSprinting = false;
    }
}
