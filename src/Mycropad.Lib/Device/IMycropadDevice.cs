
using System;
using System.Collections.Generic;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.Lib.Device
{
    public interface IMycropadDevice
    {
        bool Connected { get; }
        Action OnDeviceConnected { get; set; }
        Action OnDeviceDisconnected { get; set; }

        void Dispose();
        bool Heartbeat();
        bool DefaultKeymap();
        bool SetKeymap(Keymap keymap);
        Keymap ReadKeymap();
        void Start();
        bool SwitchKeymap(Keymap keymap);
        bool LedsSwitchPattern(LedsPattern pattern);
        bool LedsSetFixedMap(IEnumerable<LedColor> map);
    }
}