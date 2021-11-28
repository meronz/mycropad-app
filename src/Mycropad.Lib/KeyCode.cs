using System;
using System.Security.Cryptography;

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

        public KeyCode(HidKeys key, HidModifiers modifiers = 0)
        {
            Key = (byte)key;
            Modifiers = (byte)modifiers;
        }

        public static KeyCode FromUInt16(ushort bytes)
        {
            return new(
                (byte)(bytes & 0xFF),
                (byte)(bytes >> 8 & 0xFF)
            );
        }

        public ushort ToUInt16()
        {
            return (ushort)((Modifiers << 8) | Key);
        }

        public override string ToString() => $"{Modifiers:X02} : {Key:X02}";
    }
}