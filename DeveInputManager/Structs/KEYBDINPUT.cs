using System;

namespace DeveInputManager.Structs
{
    public struct KEYBDINPUT
    {
        internal short wVk;
        internal short wScan;
        internal int dwFlags;
        internal int time;
        internal IntPtr dwExtraInfo;
    }
}
