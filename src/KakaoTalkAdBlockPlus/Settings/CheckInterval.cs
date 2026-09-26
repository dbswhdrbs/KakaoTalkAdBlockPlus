using System;
using System.Globalization;

namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>카카오톡 창을 검사해 광고를 제거하는 간격.</summary>
    public readonly struct CheckInterval
    {
        public const int MinMilliseconds = 50;
        public const int MaxMilliseconds = 60_000;

        private CheckInterval(int milliseconds)
        {
            Milliseconds = milliseconds;
        }

        /// <summary>원본(KakaoTalkAdBlock)의 sleepTime과 같은 100ms.</summary>
        public static CheckInterval Default => new CheckInterval(100);

        public int Milliseconds { get; }

        public static CheckInterval FromMilliseconds(int milliseconds)
        {
            if (milliseconds < MinMilliseconds) return new CheckInterval(MinMilliseconds);
            if (milliseconds > MaxMilliseconds) return new CheckInterval(MaxMilliseconds);
            return new CheckInterval(milliseconds);
        }

        /// <summary>초 단위 입력 문자열 (예: "0.25" → 250ms)을 해석한다.</summary>
        public static IntervalParseResult ParseSeconds(string? text)
        {
            var seconds = decimal.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);
            var milliseconds = (int)decimal.Round(seconds * 1000m, MidpointRounding.AwayFromZero);
            return new IntervalParseResult(IntervalParseStatus.Ok, new CheckInterval(milliseconds));
        }

        /// <summary>초 단위 표시 문자열 (예: 100ms → "0.1").</summary>
        public string ToSecondsText() =>
            (Milliseconds / 1000m).ToString("0.###", CultureInfo.InvariantCulture);
    }
}
