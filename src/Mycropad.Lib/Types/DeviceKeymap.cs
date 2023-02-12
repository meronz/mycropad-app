using System;
using System.Collections.Generic;
using System.Linq;
using Mycropad.Lib.Enums;

namespace Mycropad.Lib.Types;

public class DeviceKeymap
{
    private const int MAX_KEYCODES_NUM = 10;
    private const int SIZE_OF_KEYCODE = 2;

    public DeviceKeymap()
    {
        KeyCodes = new();
        foreach (var key in Enum.GetValues<Keys>()) KeyCodes[key] = new(0);
    }

    public Dictionary<Keys, List<KeyCode>> KeyCodes { get; }

    public byte[] ToBytes()
    {
        var size = KeyCodes.Values.Sum(x => 1 + x.Count) * SIZE_OF_KEYCODE;
        var buf = new byte[size];
        var bufOffset = 0;

        foreach (var c in KeyCodes.Values)
        {
            // write length
            var len = (ushort) (1 + c.Count);
            Buffer.BlockCopy(BitConverter.GetBytes(len), 0, buf, bufOffset, SIZE_OF_KEYCODE);
            bufOffset += SIZE_OF_KEYCODE;

            // write keycodes
            foreach (var keyCode in c)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(keyCode.ToUInt16()), 0, buf, bufOffset, SIZE_OF_KEYCODE);
                bufOffset += SIZE_OF_KEYCODE;
            }
        }

        return buf;
    }

    internal static DeviceKeymap FromBytes(byte[] keymapBytes)
    {
        var keymap = new DeviceKeymap();

        foreach (var key in Enum.GetValues<Keys>())
        {
            var i = (int) key;
            var bufOffset = i * (MAX_KEYCODES_NUM + 1) * SIZE_OF_KEYCODE;
            var len = BitConverter.ToUInt16(keymapBytes, bufOffset) - 1;
            bufOffset += SIZE_OF_KEYCODE;

            keymap.KeyCodes[key] = new(len);
            for (var j = 0; j < len; j++)
            {
                var keyCode = BitConverter.ToUInt16(keymapBytes, bufOffset);
                keymap.KeyCodes[key].Add(KeyCode.FromUInt16(keyCode));

                bufOffset += SIZE_OF_KEYCODE;
            }
        }

        return keymap;
    }
}