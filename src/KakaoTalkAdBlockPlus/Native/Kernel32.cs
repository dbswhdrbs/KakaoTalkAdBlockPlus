using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace KakaoTalkAdBlockPlus.Native
{
    internal static class Kernel32
    {
        public const uint Th32csSnapProcess = 0x00000002;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SnapshotHandle CreateToolhelp32Snapshot(uint flags, uint processId);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "Process32FirstW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Process32First(SnapshotHandle snapshot, ref ProcessEntry32 entry);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "Process32NextW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool Process32Next(SnapshotHandle snapshot, ref ProcessEntry32 entry);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        /// <summary>PROCESSENTRY32W</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ProcessEntry32
        {
            public uint Size;
            public uint Usage;
            public uint ProcessId;
            public IntPtr DefaultHeapId;
            public uint ModuleId;
            public uint Threads;
            public uint ParentProcessId;
            public int PriorityClassBase;
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string ExeFile;
        }

        /// <summary>스냅숏 핸들을 반드시 닫는다 (원본 2.1.4의 핸들 누수 방지).</summary>
        public sealed class SnapshotHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SnapshotHandle()
                : base(ownsHandle: true)
            {
            }

            protected override bool ReleaseHandle() => CloseHandle(handle);
        }
    }
}
