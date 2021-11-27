using System;
using System.Collections.Generic;
using System.Linq;

namespace Mycropad.Lib
{
    public class Keymap
    {
        private const int MaxKeyNum = 11;
        private const int SizeOfKeycode = 2;

        public readonly List<KeyCode>[] KeyCodes;

        public Keymap()
        {
            KeyCodes = new List<KeyCode>[MaxKeyNum];
            for (int i = 0; i < MaxKeyNum; i++)
            {
                KeyCodes[i] = new(0);
            }
        }

        public byte[] Serialize()
        {
            int size = KeyCodes.Sum(x => 1 + x.Count) * SizeOfKeycode;
            var buf = new byte[size];
            var bufOffset = 0;

            foreach (var c in KeyCodes)
            {
                // write length
                var len = (uint)(1 + c.Count);
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

    }
}