using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mycropad.Lib.Device;

namespace Mycropad.App
{
    public class DeviceManager : IDisposable
    {
        private readonly ILogger<DeviceManager> _logger;
        private readonly IMycropadDevice _device;
        private readonly Timer _connectionTimer;

        public DeviceManager(ILogger<DeviceManager> logger, IMycropadDevice device)
        {
            _logger = logger;
            _device = device;
            _connectionTimer = new Timer(
                ConnectionCallback,
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1));
        }

        private void ConnectionCallback(object state)
        {
            try
            {
                if (!_device.Connected)
                {
                    _device.Start();
                }
                else
                {
                    _device.Keepalive();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "ConnectionCallback");
            }
        }

        public void Dispose()
        {
            _connectionTimer.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}