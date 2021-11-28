
using System;
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
    }
}