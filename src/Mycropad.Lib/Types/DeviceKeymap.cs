using System;
using System.Collections.Generic;
using System.Linq;
using Mycropad.Lib.Enums;

namespace Mycropad.Lib.Types
{
    public class DeviceKeymap
    {
        private const int MaxKeyNum = 11;
        private const int MaxKeycodesNum = 10;
        private const int SizeOfKeycode = 2;

        public List<KeyCode>[] KeyCodes { get; init; }

        public DeviceKeymap()
        {
            KeyCodes = new List<KeyCode>[MaxKeyNum];
            for (int i = 0; i < MaxKeyNum; i++)
            {
                KeyCodes[i] = new(0);
            }
        }

        public List<KeyCode> For(Keys key)
        {
            return KeyCodes[((int)key) - 1];
        }

        public byte[] ToBytes()
        {
            int size = KeyCodes.Sum(x => 1 + x.Count) * SizeOfKeycode;
            var buf = new byte[size];
            var bufOffset = 0;

            foreach (var c in KeyCodes)
            {
                // write length
                var len = (ushort)(1 + c.Count);
                Buffer.BlockCopy(BitConverter.GetBytes(len), 0, buf, bufOffset, SizeOfKeycode);
                bufOffset += SizeOfKeycode;

                // write keycodes
                foreach (var keyCode in c)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(keyCode.ToUInt16()), 0, buf, bufOffset, SizeOfKeycode);
                    bufOffset += SizeOfKeycode;
                }
            }

            return buf;
        }

        internal static DeviceKeymap FromBytes(byte[] keymapBytes)
        {
            var keymap = new DeviceKeymap();
            int bufOffset;

            for (int i = 0; i < MaxKeyNum; i++)
            {
                bufOffset = i * (MaxKeycodesNum + 1) * SizeOfKeycode;
                var len = BitConverter.ToUInt16(keymapBytes, bufOffset) - 1;
                bufOffset += SizeOfKeycode;

                keymap.KeyCodes[i] = new(len);
                for (int j = 0; j < len; j++)
                {
                    var keyCode = BitConverter.ToUInt16(keymapBytes, bufOffset);
                    keymap.KeyCodes[i].Add(KeyCode.FromUInt16(keyCode));

                    bufOffset += SizeOfKeycode;
                }
            }
            return keymap;
        }
    }
}