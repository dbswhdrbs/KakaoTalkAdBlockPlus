using System.Collections.Generic;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>카카오톡(kakaotalk.exe) 프로세스 ID 목록.</summary>
    public interface IProcessIdSource
    {
        IReadOnlyCollection<int> GetProcessIds();
    }
}
