using UnityEngine;

namespace SLA.Player
{
    /// <summary>
    /// CMF-2 movement speed: stopped, walk or sprint, nothing in between and no ramps. The stick's
    /// direction stays analog; its magnitude only decides "moving or not". Pure, so it's unit-tested.
    /// </summary>
    public static class QuantizedMove
    {
        /// <summary>
        /// True when the stick is past the dead zone. The Move action's StickDeadzone processor already
        /// turns anything inside the dead zone into exactly zero, so any other value counts.
        /// </summary>
        public static bool IsPastDeadZone(Vector2 stick) => stick.sqrMagnitude > 0f;

        /// <summary>0 when stopped, otherwise exactly the walk or sprint speed, whatever the deflection.</summary>
        public static float SpeedMetersPerSecond(Vector2 stick, bool sprinting, float walkMetersPerSecond, float sprintMetersPerSecond)
        {
            if (!IsPastDeadZone(stick))
            {
                return 0f;
            }

            return sprinting ? sprintMetersPerSecond : walkMetersPerSecond;
        }

        /// <summary>The stick's direction at unit length (zero when stopped), so a diagonal is no faster.</summary>
        public static Vector2 Direction(Vector2 stick) => IsPastDeadZone(stick) ? stick.normalized : Vector2.zero;
    }
}
