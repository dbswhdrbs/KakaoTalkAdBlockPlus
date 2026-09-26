using System.Collections.Generic;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Tests.AdBlock
{
    /// <summary>정해 둔 프로세스 ID를 돌려주는 가짜 카카오톡 프로세스 목록.</summary>
    internal sealed class FixedProcessIdSource : IProcessIdSource
    {
        private readonly List<int> _processIds = new List<int>();

        public void Add(int processId) => _processIds.Add(processId);
    }
}
