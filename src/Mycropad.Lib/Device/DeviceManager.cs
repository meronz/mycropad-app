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
    private readonly ILogger<DeviceManager> _logger;
    private readonly Timer _heartbeatTimer;
    private bool _autoReconnect;

    public DeviceManager(ILogger<DeviceManager> logger, IMycropadDevice device)
    {
        _logger = logger;
        _device = device;
        _heartbeatTimer = new Timer(Heartbeat, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
    }

    public Action? OnDeviceConnected { get; set; }

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

    public async Task Start(bool autoReconnect = true)
    {
        _autoReconnect = autoReconnect;
        if (!_autoReconnect)
        {
            await _device.Start();
            OnDeviceConnected?.Invoke();
        }
        _heartbeatTimer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
    }

    private async void Heartbeat(object? _)
    {
        try
        {
            if (!_device.Connected && _autoReconnect)
            {
                await _device.Start();
                OnDeviceConnected?.Invoke();
            }
            else
            {
                await _device.Heartbeat();
            }
            _heartbeatTimer.Change(TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
        }
        catch
        {
            _heartbeatTimer.Change(TimeSpan.FromMilliseconds(100), Timeout.InfiniteTimeSpan);
        }
    }

    public void Dispose()
    {
        _heartbeatTimer.Dispose();
        GC.SuppressFinalize(this);
    }
}