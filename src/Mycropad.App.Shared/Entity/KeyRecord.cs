using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Shared.Entity;

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

    public string Name { get; set; } = string.Empty;
    public List<KeyCode> KeyCodes { get; set; } = new();
    public LedColor Color { get; set; } = new(0, 0, 0);
}