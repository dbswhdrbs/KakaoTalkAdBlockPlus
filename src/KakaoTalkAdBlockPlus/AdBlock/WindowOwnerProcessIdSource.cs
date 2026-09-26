using System;
using System.Collections.Generic;
using System.Linq;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>
    /// 최상위 창을 가진 프로세스 중 실행 파일 이름이 같은 것을 찾는다.
    /// 전체 프로세스 스냅숏은 프로세스가 많은 PC에서 한 번에 10ms 넘게 걸려 이 방법을 쓴다.
    /// 프로세스 이름은 한 번만 조회해 기억한다.
    /// </summary>
    public sealed class WindowOwnerProcessIdSource : IProcessIdSource
    {
        private readonly IWindowApi _windows;
        private readonly IProcessNameResolver _names;
        private readonly string _imageName;
        private readonly Dictionary<int, bool> _isTargetByProcessId = new Dictionary<int, bool>();

        public WindowOwnerProcessIdSource(IWindowApi windows, IProcessNameResolver names, string imageName)
        {
            _windows = windows;
            _names = names;
            _imageName = imageName;
        }

        public IReadOnlyCollection<int> GetProcessIds()
        {
            var owners = new HashSet<int>(_windows.GetTopLevelWindows().Select(_windows.GetProcessId));

            // 창이 없어진 프로세스는 잊는다: 끝난 프로세스의 PID를 다른 프로그램이 받아도 헷갈리지 않게.
            foreach (var gone in _isTargetByProcessId.Keys.Where(processId => !owners.Contains(processId)).ToList())
            {
                _isTargetByProcessId.Remove(gone);
            }

            return owners.Where(IsTarget).ToList();
        }

        private bool IsTarget(int processId)
        {
            if (!_isTargetByProcessId.TryGetValue(processId, out var isTarget))
            {
                isTarget = string.Equals(_names.GetImageName(processId), _imageName, StringComparison.OrdinalIgnoreCase);
                _isTargetByProcessId[processId] = isTarget;
            }

            return isTarget;
        }
    }
}
