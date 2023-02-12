// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Mycropad.Lib.Enums;
// Taken from:
// https://gist.github.com/MightyPork/6da26e382a7ad91b5496ee55fdc73db2

public enum HidModifiers : byte
{
    //
    // Modifier masks - used for the first byte in the HID report.
    //
    MOD_NONE = 0x00,
    MOD_LCTRL = 0x01,
    MOD_LSHIFT = 0x02,
    MOD_LALT = 0x04,
    MOD_LMETA = 0x08,
    MOD_RCTRL = 0x10,
    MOD_RSHIFT = 0x20,
    MOD_RALT = 0x40,
    MOD_RMETA = 0x80,
}