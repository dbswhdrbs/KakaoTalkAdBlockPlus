using System;
using System.Collections.Generic;
using KakaoTalkAdBlockPlus.AdBlock;
using KakaoTalkAdBlockPlus.Settings;
using KakaoTalkAdBlockPlus.Startup;

namespace KakaoTalkAdBlockPlus.Tests.UI
{
    internal sealed class FakeSettingsStore : ISettingsStore
    {
        public AppSettings Stored { get; set; } = AppSettings.Default;

        public List<AppSettings> Saved { get; } = new List<AppSettings>();

        /// <summary>지정하면 Save가 이 예외를 던진다 (디스크 권한 문제 흉내).</summary>
        public Exception? SaveFailure { get; set; }

        public AppSettings Load() => Stored;

        public void Save(AppSettings settings)
        {
            if (SaveFailure != null) throw SaveFailure;

            Saved.Add(settings);
            Stored = settings;
        }
    }

    internal sealed class FakeAdBlockService : IAdBlockService
    {
        public CheckInterval Interval { get; set; } = CheckInterval.Default;

        public AdBlockStatus Status { get; set; } = AdBlockStatus.Initial;
    }

    internal sealed class FakeStartupRegistration : IStartupRegistration
    {
        public bool IsEnabled { get; set; }

        /// <summary>지정하면 Enable/Disable이 이 예외를 던진다 (레지스트리 접근 실패 흉내).</summary>
        public Exception? Failure { get; set; }

        public void Enable() => IsEnabled = Failure == null ? true : throw Failure;

        public void Disable() => IsEnabled = Failure == null ? false : throw Failure;
    }
}
