namespace KakaoTalkAdBlockPlus.AdBlock
{
    public interface IProcessNameResolver
    {
        /// <summary>실행 파일 이름 (예: "KakaoTalk.exe"). 알 수 없으면 null.</summary>
        string? GetImageName(int processId);
    }
}
