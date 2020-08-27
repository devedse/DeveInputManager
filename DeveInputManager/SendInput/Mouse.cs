using DeveInputManager.Structs;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeveInputManager.SendInput
{
    /// <summary>
    /// Provides methods to send mouse input that also works in DirectX games.
    /// </summary>
    public static class Mouse
    {
        [DllImport("user32.dll")]
        private static extern int SendInput(int cInputs, ref INPUT pInputs, int cbSize);

        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        private const uint KEYEVENTF_KEYUP = 0x2;
        private const uint KEYEVENTF_UNICODE = 0x4;
        private const uint KEYEVENTF_SCANCODE = 0x8;
        private const uint XBUTTON1 = 0x1;
        private const uint XBUTTON2 = 0x2;
        private const uint MOUSEEVENTF_MOVE = 0x1;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x2;
        private const uint MOUSEEVENTF_LEFTUP = 0x4;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x8;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x40;
        private const uint MOUSEEVENTF_XDOWN = 0x80;
        private const uint MOUSEEVENTF_XUP = 0x100;
        private const uint MOUSEEVENTF_WHEEL = 0x800;
        private const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;


        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int smIndex);

        private const int SM_SWAPBUTTON = 23;

        /// <summary>
        /// Returns true if mouse buttons are swapped
        /// </summary>
        public static bool IsLeftHanded
        {
            get
            {
                try
                {
                    return GetSystemMetrics(SM_SWAPBUTTON) == 1;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sends a mouse button signal. To send a scroll use the Scroll method.
        /// </summary>
        /// <param name="mButton">The button to send.</param>
        private static void SendButton(MouseButtons mButton)
        {
            INPUT input = new INPUT
            {
                dwType = INPUT_MOUSE
            };
            input.mkhi.mi = new MOUSEINPUT
            {
                dwExtraInfo = IntPtr.Zero,
                dwFlags = (int)mButton,
                dx = 0,
                dy = 0
            };
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        /// <summary>
        /// Sends a mouse press signal (down and up).
        /// </summary>
        /// <param name="mKey">The key to press.</param>
        /// <param name="delay">The delay to set between the events.</param>
        public static void PressButton(MouseKeys mKey, int delay = 0)
        {
            ButtonDown(mKey);

            if (delay > 0)
            {
                Thread.Sleep(delay);
            }

            ButtonUp(mKey);
        }

        /// <summary>
        /// Send a mouse button down signal.
        /// </summary>
        /// <param name="mKey">The mouse key to send as mouse button down.</param>
        public static void ButtonDown(MouseKeys mKey)
        {
            switch (mKey)
            {
                case MouseKeys.Left:
                    SendButton(MouseButtons.LeftDown);
                    break;
                case MouseKeys.Right:
                    SendButton(MouseButtons.RightDown);
                    break;
                case MouseKeys.Middle:
                    SendButton(MouseButtons.MiddleDown);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Send a mouse button up signal.
        /// </summary>
        /// <param name="mKey">The mouse key to send as mouse button up.</param>
        public static void ButtonUp(MouseKeys mKey)
        {
            switch (mKey)
            {
                case MouseKeys.Left:
                    SendButton(MouseButtons.LeftUp);
                    break;
                case MouseKeys.Right:
                    SendButton(MouseButtons.RightUp);
                    break;
                case MouseKeys.Middle:
                    SendButton(MouseButtons.MiddleUp);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Moves the mouse to a certain location on the screen.
        /// </summary>
        /// <param name="x">The x location to move the mouse.</param>
        /// <param name="y">The y location to move the mouse</param>
        public static void Move(int x, int y)
        {
            //INPUT input = new INPUT();
            //input.dwType = INPUT_MOUSE;
            //input.mkhi.mi = new MOUSEINPUT();
            //input.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            //input.mkhi.mi.dwFlags = (int)(MOUSEEVENTF_ABSOLUTE + MOUSEEVENTF_MOVE);
            //input.mkhi.mi.dx = x * (65535 / Screen.PrimaryScreen.Bounds.Width);
            //input.mkhi.mi.dy = x * (65535 / Screen.PrimaryScreen.Bounds.Height);
            //int cbSize = Marshal.SizeOf(typeof(INPUT));
            //SendInput(1, ref input, cbSize);
        }

        /// <summary>
        /// Moves the mouse to a location relative to the current one.
        /// </summary>
        /// <param name="x">The amount of pixels to move the mouse on the x axis.</param>
        /// <param name="y">The amount of pixels to move the mouse on the y axis.</param>
        public static void MoveRelative(int x, int y)
        {
            INPUT input = new INPUT
            {
                dwType = INPUT_MOUSE
            };
            input.mkhi.mi = new MOUSEINPUT
            {
                dwExtraInfo = IntPtr.Zero,
                dwFlags = (int)MOUSEEVENTF_MOVE,
                dx = x,
                dy = y
            };
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        /// <summary>
        /// Sends a scroll signal with a specific direction to scroll.
        /// </summary>
        /// <param name="direction">The direction to scroll.</param>
        public static void Scroll(ScrollDirection direction)
        {
            INPUT input = new INPUT
            {
                dwType = INPUT_MOUSE
            };
            input.mkhi.mi = new MOUSEINPUT
            {
                dwExtraInfo = IntPtr.Zero,
                dwFlags = (int)MouseButtons.Wheel,
                mouseData = (int)direction,
                dx = 0,
                dy = 0
            };
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }
    }
}
