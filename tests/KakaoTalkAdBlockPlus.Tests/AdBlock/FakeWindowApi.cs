using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    /// <summary>진짜 창 대신 메모리 속 창 트리를 흉내 내고, 엔진이 한 일을 기록한다.</summary>
    internal sealed class FakeWindowApi : IWindowApi
    {
    }
}
