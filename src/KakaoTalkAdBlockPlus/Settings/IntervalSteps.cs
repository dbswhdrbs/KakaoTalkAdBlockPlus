using System;

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

        /// <summary>주어진 주기와 가장 가까운 단계. 두 단계의 한가운데면 짧은 쪽을 고른다.</summary>
        public static int IndexOfNearest(CheckInterval interval)
        {
            var nearest = 0;
            for (var i = 1; i < StepMilliseconds.Length; i++)
            {
                if (Distance(i) < Distance(nearest)) nearest = i;
            }

            return nearest;

            int Distance(int index) => Math.Abs(StepMilliseconds[index] - interval.Milliseconds);
        }
    }
}
