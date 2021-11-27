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

        public static (CommandTypes cmd, bool ok, byte[] data) Response(byte[] responseData, int length)
        {
            if (responseData[0] != 0x02)
            {
                throw new Exception("Not starting with STX");
            }

            if (responseData[length - 1] != 0x03)
            {
                throw new Exception("Not ending with ETX");
            }

            var cmd = (CommandTypes)responseData[1];
            var ok = responseData[2] == 0;
            var responseSize = (int)BitConverter.ToUInt32(responseData, 3);
            var response = responseData.AsSpan(7, responseSize).ToArray();

            return (cmd, ok, response);
        }
    }
}