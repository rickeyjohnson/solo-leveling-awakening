using System;
using System.Collections.Generic;

namespace SLA.Data
{
    /// <summary>Finds the comfort preset a save names. Pure, so it's unit-tested.</summary>
    public static class ComfortPresetLookup
    {
        /// <summary>
        /// The preset whose id matches <paramref name="id"/> (ignoring case), or <paramref name="fallback"/>
        /// when the id is empty or unknown (an old or hand-edited save must never leave the player without
        /// movement settings). If the fallback is missing too, the first preset.
        /// </summary>
        public static ComfortPreset Find(IReadOnlyList<ComfortPreset> presets, string id, ComfortPreset fallback)
        {
            if (presets != null && !string.IsNullOrEmpty(id))
            {
                for (int i = 0; i < presets.Count; i++)
                {
                    var preset = presets[i];
                    if (preset != null && string.Equals(preset.id, id, StringComparison.OrdinalIgnoreCase))
                    {
                        return preset;
                    }
                }
            }

            if (fallback != null || presets == null)
            {
                return fallback;
            }

            for (int i = 0; i < presets.Count; i++)
            {
                if (presets[i] != null)
                {
                    return presets[i];
                }
            }

            return null;
        }
    }
}
