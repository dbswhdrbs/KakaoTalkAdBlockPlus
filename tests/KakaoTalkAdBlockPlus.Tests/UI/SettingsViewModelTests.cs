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
    }
}
