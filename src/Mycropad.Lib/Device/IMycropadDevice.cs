using System;
using System.Collections.Generic;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

// This is a client for the device. Functions implemented here
// could be also used for debugging.
// ReSharper disable UnusedMember.Global
namespace Mycropad.Lib.Device
{
    public interface IMycropadDevice
    {
        bool Connected { get; }
        Action OnDeviceConnected { get; set; }
        Action OnDeviceDisconnected { get; set; }

        void Heartbeat();
        void DefaultKeymap();
        void SetKeymap(DeviceKeymap deviceKeymap);
        DeviceKeymap ReadKeymap();
        void Start();
        void SwitchKeymap(DeviceKeymap deviceKeymap);
        void LedsSwitchPattern(LedsPattern pattern);
        void LedsSetFixedMap(IEnumerable<LedColor> map);
    }
}