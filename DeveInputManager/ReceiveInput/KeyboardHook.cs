using DeveInputManager.Events;
using DeveInputManager.SendInput;
using DeveInputManager.Structs;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DeveInputManager.ReceiveInput
{
    public class KeyboardHook
    {
        //[DllImport("user32.dll")]
        //private static extern int SetWindowsHookEx(int idHook, KeyboardProcDelegate pInputs, IntPtr hmod, int dwThreadId);

        //[DllImport("user32.dll")]
        //private static extern int CallNextHookEx(int hHook, int nCode, int wParam, KBDLLHOOKSTRUCT lParam);

        //[DllImport("user32.dll")]
        //private static extern int UnhookWindowsHookEx(int hHook);

        //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);





        private IntPtr KeyHook { get; set; }
        private LowLevelKeyboardProc KeyHookDelegate { get; set; }


        private const int HC_ACTION = 0;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const int WM_KEYLAST = 0x108;



        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam);

        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyEventArgs> KeyDown;

        private readonly ConcurrentDictionary<Keys, bool> KeyState = new ConcurrentDictionary<Keys, bool>();

        public void InstallHook()
        {
            KeyHookDelegate = KeyboardProc;
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    KeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyHookDelegate, GetModuleHandle(curModule.ModuleName), 0);
                }
            }

            //var aaaa = System.Reflection.Assembly.GetExecutingAssembly().GetModules();
            //KeyHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyHookDelegate, Marshal.GetHINSTANCE(aaaa[0]).ToInt32(), 0);
        }

        public void UninstallHook()
        {
            if (KeyHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(KeyHook);
                KeyHook = IntPtr.Zero;
            }
        }

        public IntPtr KeyboardProc(int nCode, IntPtr wParam, KBDLLHOOKSTRUCT lParam)
        {
            try
            {
                //'If it's a keyboard state event...
                if (nCode == HC_ACTION)
                {
                    switch ((int)wParam)
                    {
                        case WM_KEYDOWN:
                        case WM_SYSKEYDOWN:
                            Console.WriteLine($"KeyDown: {(Keys)lParam.vkCode}");
                            KeyState[(Keys)lParam.vkCode] = true;
                            if (KeyDown != null)
                            {
                                KeyDown(this, new KeyEventArgs(lParam.vkCode));
                            }
                            break;
                        case WM_KEYUP:
                        case WM_SYSKEYUP:
                            Console.WriteLine($"KeyUp:   {(Keys)lParam.vkCode}");
                            KeyState[(Keys)lParam.vkCode] = false;
                            if (KeyUp != null)
                            {
                                KeyUp(this, new KeyEventArgs(lParam.vkCode));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            return CallNextHookEx(KeyHook, nCode, wParam, lParam);
        }

        public bool GetKey(Keys key)
        {
            if (KeyState.ContainsKey(key))
            {
                return KeyState[key];
            }
            return false;
        }

        ~KeyboardHook()
        {
            UninstallHook();
        }
    }
}
