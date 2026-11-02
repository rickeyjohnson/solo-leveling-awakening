using NUnit.Framework;
using SLA.UI;
using UnityEngine;

namespace SLA.Tests
{
    /// <summary>
    /// Standing height calibration (QST-8, CMF-13) and eye-level menu placement (CMF-13, UI-3, UI-11).
    /// </summary>
    public class HeightCalibrationTests
    {
        private const float Window = 2f;
        private const float MinTracked = 0.5f;
        private const float Frame = 1f / 72f;

        private static StandingHeightCalibration Run(float height, float seconds)
        {
            var calibration = new StandingHeightCalibration(Window, MinTracked);
            calibration.Begin();
            for (float t = 0f; t < seconds; t += Frame)
            {
                calibration.Sample(height, Frame);
            }
            return calibration;
        }

        // ------------------------------------------------------------------ QST-8 / CMF-13

        [Test]
        public void QST_8_AveragesHeadHeightOverTwoSeconds()
        {
            var calibration = new StandingHeightCalibration(Window, MinTracked);
            calibration.Begin();
            for (int i = 0; i < 72; i++) calibration.Sample(1.60f, Frame);
            for (int i = 0; i < 80 && calibration.IsRunning; i++) calibration.Sample(1.70f, Frame);

            Assert.IsTrue(calibration.TryGetStandingHeight(out float height));
            Assert.AreEqual(1.65f, height, 1e-3f);
        }

        [Test]
        public void QST_8_IsNotDoneBeforeTheWindowEnds()
        {
            var calibration = Run(1.70f, 1.9f);

            Assert.IsTrue(calibration.IsRunning);
            Assert.IsFalse(calibration.IsDone);
            Assert.IsFalse(calibration.TryGetStandingHeight(out _));
            Assert.AreEqual(0.95f, calibration.Progress01, 0.02f);
        }

        [Test]
        public void QST_8_FinishesOnceTheWindowHasPassed()
        {
            var calibration = Run(1.70f, 2.05f);

            Assert.IsFalse(calibration.IsRunning);
            Assert.IsTrue(calibration.IsDone);
            Assert.AreEqual(1f, calibration.Progress01);
        }

        [Test]
        public void QST_8_LongFramesCountForMoreThanShortOnes()
        {
            var calibration = new StandingHeightCalibration(Window, MinTracked);
            calibration.Begin();
            calibration.Sample(1.80f, 0.5f);  // a hitch
            calibration.Sample(1.60f, 1.5f);

            Assert.IsTrue(calibration.TryGetStandingHeight(out float height));
            Assert.AreEqual(1.65f, height, 1e-4f);
        }

        [Test]
        public void QST_8_SamplesAfterTheWindowAreIgnored()
        {
            var calibration = Run(1.70f, 2.05f);
            calibration.Sample(0.4f, 5f);

            Assert.IsTrue(calibration.TryGetStandingHeight(out float height));
            Assert.AreEqual(1.70f, height, 1e-4f);
        }

        [Test]
        public void QST_8_MeasuringAgainStartsFromScratch()
        {
            var calibration = Run(1.50f, 2.05f);
            calibration.Begin();
            for (float t = 0f; t < 2.05f; t += Frame) calibration.Sample(1.80f, Frame);

            Assert.IsTrue(calibration.TryGetStandingHeight(out float height));
            Assert.AreEqual(1.80f, height, 1e-4f);
        }

        [Test]
        public void CMF_13_NoHeadTrackingGivesNoHeight()
        {
            // An untracked headset (e.g. the Editor without one) reports the head at the floor.
            var calibration = Run(0f, 2.05f);

            Assert.IsTrue(calibration.IsDone);
            Assert.IsFalse(calibration.TryGetStandingHeight(out _));
            Assert.AreEqual(0f, calibration.AverageHeightMeters, 1e-5f);
        }

        [Test]
        public void CMF_13_FramesBeforeTrackingStartsDoNotDragTheAverageDown()
        {
            var calibration = new StandingHeightCalibration(Window, MinTracked);
            calibration.Begin();
            for (int i = 0; i < 36; i++) calibration.Sample(0f, Frame);
            for (int i = 0; i < 200 && calibration.IsRunning; i++) calibration.Sample(1.70f, Frame);

            Assert.IsTrue(calibration.TryGetStandingHeight(out float height));
            Assert.AreEqual(1.70f, height, 1e-4f);
        }

        [Test]
        public void CMF_13_SamplesBeforeBeginAreIgnored()
        {
            var calibration = new StandingHeightCalibration(Window, MinTracked);
            calibration.Sample(1.70f, 3f);

            Assert.IsFalse(calibration.IsRunning);
            Assert.IsFalse(calibration.IsDone);
            Assert.IsFalse(calibration.TryGetStandingHeight(out _));
        }

        // ------------------------------------------------------------------ CMF-13 / UI-3: menus at eye level

        [Test]
        public void CMF_13_MenusUseTheCalibratedHeightOnceThereIsOne()
        {
            var source = MenuPlacement.ChooseEyeHeight(1.62f, 1.40f, MinTracked, out float eye);

            Assert.AreEqual(EyeHeightSource.Calibrated, source);
            Assert.AreEqual(1.62f, eye);
        }

        [Test]
        public void CMF_13_BeforeCalibrationMenusUseTheLiveHeadHeight()
        {
            var source = MenuPlacement.ChooseEyeHeight(0f, 1.55f, MinTracked, out float eye);

            Assert.AreEqual(EyeHeightSource.Live, source);
            Assert.AreEqual(1.55f, eye);
        }

        [Test]
        public void CMF_13_WithoutTrackingOrCalibrationMenusStayWhereTheyAre()
        {
            Assert.AreEqual(EyeHeightSource.None, MenuPlacement.ChooseEyeHeight(0f, 0f, MinTracked, out _));
        }

        [TestCase(10f)]
        [TestCase(15f)]
        public void UI_3_PanelCentreSitsTheChosenAngleBelowEyeLevel(float degrees)
        {
            const float distance = 1.6f;
            float drop = MenuPlacement.DropBelowEyeMeters(distance, degrees);

            Assert.AreEqual(degrees, Mathf.Atan2(drop, distance) * Mathf.Rad2Deg, 1e-3f);
        }
    }
}
