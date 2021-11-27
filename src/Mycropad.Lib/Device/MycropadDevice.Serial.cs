using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
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

        public bool NewKeymap(Keymap keymap)
        {
            var data = Command(CommandTypes.NewKeymap, keymap.Serialize());
            Write(data);
            return Read();
        }

        public bool Keepalive()
        {
            var data = Command(CommandTypes.Heartbeat);
            Write(data);
            return Read();
        }

        private bool Read()
        {
            try
            {
                var responseData = new byte[1024];
                _device.Read(responseData, 0, responseData.Length);
                var ok = responseData[0] != 0;
                return ok;
            }
            catch (Exception)
            {
                _device.Close();
                OnDeviceDisconnected?.Invoke();
            }
            return false;
        }

        private void Write(byte[] data)
        {
            try
            {
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