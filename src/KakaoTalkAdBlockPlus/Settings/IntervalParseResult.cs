namespace KakaoTalkAdBlockPlus.Settings
{
    public enum IntervalParseStatus
    {
        Ok,
        Empty,
        NotANumber,
        TooSmall,
        TooLarge,
    }

    /// <summary>사용자가 입력한 확인 주기 문자열을 해석한 결과.</summary>
    public readonly struct IntervalParseResult
    {
        private IntervalParseResult(IntervalParseStatus status, CheckInterval interval)
        {
            Status = status;
            Interval = interval;
        }

        public IntervalParseStatus Status { get; }

        /// <summary>해석에 성공했을 때의 값. 실패하면 기본값이다.</summary>
        public CheckInterval Interval { get; }

        public static IntervalParseResult Success(CheckInterval interval) =>
            new IntervalParseResult(IntervalParseStatus.Ok, interval);

        public static IntervalParseResult Failure(IntervalParseStatus status) =>
            new IntervalParseResult(status, CheckInterval.Default);
    }
}
