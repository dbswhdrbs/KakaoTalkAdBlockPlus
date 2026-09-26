using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using KakaoTalkAdBlockPlus.AdBlock;
using Microsoft.Win32.SafeHandles;

namespace KakaoTalkAdBlockPlus.Native
{
    /// <summary>PID로 실행 파일 이름을 얻는다 (권한이 필요 없는 PROCESS_QUERY_LIMITED_INFORMATION).</summary>
    public sealed class ProcessNameResolver : IProcessNameResolver
    {
        private const uint ProcessQueryLimitedInformation = 0x1000;

        public string? GetImageName(int processId)
        {
            using var process = OpenProcess(ProcessQueryLimitedInformation, false, (uint)processId);
            if (process.IsInvalid) return null;

            var path = new StringBuilder(1024);
            var length = path.Capacity;
            return QueryFullProcessImageName(process, 0, path, ref length) ? Path.GetFileName(path.ToString()) : null;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern SafeProcessHandle OpenProcess(uint access, bool inheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "QueryFullProcessImageNameW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryFullProcessImageName(SafeProcessHandle process, uint flags, StringBuilder path, ref int size);
    }
}
