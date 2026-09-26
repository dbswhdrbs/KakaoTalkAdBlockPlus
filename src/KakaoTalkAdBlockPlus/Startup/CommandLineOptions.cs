using System;
using System.Collections.Generic;
using System.Linq;

namespace KakaoTalkAdBlockPlus.Startup
{
    /// <summary>실행 인자. 윈도우 시작 시 자동 실행이면 Run 값에 <c>--autostart</c>가 붙는다.</summary>
    public sealed class CommandLineOptions
    {
        public const string AutostartFlag = "--autostart";

        private CommandLineOptions(bool isAutostart)
        {
            IsAutostart = isAutostart;
        }

        public bool IsAutostart { get; }

        public static CommandLineOptions Parse(IEnumerable<string> arguments) =>
            new CommandLineOptions(arguments.Any(a => string.Equals(a, AutostartFlag, StringComparison.OrdinalIgnoreCase)));
    }
}
