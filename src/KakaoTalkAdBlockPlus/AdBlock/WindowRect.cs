namespace KakaoTalkAdBlockPlus.AdBlock
{
    /// <summary>GetWindowRect가 돌려주는 화면 좌표 사각형.</summary>
    public readonly struct WindowRect
    {
        public WindowRect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left { get; }

        public int Top { get; }

        public int Right { get; }

        public int Bottom { get; }

        public int Width => Right - Left;

        public int Height => Bottom - Top;
    }
}
