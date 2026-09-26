using System;
using System.Collections.Generic;
using System.Linq;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>
    /// 최상위 창을 가진 프로세스 중 실행 파일 이름이 같은 것을 찾는다.
    /// 전체 프로세스 스냅숏(원본 방식)은 프로세스가 많은 PC에서 한 번에 10ms 넘게 걸려 이 방법을 쓴다.
    /// </summary>
    public sealed class WindowOwnerProcessIdSource : IProcessIdSource
    {
        private readonly IWindowApi _windows;
        private readonly IProcessNameResolver _names;
        private readonly string _imageName;

        public WindowOwnerProcessIdSource(IWindowApi windows, IProcessNameResolver names, string imageName)
        {
            _windows = windows;
            _names = names;
            _imageName = imageName;
        }

        public IReadOnlyCollection<int> GetProcessIds() =>
            _windows.GetTopLevelWindows()
                .Select(_windows.GetProcessId)
                .Distinct()
                .Where(processId => string.Equals(_names.GetImageName(processId), _imageName, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}
