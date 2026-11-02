using NUnit.Framework;
using SLA.Data;
using UnityEditor;
using UnityEngine;

namespace SLA.Tests
{
    /// <summary>
    /// The comfort presets table in docs/design/comfort-and-accessibility.md (CMF-2, CMF-7), checked
    /// against the three assets the rig uses, plus the lookup that picks one from the save.
    /// </summary>
    public class ComfortPresetTests
    {
        private const string PresetFolder = "Assets/_Project/Settings/Comfort";

        private ComfortPreset _recommended;
        private ComfortPreset _comfortable;
        private ComfortPreset _advanced;
        private ComfortPreset[] _all;

        [SetUp]
        public void SetUp()
        {
            _recommended = Load(ComfortPresetNames.Recommended);
            _comfortable = Load(ComfortPresetNames.Comfortable);
            _advanced = Load(ComfortPresetNames.Advanced);
            _all = new[] { _recommended, _comfortable, _advanced };
        }

        private static ComfortPreset Load(string id)
        {
            var preset = AssetDatabase.LoadAssetAtPath<ComfortPreset>($"{PresetFolder}/{id}.asset");
            Assert.IsNotNull(preset, $"{PresetFolder}/{id}.asset is missing; run Tools > SLA > Set Up Player Rig.");
            return preset;
        }

        // ------------------------------------------------------------------ the assets match the spec table

        [Test]
        public void CMF_7_EachPresetAssetCarriesItsSavedName()
        {
            Assert.AreEqual(ComfortPresetNames.Recommended, _recommended.id);
            Assert.AreEqual(ComfortPresetNames.Comfortable, _comfortable.id);
            Assert.AreEqual(ComfortPresetNames.Advanced, _advanced.id);
        }

        [Test]
        public void CMF_2_EveryPresetWalksAt1_5AndSprintsAt3MetersPerSecond()
        {
            foreach (var preset in _all)
            {
                Assert.AreEqual(1.5f, preset.walkSpeedMetersPerSecond, preset.id);
                Assert.AreEqual(3f, preset.sprintSpeedMetersPerSecond, preset.id);
            }
        }

        [Test]
        public void CMF_7_RecommendedIsSmoothMovementSnapTurnAndMediumVignette()
        {
            Assert.AreEqual(MovementMode.Smooth, _recommended.movement);
            Assert.AreEqual(TurnMode.Snap, _recommended.turning);
            Assert.AreEqual(45f, _recommended.snapTurnDegrees);
            Assert.IsTrue(_recommended.vignette);
            Assert.AreEqual(0.7f, _recommended.vignetteApertureSize, 1e-5f);
            Assert.AreEqual(0.2f, _recommended.vignetteFeathering, 1e-5f);
            Assert.IsTrue(_recommended.dashFade);
        }

        [Test]
        public void CMF_7_ComfortableTeleportsSnapTurnsAndHasTheStrongVignette()
        {
            Assert.AreEqual(MovementMode.Teleport, _comfortable.movement);
            Assert.AreEqual(TurnMode.Snap, _comfortable.turning);
            Assert.AreEqual(45f, _comfortable.snapTurnDegrees);
            Assert.IsTrue(_comfortable.vignette);
            Assert.AreEqual(0.55f, _comfortable.vignetteApertureSize, 1e-5f);
            Assert.AreEqual(0.15f, _comfortable.vignetteFeathering, 1e-5f);
            Assert.Less(_comfortable.vignetteApertureSize, _recommended.vignetteApertureSize, "Strong is a smaller aperture than medium.");
            Assert.IsTrue(_comfortable.dashFade);
        }

        [Test]
        public void CMF_7_AdvancedIsSmoothMovementSmoothTurnAt60AndNoVignette()
        {
            Assert.AreEqual(MovementMode.Smooth, _advanced.movement);
            Assert.AreEqual(TurnMode.Smooth, _advanced.turning);
            Assert.AreEqual(60f, _advanced.smoothTurnDegreesPerSecond);
            Assert.IsFalse(_advanced.vignette);
            Assert.IsFalse(_advanced.dashFade, "Advanced dash blinks with no fade.");
        }

        [Test]
        public void CMF_7_VignettesEaseInAndOutOver0_3Seconds()
        {
            foreach (var preset in _all)
            {
                Assert.AreEqual(0.3f, preset.vignetteEaseInSeconds, 1e-5f, preset.id);
                Assert.AreEqual(0.3f, preset.vignetteEaseOutSeconds, 1e-5f, preset.id);
            }
        }

        [Test]
        public void Presets_EveryPresetFadesForATenthOfASecondOnTeleport()
        {
            foreach (var preset in _all)
            {
                Assert.AreEqual(0.1f, preset.teleportFadeSeconds, 1e-5f, preset.id);
            }
        }

        // ------------------------------------------------------------------ lookup from the save

        [TestCase("Recommended")]
        [TestCase("Comfortable")]
        [TestCase("Advanced")]
        public void CMF_7_SavedNameFindsItsPreset(string id)
        {
            Assert.AreEqual(id, ComfortPresetLookup.Find(_all, id, _recommended).id);
        }

        [Test]
        public void CMF_7_SavedNameIgnoresCase()
        {
            Assert.AreSame(_comfortable, ComfortPresetLookup.Find(_all, "comfortable", _recommended));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("Seated")]
        public void CMF_7_EmptyOrUnknownNameFallsBackToRecommended(string id)
        {
            Assert.AreSame(_recommended, ComfortPresetLookup.Find(_all, id, _recommended));
        }

        [Test]
        public void CMF_7_MissingFallbackUsesTheFirstPreset()
        {
            Assert.AreSame(_recommended, ComfortPresetLookup.Find(_all, "Unknown", null));
        }

        [Test]
        public void CMF_7_LookupWorksOnUnsavedPresets()
        {
            var made = ScriptableObject.CreateInstance<ComfortPreset>();
            made.id = "Custom";
            try
            {
                Assert.AreSame(made, ComfortPresetLookup.Find(new[] { made }, "Custom", null));
            }
            finally
            {
                Object.DestroyImmediate(made);
            }
        }
    }
}
