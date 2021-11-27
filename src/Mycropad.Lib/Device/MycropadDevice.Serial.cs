using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using Mycropad.Lib.Device.Messages;

namespace Mycropad.Lib.Device
{
    public class MycropadDevice : IDisposable, IMycropadDevice
    {
        private const int USB_VID = 0xcafe;
        private const int USB_PID = 0x4005;
        private MycropadDevice() { }
        private static MycropadDevice _instance;
        public static MycropadDevice Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MycropadDevice();
                }

                return _instance;
            }
        }

        private SerialPort _device;

        public bool Connected { get => _device?.IsOpen ?? false; }
        public Action OnDeviceConnected { get; set; }
        public Action OnDeviceDisconnected { get; set; }

        public void Start()
        {
            OpenDevice();
        }

        private void OpenDevice()
        {
            _device = new SerialPort
            {
                PortName = FindSerialPort(),
                WriteTimeout = 500,
                ReadTimeout = 500,
            };

            _device.Open();
            _device.DiscardInBuffer();
            _device.DiscardOutBuffer();
        }

        public bool NewKeymap(Keymap keymap)
        {
            var msgData = keymap.Serialize();
            var data = new byte[3 + msgData.Length];
            data[0] = 0x02;   // STX
            data[1] = (byte)CommandTypes.NewKeymap;
            data[^1] = 0x03;  // ETX
            Array.Copy(msgData, 0, data, 2, msgData.Length);

            // write
            _device.Write(data, 0, data.Length);

            var responseData = new byte[1024];
            _device.Read(responseData, 0, responseData.Length);
            _device.DiscardInBuffer();
            var ok = responseData[0] != 0;
            return ok;
        }

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
                        _device = null;
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
    }
}