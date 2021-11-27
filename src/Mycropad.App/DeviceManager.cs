using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mycropad.Lib;
using Mycropad.Lib.Device;

namespace Mycropad.App
{
    public class DeviceManager : IDisposable
    {
        private readonly ILogger<DeviceManager> _logger;
        private readonly IMycropadDevice _device;
        private readonly Thread _deviceThread;
        private bool _closing;

        public Keymap Keymap { get; private set; }

        public DeviceManager(ILogger<DeviceManager> logger, IMycropadDevice device)
        {
            _logger = logger;
            _device = device;
            _deviceThread = new Thread(DeviceThread);
            _deviceThread.Start();
        }

        private void DeviceThread(object state)
        {
            while (!_closing)
            {
                try
                {
                    if (!_device.Connected)
                    {
                        _device.Start();
                    }
                    else
                    {

                        if (Keymap == null)
                        {
                            Keymap = _device.ReadKeymap();
                        }

                        _device.Heartbeat();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "ConnectionCallback");
                }

                Thread.Sleep(2000);
            }
        }

        public void Dispose()
        {
            _closing = true;
            _deviceThread.Join();
            GC.SuppressFinalize(this);
        }

    }
}