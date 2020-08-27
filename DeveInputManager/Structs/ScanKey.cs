namespace DeveInputManager.Structs
{
    public struct ScanKey
    {
        internal uint ScanCode;
        internal bool Extended;

        public ScanKey(uint scanCode, bool extended = false)
        {
            ScanCode = scanCode;
            Extended = extended;
        }
    }
}
