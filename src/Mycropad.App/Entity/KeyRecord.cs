using System.Collections.Generic;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Entity
{
    public record KeyRecord
    {
        // ReSharper disable once UnusedMember.Global
        // Needed for JsonSerialization
        public KeyRecord()
        {
        }

        public KeyRecord(HidKeys hidKey, HidModifiers hidModifiers = HidModifiers.MOD_NONE)
        {
            Name = hidKey.ToString();
            KeyCodes = new() {new(hidKey, hidModifiers)};
            Color = new(0x82, 0x00, 0xAC);
        }

        public string Name { get; set; }
        public List<KeyCode> KeyCodes { get; set; }
        public LedColor Color { get; set; }
    }
}