using DeveInputManager.SendInput;

namespace DeveInputManager.Structs
{
    internal struct KeyPressStruct
    {
        internal Keys[] Keys;
        internal int Delay;

        public KeyPressStruct(Keys[] keys, int delay = 0)
        {
            Keys = keys;
            Delay = delay;
        }
    }
}
