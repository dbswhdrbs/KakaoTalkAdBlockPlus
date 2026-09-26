using System;
using System.Collections.Generic;

namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>광고 제거 엔진이 쓰는 Win32 창 기능. 테스트에서는 가짜 창 트리로 바꿔 끼운다.</summary>
    public interface IWindowApi
    {
        /// <summary>EnumWindows: 최상위 창 목록.</summary>
        IReadOnlyList<IntPtr> GetTopLevelWindows();

        /// <summary>EnumChildWindows: 모든 자손 창 (전위 순서).</summary>
        IReadOnlyList<IntPtr> GetDescendantWindows(IntPtr parent);

        /// <summary>GetWindowThreadProcessId: 창을 만든 프로세스.</summary>
        int GetProcessId(IntPtr window);

        string GetClassName(IntPtr window);

        string GetText(IntPtr window);

        /// <summary>GetParent: 자식 창이면 부모, 최상위 창이면 소유자(없으면 IntPtr.Zero).</summary>
        IntPtr GetParent(IntPtr window);

        /// <summary>WM_CLOSE를 보낸다.</summary>
        void Close(IntPtr window);
    }
}
