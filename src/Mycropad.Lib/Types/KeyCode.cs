using Mycropad.Lib.Enums;

namespace Mycropad.Lib.Types
{
    public record KeyCode
    {
        // Default constructor needed for JSON serialization
        // ReSharper disable once MemberCanBePrivate.Global
        public KeyCode()
        {
        }

        public KeyCode(HidKeys key, HidModifiers modifiers = 0)
        {
            Key = (byte) key;
            Modifiers = (byte) modifiers;
        }

        public byte Modifiers { get; set; }
        public byte Key { get; set; }

        public static KeyCode FromUInt16(ushort bytes)
        {
            return new()
            {
                Key = (byte) (bytes & 0xFF),
                Modifiers = (byte) ((bytes >> 8) & 0xFF),
            };
        }

        public ushort ToUInt16()
        {
            return (ushort) ((Modifiers << 8) | Key);
        }

        public override string ToString()
        {
            return $"{Modifiers:X02} : {Key:X02}";
        }
    }
}