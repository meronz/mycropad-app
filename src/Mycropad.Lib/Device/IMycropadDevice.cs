using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

// This is a client for the device. Functions implemented here
// could be also used for debugging.
// ReSharper disable UnusedMember.Global
namespace Mycropad.Lib.Device;

public interface IMycropadDevice
{
    bool Connected { get; }
    Action? OnDeviceConnected { get; set; }
    Action? OnDeviceDisconnected { get; set; }

    Task Heartbeat();
    Task DefaultKeymap();
    Task SetKeymap(DeviceKeymap deviceKeymap);
    Task<DeviceKeymap> ReadKeymap();
    Task Start();
    Task SwitchKeymap(DeviceKeymap deviceKeymap);
    Task LedsSwitchPattern(LedsPattern pattern);
    Task LedsSetFixedMap(IEnumerable<LedColor> map);
}