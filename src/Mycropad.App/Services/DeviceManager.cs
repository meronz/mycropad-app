using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Mycropad.Lib.Device;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Services;

public class DeviceManager : IDisposable
{
    private readonly IMycropadDevice _device;
    private readonly Thread _deviceThread;
    private readonly ILogger<DeviceManager> _logger;
    private bool _closing;

    public DeviceManager(ILogger<DeviceManager> logger, IMycropadDevice device)
    {
        _logger = logger;
        _device = device;
        _deviceThread = new(DeviceThread);
    }

    public Action OnDeviceConnected { get; set; }

    public void Dispose()
    {
        _closing = true;
        _deviceThread.Join();
        GC.SuppressFinalize(this);
    }

    public void ResetKeymap()
    {
        try
        {
            _logger.LogDebug("ResetKeymap start");
            _device.DefaultKeymap();
            _logger.LogDebug("Resetting keymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ResetKeymap failed");
        }
    }

    public void SetDefaultKeymap(DeviceKeymap deviceKeymap)
    {
        try
        {
            _logger.LogDebug("SetDefaultKeymap start");
            _device.SetKeymap(deviceKeymap);
            _logger.LogDebug("SetDefaultKeymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetDefaultKeymap failed");
        }
    }

    public void SwitchKeymap(DeviceKeymap km)
    {
        try
        {
            _logger.LogDebug("SwitchKeymap start");
            _device.SwitchKeymap(km);
            _logger.LogDebug("SwitchKeymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SwitchKeymap failed");
        }
    }

    public void LedsSwitchPattern(LedsPattern pattern)
    {
        try
        {
            _logger.LogDebug("LedsSwitchPattern start");
            _device.LedsSwitchPattern(pattern);
            _logger.LogDebug("LedsSwitchPattern done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LedsSwitchPattern failed");
        }
    }

    public void LedsSetFixedMap(IEnumerable<LedColor> ledsMap)
    {
        try
        {
            _logger.LogDebug("LedsSetFixedMap start");
            _device.LedsSetFixedMap(ledsMap);
            _logger.LogDebug("LedsSetFixedMap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LedsSetFixedMap failed");
        }
    }

    public void Start()
    {
        _closing = false;
        _deviceThread.Start();
    }

    private void DeviceThread(object state)
    {
        while (!_closing)
            try
            {
                if (!_device.Connected)
                {
                    _device.Start();
                    OnDeviceConnected?.Invoke();
                }
                else
                {
                    _device.Heartbeat();
                    Thread.Sleep(2000);
                }
            }
            catch (Exception)
            {
                Thread.Sleep(100);
            }
    }
}