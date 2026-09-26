using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KakaoTalkAdBlockPlus.AdBlock;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>프로세스 스냅숏에서 실행 파일 이름(대소문자 무시)이 같은 프로세스를 찾는다. 원본과 같은 방식.</summary>
    public sealed class ToolhelpProcessIdSource : IProcessIdSource
    {
        private readonly string _imageName;

        public ToolhelpProcessIdSource(string imageName)
        {
            _imageName = imageName;
        }

        public IReadOnlyCollection<int> GetProcessIds()
        {
            var ids = new List<int>();
            using var snapshot = Kernel32.CreateToolhelp32Snapshot(Kernel32.Th32csSnapProcess, 0);
            if (snapshot.IsInvalid) return ids;

            var entry = new Kernel32.ProcessEntry32 { Size = (uint)Marshal.SizeOf<Kernel32.ProcessEntry32>() };
            for (var found = Kernel32.Process32First(snapshot, ref entry); found; found = Kernel32.Process32Next(snapshot, ref entry))
            {
                if (string.Equals(entry.ExeFile, _imageName, StringComparison.OrdinalIgnoreCase)) ids.Add((int)entry.ProcessId);
            }

            return ids;
        }
    }
}
