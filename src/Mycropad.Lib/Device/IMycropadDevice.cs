
using System;

namespace Mycropad.Lib.Device
{
    public interface IMycropadDevice
    {
        bool Connected { get; }
        Action OnDeviceConnected { get; set; }
        Action OnDeviceDisconnected { get; set; }

        void Dispose();
        bool NewKeymap(Keymap keymap);
        void Start();
    }
}