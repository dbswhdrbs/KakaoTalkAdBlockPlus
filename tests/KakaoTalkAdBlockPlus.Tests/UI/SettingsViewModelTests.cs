using KakaoTalkAdBlockPlus.Settings;
using KakaoTalkAdBlockPlus.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KakaoTalkAdBlockPlus.Tests.UI
{
    [TestClass]
    public class SettingsViewModelTests
    {
        private readonly FakeSettingsStore _store = new FakeSettingsStore();
        private readonly FakeAdBlockService _service = new FakeAdBlockService();
        private readonly FakeStartupRegistration _startup = new FakeStartupRegistration();

        private SettingsViewModel CreateViewModel() => new SettingsViewModel(_store, _service, _startup);

        [TestMethod]
        public void ShouldShowCurrentIntervalInSeconds()
        {
            _store.Stored = new AppSettings(CheckInterval.FromMilliseconds(250));

            Assert.AreEqual("0.25", CreateViewModel().IntervalText);
        }

        [TestMethod]
        public void ShouldApplyAndSaveValidIntervalText()
        {
            var viewModel = CreateViewModel();

            viewModel.IntervalText = " 0,5초 ";
            viewModel.ApplyIntervalText();

            Assert.AreEqual(500, _service.Interval.Milliseconds);
            Assert.AreEqual(500, _store.Stored.CheckInterval.Milliseconds);
            Assert.AreEqual("0.5", viewModel.IntervalText);
            Assert.IsNull(viewModel.IntervalError);
        }

        [TestMethod]
        public void ShouldShowErrorForInvalidIntervalText()
        {
            _service.Interval = CheckInterval.FromMilliseconds(300);
            var viewModel = CreateViewModel();

            viewModel.IntervalText = "빠르게";
            viewModel.ApplyIntervalText();

            Assert.AreEqual("숫자로 입력해 주세요 (예: 0.5)", viewModel.IntervalError);
            Assert.AreEqual(300, _service.Interval.Milliseconds);
            Assert.AreEqual(0, _store.Saved.Count);
        }

        [DataTestMethod]
        [DataRow("0.01")]
        [DataRow("100")]
        public void ShouldShowRangeErrorForOutOfRangeText(string text)
        {
            var viewModel = CreateViewModel();

            viewModel.IntervalText = text;
            viewModel.ApplyIntervalText();

            Assert.AreEqual("0.05초 ~ 60초 사이로 입력해 주세요", viewModel.IntervalError);
            Assert.AreEqual(0, _store.Saved.Count);
        }

        [TestMethod]
        public void ShouldApplySliderStep()
        {
            var viewModel = CreateViewModel();

            viewModel.IntervalStepIndex = 4;

            Assert.AreEqual(500, _service.Interval.Milliseconds);
            Assert.AreEqual(500, _store.Stored.CheckInterval.Milliseconds);
            Assert.AreEqual("0.5", viewModel.IntervalText);
        }

        [TestMethod]
        public void ShouldMoveSliderToNearestStepWhenTextApplied()
        {
            _store.Stored = new AppSettings(CheckInterval.FromMilliseconds(3_000));
            var viewModel = CreateViewModel();
            Assert.AreEqual(7, viewModel.IntervalStepIndex, "처음 슬라이더 위치: 3초");

            viewModel.IntervalText = "0.25";
            viewModel.ApplyIntervalText();

            Assert.AreEqual(2, viewModel.IntervalStepIndex, "0.25초와 가장 가까운 단계: 0.2초");
            Assert.AreEqual(250, _service.Interval.Milliseconds, "슬라이더 단계가 아니라 입력한 값이 적용돼야 한다");
        }

        [TestMethod]
        public void ShouldResetIntervalToDefault()
        {
            _store.Stored = new AppSettings(CheckInterval.FromMilliseconds(5_000));
            var viewModel = CreateViewModel();
            Assert.IsFalse(viewModel.IsDefaultInterval);

            viewModel.ResetInterval();

            Assert.AreEqual(100, _service.Interval.Milliseconds);
            Assert.AreEqual(100, _store.Stored.CheckInterval.Milliseconds);
            Assert.AreEqual("0.1", viewModel.IntervalText);
            Assert.AreEqual(1, viewModel.IntervalStepIndex);
            Assert.IsTrue(viewModel.IsDefaultInterval);
        }
    }
}
