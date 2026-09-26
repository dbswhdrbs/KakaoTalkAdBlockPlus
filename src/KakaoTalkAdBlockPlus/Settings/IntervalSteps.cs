namespace KakaoTalkAdBlockPlus.Settings
{
    /// <summary>설정창 슬라이더가 고를 수 있는 확인 주기 단계.</summary>
    public static class IntervalSteps
    {
        private static readonly int[] StepMilliseconds =
        {
            50, 100, 200, 300, 500, 1_000, 2_000, 3_000, 5_000, 10_000, 30_000, 60_000,
        };

        public static int Count => StepMilliseconds.Length;

        public static CheckInterval At(int index) => CheckInterval.FromMilliseconds(StepMilliseconds[index]);
    }
}
