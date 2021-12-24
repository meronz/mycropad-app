using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mycropad.Lib.Device;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Services
{
    public class DeviceManager : IDisposable
    {
        private readonly ILogger<DeviceManager> _logger;
        private readonly IMycropadDevice _device;
        private readonly Thread _deviceThread;
        private bool _closing;
        private bool _disconnectionEventRaised;

        public Action OnDeviceConnected { get; set; }
        public Action OnDeviceDisconnected { get; set; }

        public DeviceManager(ILogger<DeviceManager> logger, IMycropadDevice device)
        {
            _logger = logger;
            _device = device;
            _deviceThread = new Thread(DeviceThread);
            _deviceThread.Start();
        }

        public void ResetKeymap()
        {
            _logger.LogInformation("ResetKeymap start");
            var ok = _device.DefaultKeymap();
            if (!ok)
            {
                // todo: error popup
                _logger.LogError("ResetKeymap failed");
            }
            else
            {
                _logger.LogInformation("Reading default keymap");
            }
            _logger.LogInformation("Resetting keymap done");
        }

        public void SetDefaultKeymap(Keymap keymap)
        {
            _logger.LogInformation("SetDefaultKeymap start");
            var ok = _device.SetKeymap(keymap);
            if (!ok)
            {
                _logger.LogError("SetDefaultKeymap failed");
            }
            _logger.LogInformation("SetDefaultKeymap done");
        }

        public void SwitchKeymap(Keymap km)
        {
            _logger.LogInformation("SwitchKeymap start");
            var ok = _device.SwitchKeymap(km);
            if (!ok)
            {
                // todo: error popup
                _logger.LogError("SwitchKeymap failed");
            }
            _logger.LogInformation("SwitchKeymap done");
        }

        public void LedsSwitchPattern(LedsPattern pattern)
        {
            _logger.LogInformation("LedsSwitchPattern start");
            var ok = _device.LedsSwitchPattern(pattern);
            if (!ok)
            {
                // todo: error popup
                _logger.LogError("LedsSwitchPattern failed");
            }
            _logger.LogInformation("LedsSwitchPattern done");
        }

        public void LedsSetFixedMap(IEnumerable<LedColor> ledsMap)
        {
            _logger.LogInformation("LedsSetFixedMap start");
            var ok = _device.LedsSetFixedMap(ledsMap);
            if (!ok)
            {
                // todo: error popup
                _logger.LogError("LedsSetFixedMap failed");
            }
            _logger.LogInformation("LedsSetFixedMap done");
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
                        OnDeviceConnected?.Invoke();
                        _disconnectionEventRaised = false;
                    }
                    else
                    {
                        _device.Heartbeat();
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception)
                {
                    if (!_disconnectionEventRaised)
                    {
                        OnDeviceDisconnected?.Invoke();
                        _disconnectionEventRaised = true;
                    }
                    Thread.Sleep(100);
                }
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