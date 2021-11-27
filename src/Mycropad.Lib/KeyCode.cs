namespace Mycropad.Lib
{
    public record KeyCode
    {
        public byte Modifiers { get; set; }
        public byte Key { get; set; }

        public KeyCode(byte key, byte modifiers = 0)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public uint ToUInt32()
        {
            return (uint)((Modifiers << 8) | Key);
        }

        public override string ToString() => $"{Modifiers:X02} : {Key:X02}";
    }
}