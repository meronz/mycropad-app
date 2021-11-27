using System;
using Mycropad.Lib.Device.Messages;

namespace Mycropad.Lib.Device
{
    public class MycropadDeviceBase
    {
        public static byte[] Command(CommandTypes command, byte[] cmdData = null)
        {
            var data = new byte[3 + (cmdData?.Length ?? 0)];
            data[0] = 0x02;   // STX
            data[1] = (byte)command;
            data[^1] = 0x03;  // ETX

            if (cmdData != null)
            {
                Array.Copy(cmdData, 0, data, 2, cmdData.Length);
            }

            return data;
        }
    }
}