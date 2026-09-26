namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>카카오톡 창을 검사해 광고를 제거하는 간격.</summary>
    public readonly struct CheckInterval
    {
        public const int MinMilliseconds = 50;

        private CheckInterval(int milliseconds)
        {
            Milliseconds = milliseconds;
        }

        /// <summary>원본(KakaoTalkAdBlock)의 sleepTime과 같은 100ms.</summary>
        public static CheckInterval Default => new CheckInterval(100);

        public int Milliseconds { get; }

        public static CheckInterval FromMilliseconds(int milliseconds) =>
            new CheckInterval(milliseconds < MinMilliseconds ? MinMilliseconds : milliseconds);
    }
}
