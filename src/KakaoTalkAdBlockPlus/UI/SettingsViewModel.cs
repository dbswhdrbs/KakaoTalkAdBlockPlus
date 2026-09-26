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

        /// <summary>입력칸 값이 잘못됐을 때 보여 줄 문구. 문제가 없으면 null.</summary>
        public string? IntervalError { get; private set; }

        /// <summary>입력칸에서 Enter를 누르거나 포커스가 떠날 때 부른다.</summary>
        public void ApplyIntervalText()
        {
            var result = CheckInterval.ParseSeconds(IntervalText);
            if (result.Status != IntervalParseStatus.Ok)
            {
                IntervalError = "숫자로 입력해 주세요 (예: 0.5)";
                return;
            }

            var interval = result.Interval;
            IntervalError = null;
            _service.Interval = interval;
            _store.Save(new AppSettings(interval));
            IntervalText = interval.ToSecondsText();
        }
    }
}
