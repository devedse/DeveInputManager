using System;

namespace DeveInputManager.Structs
{
    public struct MOUSEINPUT
    {
        internal int dx;
        internal int dy;
        internal int mouseData;
        internal int dwFlags;
        internal int time;
        internal IntPtr dwExtraInfo;
    }
}
