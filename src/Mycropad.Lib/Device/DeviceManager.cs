using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.Lib.Device;

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

    public Action? OnDeviceConnected { get; set; }

    public void Dispose()
    {
        _closing = true;
        _deviceThread.Join();
        GC.SuppressFinalize(this);
    }

    public async Task ResetKeymap()
    {
        try
        {
            _logger.LogDebug("ResetKeymap start");
            await _device.DefaultKeymap();
            _logger.LogDebug("Resetting keymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ResetKeymap failed");
        }
    }

    public async Task SetDefaultKeymap(DeviceKeymap deviceKeymap)
    {
        try
        {
            _logger.LogDebug("SetDefaultKeymap start");
            await _device.SetKeymap(deviceKeymap);
            _logger.LogDebug("SetDefaultKeymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetDefaultKeymap failed");
        }
    }

    public async Task SwitchKeymap(DeviceKeymap km)
    {
        try
        {
            _logger.LogDebug("SwitchKeymap start");
            await _device.SwitchKeymap(km);
            _logger.LogDebug("SwitchKeymap done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SwitchKeymap failed");
        }
    }

    public async Task LedsSwitchPattern(LedsPattern pattern)
    {
        try
        {
            _logger.LogDebug("LedsSwitchPattern start");
            await _device.LedsSwitchPattern(pattern);
            _logger.LogDebug("LedsSwitchPattern done");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "LedsSwitchPattern failed");
        }
    }

    public async Task LedsSetFixedMap(IEnumerable<LedColor> ledsMap)
    {
        try
        {
            _logger.LogDebug("LedsSetFixedMap start");
            await _device.LedsSetFixedMap(ledsMap);
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

    public async Task StartNoThread()
    {
        _closing = false;
        await _device.Start();
        OnDeviceConnected?.Invoke();
        await _device.Heartbeat();
    }

    private async void DeviceThread(object? state)
    {
        while (!_closing)
        {
            try
            {
                if (!_device.Connected)
                {
                    await _device.Start();
                    OnDeviceConnected?.Invoke();
                }
                else
                {
                    await _device.Heartbeat();
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    _logger.LogError(ex, "DeviceTask error");
                #endif
                await Task.Delay(100);
            }
        }
    }
}