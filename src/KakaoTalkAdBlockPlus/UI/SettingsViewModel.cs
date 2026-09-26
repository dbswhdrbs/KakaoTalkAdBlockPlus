using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Settings;
using KakaoTalkAdBlockPlus.Startup;

namespace KakaoTalkAdBlockPlus.UI
{
    /// <summary>설정창: 확인 주기, 윈도우 시작 시 자동 실행, 현재 상태.</summary>
    public sealed class SettingsViewModel
    {
        private readonly ISettingsStore _store;
        private readonly IAdBlockService _service;
        private readonly IStartupRegistration _startup;

        public SettingsViewModel(ISettingsStore store, IAdBlockService service, IStartupRegistration startup)
        {
            _store = store;
            _service = service;
            _startup = startup;
            IntervalText = store.Load().CheckInterval.ToSecondsText();
        }

        /// <summary>확인 주기 입력칸 (초 단위).</summary>
        public string IntervalText { get; set; }
    }
}
