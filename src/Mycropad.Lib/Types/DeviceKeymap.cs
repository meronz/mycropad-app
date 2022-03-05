using System;
using System.Collections.Generic;
using System.Linq;
using Mycropad.Lib.Enums;

namespace Mycropad.Lib.Types
{
    public class DeviceKeymap
    {
        private const int MaxKeycodesNum = 10;
        private const int SizeOfKeycode = 2;

        public Dictionary<Keys, List<KeyCode>> KeyCodes { get; }

        public DeviceKeymap()
        {
            KeyCodes = new();
            foreach (var key in Enum.GetValues<Keys>())
            {
                KeyCodes[key] = new(0);
            }
        }

        public byte[] ToBytes()
        {
            var size = KeyCodes.Values.Sum(x => 1 + x.Count) * SizeOfKeycode;
            var buf = new byte[size];
            var bufOffset = 0;

            foreach (var c in KeyCodes.Values)
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

            foreach(var key in Enum.GetValues<Keys>())
            {
                var i = (int) key;
                var bufOffset = i * (MaxKeycodesNum + 1) * SizeOfKeycode;
                var len = BitConverter.ToUInt16(keymapBytes, bufOffset) - 1;
                bufOffset += SizeOfKeycode;

                keymap.KeyCodes[key] = new(len);
                for (var j = 0; j < len; j++)
                {
                    var keyCode = BitConverter.ToUInt16(keymapBytes, bufOffset);
                    keymap.KeyCodes[key].Add(KeyCode.FromUInt16(keyCode));

                    bufOffset += SizeOfKeycode;
                }
            }

            return keymap;
        }
    }
}