using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using Mycropad.Lib.Device.Messages;

namespace Mycropad.Lib.Device
{
    public class MycropadDevice_Serial : MycropadDeviceBase, IMycropadDevice, IDisposable
    {
        private const int USB_VID = 0xCAFE;
        private const int USB_PID = 0x4005;

        private MycropadDevice_Serial() { }
        private static MycropadDevice_Serial _instance;
        public static MycropadDevice_Serial Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MycropadDevice_Serial();
                }

                return _instance;
            }
        }

        private readonly SerialPort _device = new();
        private readonly object _deviceMutex = new();
        public bool Connected { get => _device.IsOpen; }
        public Action OnDeviceConnected { get; set; }
        public Action OnDeviceDisconnected { get; set; }

        public void Start()
        {
            OpenDevice();
            OnDeviceConnected?.Invoke();
        }

        private void OpenDevice()
        {
            _device.PortName = FindSerialPort();
            _device.WriteTimeout = 500;
            _device.ReadTimeout = 500;

            _device.Open();
            _device.DiscardInBuffer();
            _device.DiscardOutBuffer();
        }


        public bool Heartbeat()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.Heartbeat);
                Write(data);
                var (readData, readLength) = Read();

                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.Heartbeat) throw new Exception($"Bad CommandType {cmd}");
                if (!ok) throw new Exception($"{cmd} failed!");

                return ok;
            }
        }

        public bool NewKeymap(Keymap keymap)
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.NewKeymap, keymap.ToBytes());
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.NewKeymap) throw new Exception($"Bad CommandType {cmd}");
                if (!ok) throw new Exception($"{cmd} failed!");

                return ok;
            }
        }

        public Keymap ReadKeymap()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.ReadKeymap);
                Write(data);

                var (readData, readLength) = Read();
                var (cmd, ok, keymapBytes) = Response(readData, readLength);

                if (cmd != CommandTypes.ReadKeymap) throw new Exception($"Bad CommandType {cmd}");
                if (!ok) throw new Exception($"{cmd} failed!");

                return Keymap.FromBytes(keymapBytes);
            }
        }

        public bool DefaultKeymap()
        {
            lock (_deviceMutex)
            {
                var data = Command(CommandTypes.DefaultKeymap);
                Write(data);
                var (readData, readLength) = Read();

                var (cmd, ok, _) = Response(readData, readLength);
                if (cmd != CommandTypes.DefaultKeymap) throw new Exception($"Bad CommandType {cmd}");
                if (!ok) throw new Exception($"{cmd} failed!");

                return ok;
            }
        }

        private (byte[] data, int length) Read()
        {
            try
            {
                var offset = 0;
                bool stxReceived = false;
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
                            toReceive = (int)BitConverter.ToUInt32(responseData, 3);
                            toReceive += 8;
                        }
                        else
                        {
                            offset = 0;
                            continue;
                        }
                    }

                    if (stxReceived)
                    {
                        // etx received
                        if (responseData[offset - 1] == 0x03 && toReceive == offset)
                        {
                            return (responseData, offset);
                        }
                    }
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

        private string FindSerialPort()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                foreach (var devPath in Directory.EnumerateDirectories("/sys/class/tty/", "ttyACM*"))
                {
                    var modAlias = File.ReadAllText(Path.Combine($"{devPath}/device/modalias"));
                    var vid = uint.Parse(modAlias[5..9], System.Globalization.NumberStyles.HexNumber);
                    var pid = uint.Parse(modAlias[10..14], System.Globalization.NumberStyles.HexNumber);
                    if (vid == USB_VID && pid == USB_PID)
                    {
                        return $"/dev/{devPath[15..]}";
                    }
                }
                throw new Exception("Device not found");
            }

            throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
        }

        #region IDisposable
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_device != null)
                    {
                        _device.Close();
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}