namespace KakaoTalkAdBlockPlus.AdBlock
{
    public interface IAdBlockEngine
    {
        /// <summary>카카오톡 창을 한 번 검사해 광고를 없앤다.</summary>
        AdBlockReport RunOnce();
    }
}
