using System;

namespace DeveInputManager.Events
{
    public class KeyEventArgs : EventArgs
    {
        public int Key { get; }

        public KeyEventArgs(int key)
        {
            Key = key;
        }
    }
}
