namespace Mycropad.Lib.Types
{
    public class LedColor
    {
        public LedColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public uint ToUInt32() => (uint)
            (B << 0 |
             R << 8 |
             G << 16);
    }
}