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
    }
}
