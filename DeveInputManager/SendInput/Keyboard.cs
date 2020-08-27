using DeveInputManager.Structs;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeveInputManager.SendInput
{
    /// <summary>
    /// Provides methods to send keyboard input that also works in DirectX games.
    /// </summary>
    public static class Keyboard
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
        public static extern uint MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);


        [DllImport("user32.dll")]
        public static extern uint MapVirtualKeyEx(uint uCode, MapVirtualKeyMapTypes uMapType, IntPtr dwhkl);


        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);


        private static ScanKey GetScanKey(Keys vKey)
        {
            uint uintVKey = (uint)vKey;

            switch (vKey)
            {
                case Keys.Alt:
                    uintVKey = (uint)Keys.Menu;
                    break;
                case Keys.Control:
                    uintVKey = (uint)Keys.ControlKey;
                    break;
                case Keys.Shift:
                    uintVKey = (uint)Keys.ShiftKey;
                    break;
                default:
                    break;
            }


            uint scanCode = MapVirtualKey(uintVKey, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
            bool extended = uintVKey == (uint)Keys.RMenu || uintVKey == (uint)Keys.RControlKey || uintVKey == (uint)Keys.Left || uintVKey == (uint)Keys.Right || uintVKey == (uint)Keys.Up || uintVKey == (uint)Keys.Down || uintVKey == (uint)Keys.Home || uintVKey == (uint)Keys.Delete || uintVKey == (uint)Keys.PageUp || uintVKey == (uint)Keys.PageDown || uintVKey == (uint)Keys.End || uintVKey == (uint)Keys.Insert || uintVKey == (uint)Keys.NumLock || uintVKey == (uint)Keys.PrintScreen || uintVKey == (uint)Keys.Divide;
            return new ScanKey(scanCode, extended);
        }

        /// <summary>
        /// Sends shortcut keys (key down and up) signals.
        /// </summary>
        /// <param name="kCode">The array of keys to send as a shortcut.</param>
        /// <param name="delay">The delay in milliseconds between the key down and up events.</param>
        public static void ShortcutKeys(Keys[] kCode, int delay = 0)
        {
            KeyPressStruct KeysPress = new KeyPressStruct(kCode, delay);
            Thread t = new Thread(() => KeyPressThread(KeysPress));
        }

        /// <summary>
        /// Sends a key down signal.
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        public static void KeyDown(Keys kCode)
        {
            ScanKey sKey = GetScanKey(kCode);
            INPUT input = new INPUT
            {
                dwType = INPUT_KEYBOARD
            };
            input.mkhi.ki = new KEYBDINPUT
            {
                wScan = (short)sKey.ScanCode,
                dwExtraInfo = IntPtr.Zero,
                dwFlags = (int)(KEYEVENTF_SCANCODE | (sKey.Extended ? KEYEVENTF_EXTENDEDKEY : uint.MinValue))
            };
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        /// <summary>
        /// Sends a key up signal.
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        public static void KeyUp(Keys kCode)
        {
            ScanKey sKey = GetScanKey(kCode);
            INPUT input = new INPUT
            {
                dwType = INPUT_KEYBOARD
            };
            input.mkhi.ki = new KEYBDINPUT
            {
                wScan = (short)sKey.ScanCode,
                dwExtraInfo = IntPtr.Zero,
                dwFlags = (int)(KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP | (sKey.Extended ? KEYEVENTF_EXTENDEDKEY : uint.MinValue))
            };
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            SendInput(1, ref input, cbSize);
        }

        /// <summary>
        /// Sends a key press signal (key down and up).
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        public static void KeyPress(Keys kCode)
        {
            KeyDown(kCode);
            KeyUp(kCode);
        }

        /// <summary>
        /// Sends a key press signal (key down and up).
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        /// <param name="delay">The delay to set between the key down and up commands.</param>

        public static void KeyPress(Keys kCode, int delay)
        {
            Keys[] sendKeys = new[] { kCode };
            KeyPressStruct keysPress = new KeyPressStruct(sendKeys, delay);
            Thread t = new Thread(() => KeyPressThread(keysPress));
            t.Start();
        }

        private static void KeyPressThread(KeyPressStruct keysP)
        {
            foreach (Keys k in keysP.Keys)
            {
                KeyDown(k);
            }

            if (keysP.Delay > 0)
            {
                Thread.Sleep(keysP.Delay);
            }

            foreach (Keys k in keysP.Keys)
            {
                KeyUp(k);
            }
        }
    }
}
