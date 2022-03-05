using System.Collections.Generic;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Entity
{
    public record KeyRecord
    {
        public string Name { get; set; }
        public List<KeyCode> KeyCodes { get; set; }
        public LedColor Color { get; set; }

        public KeyRecord() {}
        public KeyRecord(HidKeys hidKey, HidModifiers hidModifiers = HidModifiers.MOD_NONE)
        {
            Name = hidKey.ToString();
            KeyCodes = new() {new KeyCode(hidKey, hidModifiers)};
            Color = new LedColor(0x82, 0x00, 0xAC);
        }
    }
}