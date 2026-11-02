using UnityEngine;

namespace SLA.Data
{
    /// <summary>
    /// One row set of the comfort presets table (docs/design/comfort-and-accessibility.md): how the
    /// player moves, turns and is shielded from vection. Three assets live in Settings/Comfort;
    /// the rig's ComfortSettings applies the one named in the save's settings.comfortPreset.
    /// Field defaults are the Recommended preset.
    /// </summary>
    [CreateAssetMenu(menuName = "SLA/Config/Comfort Preset")]
    public class ComfortPreset : ScriptableObject
    {
        [Tooltip("Saved in settings.comfortPreset. One of ComfortPresetNames.")]
        public string id = ComfortPresetNames.Recommended;

        [Header("Movement (CMF-2: stopped, walk or sprint; no acceleration)")]
        public MovementMode movement = MovementMode.Smooth;
        public float walkSpeedMetersPerSecond = 1.5f;
        public float sprintSpeedMetersPerSecond = 3f;

        [Header("Turning")]
        public TurnMode turning = TurnMode.Snap;
        public float snapTurnDegrees = 45f;
        [Tooltip("Smooth turning only. The player-adjustable slider arrives with the settings menu.")]
        public float smoothTurnDegreesPerSecond = 60f;

        [Header("Tunneling vignette (only while moving or turning by stick)")]
        public bool vignette = true;
        [Tooltip("XRI aperture: 1 is no vignette, smaller is stronger.")]
        [Range(0f, 1f)] public float vignetteApertureSize = 0.7f;
        [Range(0f, 1f)] public float vignetteFeathering = 0.2f;
        public float vignetteEaseInSeconds = 0.3f;
        public float vignetteEaseOutSeconds = 0.3f;

        [Header("Teleport and dash")]
        [Tooltip("Fade to black, and back, around a teleport. 0 = no fade.")]
        public float teleportFadeSeconds = 0.1f;
        [Tooltip("Dash blinks with a brief fade (read by the Dash skill).")]
        public bool dashFade = true;
    }
}
