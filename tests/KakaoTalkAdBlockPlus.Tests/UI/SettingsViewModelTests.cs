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
    }
}
