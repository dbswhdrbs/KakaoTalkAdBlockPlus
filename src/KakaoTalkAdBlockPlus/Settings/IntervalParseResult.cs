namespace KakaoTalkAdBlockPlus.Settings
{
    public enum IntervalParseStatus
    {
        Ok,
        Empty,
        NotANumber,
    }

    /// <summary>사용자가 입력한 확인 주기 문자열을 해석한 결과.</summary>
    public readonly struct IntervalParseResult
    {
        public IntervalParseResult(IntervalParseStatus status, CheckInterval interval)
        {
            Status = status;
            Interval = interval;
        }

        public IntervalParseStatus Status { get; }

        public CheckInterval Interval { get; }
    }
}
