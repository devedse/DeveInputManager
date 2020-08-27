using DeveInputManager.Structs;
using System.Runtime.InteropServices;
using System.Threading;

namespace DeveInputManager.SendInput
{
    /// <summary>
    /// Provides methods to send keyboard input. The keys are being sent virtually and cannot be used with DirectX.
    /// </summary>
    public static class VirtualKeyboard
    {
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "keybd_event", ExactSpelling = true, SetLastError = true)]
        private static extern bool keybd_event(int bVk, int bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;

        /// <summary>
        /// Sends shortcut keys (key down and up) signals.
        /// </summary>
        /// <param name="kCode">The array of keys to send as a shortcut.</param>
        /// <param name="delay">The delay in milliseconds between the key down and up events.</param>
        public static void ShortcutKeys(Keys[] kCode, int delay = 0)
        {
            KeyPressStruct KeyPress = new KeyPressStruct(kCode, delay);
            Thread t = new Thread(() => KeyPressThread(KeyPress));
            t.Start();
        }

        /// <summary>
        /// Sends a key down signal.
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        public static void KeyDown(Keys kCode)
        {
            keybd_event((int)kCode, 0, 0, 0);
        }

        /// <summary>
        /// Sends a key up signal.
        /// </summary>
        /// <param name="kCode">The virtual keycode to send.</param>
        public static void KeyUp(Keys kCode)
        {
            keybd_event((int)kCode, 0, KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// Sends a key press signal (key down and up).
        /// </summary>
        /// <param name="kCode">The virtual key code to send.</param>
        /// <param name="delay">The delay to set between the key down and up commands.</param>
        public static void KeyPress(Keys kCode, int delay = 0)
        {
            Keys[] SendKeys = new[] { kCode };
            KeyPressStruct KeyPress = new KeyPressStruct(SendKeys, delay);
            Thread t = new Thread(() => KeyPressThread(KeyPress));
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
