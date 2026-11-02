using UnityEngine;

namespace SLA.UI
{
    /// <summary>Where a menu's eye height came from.</summary>
    public enum EyeHeightSource
    {
        /// <summary>No calibration and no head tracking: leave the menu where it was authored.</summary>
        None,
        /// <summary>The headset's current height, before calibration.</summary>
        Live,
        /// <summary>The calibrated standing height (CMF-13).</summary>
        Calibrated,
    }

    /// <summary>Menu placement at the player's eye level (CMF-13, UI-3, UI-11). Pure, so it's unit-tested.</summary>
    public static class MenuPlacement
    {
        /// <summary>How far below eye level a panel <paramref name="distanceMeters"/> away sits to be <paramref name="degreesBelowEyeLevel"/> down.</summary>
        public static float DropBelowEyeMeters(float distanceMeters, float degreesBelowEyeLevel) =>
            distanceMeters * Mathf.Tan(degreesBelowEyeLevel * Mathf.Deg2Rad);

        /// <summary>
        /// The calibrated standing height when there is one, else the live head height when the headset is
        /// tracking, else none. Heights below <paramref name="minTrackedMeters"/> count as missing.
        /// </summary>
        public static EyeHeightSource ChooseEyeHeight(float calibratedMeters, float liveMeters, float minTrackedMeters, out float eyeMeters)
        {
            if (calibratedMeters >= minTrackedMeters)
            {
                eyeMeters = calibratedMeters;
                return EyeHeightSource.Calibrated;
            }

            if (liveMeters >= minTrackedMeters)
            {
                eyeMeters = liveMeters;
                return EyeHeightSource.Live;
            }

            eyeMeters = 0f;
            return EyeHeightSource.None;
        }
    }
}
