namespace SLA.Core
{
    /// <summary>
    /// Turns the stream of state changes into real exits. Pause is an overlay: entering it or resuming
    /// from it exits nothing, and quitting from it exits the state underneath.
    /// </summary>
    public sealed class StateExitTracker
    {
        private GameState _current;

        /// <summary>Starts tracking from <paramref name="initial"/>, which must not be Paused.</summary>
        public StateExitTracker(GameState initial)
        {
            _current = initial;
        }

        /// <summary>
        /// Records a change to <paramref name="next"/>. Returns true, with the state that was left,
        /// only when a real (non-pause) state was exited.
        /// </summary>
        public bool TryExit(GameState next, out GameState exited)
        {
            exited = _current;
            if (next == GameState.Paused || next == _current)
            {
                return false;
            }

            _current = next;
            return true;
        }
    }
}
