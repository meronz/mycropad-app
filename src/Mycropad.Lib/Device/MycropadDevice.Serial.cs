using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Versioning;
using Mycropad.Lib.Device.Messages;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.Lib.Device
{
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("windows")]
    public sealed class MycropadDeviceSerial : MycropadDeviceBase, IMycropadDevice, IDisposable
    {
        private const uint USB_VID = 0xCAFE;
        private const uint USB_PID = 0x4005;
        private static MycropadDeviceSerial _instance;

        private readonly SerialPort _device = new();
        private readonly object _deviceMutex = new();

        private MycropadDeviceSerial()
        {
        }

        public static MycropadDeviceSerial Instance => _instance ??= new();
        public bool Connected => _device.IsOpen;
        public Action OnDeviceConnected { get; set; }
        public Action OnDeviceDisconnected { get; set; }

        public void Start()
        {
            OpenDevice();
            OnDeviceConnected?.Invoke();
        }


        public void Heartbeat()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.Heartbeat);
                Write(data);
                var (readData, readLength) = Read();

                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.Heartbeat) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        public void SetKeymap(DeviceKeymap deviceKeymap)
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.SetKeymap, deviceKeymap.ToBytes());
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.SetKeymap) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        public DeviceKeymap ReadKeymap()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.ReadKeymap);
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, keymapBytes) = Response(readData, readLength);

                if (cmd != CommandTypes.ReadKeymap) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");

                return DeviceKeymap.FromBytes(keymapBytes);
            }
        }

        public void DefaultKeymap()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.DefaultKeymap);
                Write(data);
                var (readData, readLength) = Read();

                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.DefaultKeymap) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        public void SwitchKeymap(DeviceKeymap deviceKeymap)
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.SwitchKeymap, deviceKeymap.ToBytes());
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.SwitchKeymap) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        public void LedsSwitchPattern(LedsPattern pattern)
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.LedsSwitchPattern, new[] {(byte) pattern});
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.LedsSwitchPattern) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        public void LedsSetFixedMap(IEnumerable<LedColor> map)
        {
            lock (_deviceMutex)
            {
                var mapBytes = map.Take(8).SelectMany(x => BitConverter.GetBytes(x.ToUInt32())).ToArray();
                var data = Command(CommandTypes.LedsSetFixedMap, mapBytes);
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.LedsSetFixedMap) throw new($"Bad CommandType {cmd}");
                if (!ok) throw new($"{cmd} failed!");
            }
        }

        private void OpenDevice()
        {
            _device.PortName = PlatformUtils.FindSerialPort(USB_VID, USB_PID);
            _device.WriteTimeout = 500;
            _device.ReadTimeout = 500;

            _device.Open();
            _device.DiscardInBuffer();
            _device.DiscardOutBuffer();
        }

        private (byte[] data, int length) Read()
        {
            try
            {
                var offset = 0;
                var stxReceived = false;
                var responseData = new byte[1024];
                var toReceive = 0;

                do
                {
                    var remainingLength = responseData.Length - offset;
                    var bytesRead = _device.Read(responseData, offset, remainingLength);
                    offset += bytesRead;

                    if (remainingLength == 0)
                    {
                        var newBuf = new byte[responseData.Length + 1024];
                        Buffer.BlockCopy(responseData, 0, newBuf, 0, responseData.Length);
                        responseData = newBuf;
                    }

                    if (!stxReceived)
                    {
                        if (responseData[0] == 0x02)
                        {
                            stxReceived = true;
                            toReceive = (int) BitConverter.ToUInt32(responseData, 3);
                            toReceive += 8;
                        }
                        else
                        {
                            offset = 0;
                            continue;
                        }
                    }

                    // here stx has been received.
                    // etx received
                    if (responseData[offset - 1] == 0x03 && toReceive == offset) return (responseData, offset);
                } while (true);
            }
            catch (Exception)
            {
                _device.Close();
                OnDeviceDisconnected?.Invoke();
            }

            return (null, -1);
        }

        private void Write(byte[] data)
        {
            try
            {
                _device.DiscardInBuffer();
                _device.Write(data, 0, data.Length);
            }
            catch (Exception)
            {
                _device.Close();
                OnDeviceDisconnected?.Invoke();
            }
        }

        #region IDisposable

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _device?.Close();

            _disposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
        }

        #endregion
    }
}