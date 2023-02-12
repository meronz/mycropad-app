using System;
using System.Collections.Generic;
using System.Linq;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Mycropad.Lib.Macro;

public static class MacroExtensions
{
    public enum KeyTokens : byte
    {
        ESC = HidKeys.KEY_ESC,
        TAB = HidKeys.KEY_TAB,
        SPACE = HidKeys.KEY_SPACE,
        BACK = HidKeys.KEY_BACKSPACE,
        ENTER = HidKeys.KEY_ENTER,
        UP = HidKeys.KEY_UP,
        DOWN = HidKeys.KEY_DOWN,
        LEFT = HidKeys.KEY_LEFT,
        RIGHT = HidKeys.KEY_RIGHT,
        HOME = HidKeys.KEY_HOME,
        PAGE_UP = HidKeys.KEY_PAGEUP,
        PAGE_DOWN = HidKeys.KEY_PAGEDOWN,
        END = HidKeys.KEY_END,
        DEL = HidKeys.KEY_DELETE,
        PRINTSCR = HidKeys.KEY_SYSRQ,

        // F-keys
        F1 = HidKeys.KEY_F1,
        F2 = HidKeys.KEY_F2,
        F3 = HidKeys.KEY_F3,
        F4 = HidKeys.KEY_F4,
        F5 = HidKeys.KEY_F5,
        F6 = HidKeys.KEY_F6,
        F7 = HidKeys.KEY_F7,
        F8 = HidKeys.KEY_F8,
        F9 = HidKeys.KEY_F9,
        F10 = HidKeys.KEY_F10,
        F11 = HidKeys.KEY_F11,
        F12 = HidKeys.KEY_F12,
        F13 = HidKeys.KEY_F13,
        F14 = HidKeys.KEY_F14,
        F15 = HidKeys.KEY_F15,
        F16 = HidKeys.KEY_F16,
        F17 = HidKeys.KEY_F17,
        F18 = HidKeys.KEY_F18,
        F19 = HidKeys.KEY_F19,
        F20 = HidKeys.KEY_F20,
        F21 = HidKeys.KEY_F21,
        F22 = HidKeys.KEY_F22,
        F23 = HidKeys.KEY_F23,
        F24 = HidKeys.KEY_F24,

        // Letter Keys
        A = HidKeys.KEY_A,
        B = HidKeys.KEY_B,
        C = HidKeys.KEY_C,
        D = HidKeys.KEY_D,
        E = HidKeys.KEY_E,
        F = HidKeys.KEY_F,
        G = HidKeys.KEY_G,
        H = HidKeys.KEY_H,
        I = HidKeys.KEY_I,
        J = HidKeys.KEY_J,
        K = HidKeys.KEY_K,
        L = HidKeys.KEY_L,
        M = HidKeys.KEY_M,
        N = HidKeys.KEY_N,
        O = HidKeys.KEY_O,
        P = HidKeys.KEY_P,
        Q = HidKeys.KEY_Q,
        R = HidKeys.KEY_R,
        S = HidKeys.KEY_S,
        T = HidKeys.KEY_T,
        U = HidKeys.KEY_U,
        V = HidKeys.KEY_V,
        W = HidKeys.KEY_W,
        X = HidKeys.KEY_X,
        Y = HidKeys.KEY_Y,
        Z = HidKeys.KEY_Z,

        // Numbers
        _1 = HidKeys.KEY_1,
        _2 = HidKeys.KEY_2,
        _3 = HidKeys.KEY_3,
        _4 = HidKeys.KEY_4,
        _5 = HidKeys.KEY_5,
        _6 = HidKeys.KEY_6,
        _7 = HidKeys.KEY_7,
        _8 = HidKeys.KEY_8,
        _9 = HidKeys.KEY_9,
        _0 = HidKeys.KEY_0,

        // Modifier keys
        KEY_ALT = HidKeys.KEY_LEFTALT,
        KEY_ALT_R = HidKeys.KEY_RIGHTALT,
        KEY_SHIFT = HidKeys.KEY_LEFTSHIFT,
        KEY_SHIFT_R = HidKeys.KEY_RIGHTSHIFT,
        KEY_META = HidKeys.KEY_LEFTMETA,
        KEY_META_R = HidKeys.KEY_RIGHTMETA,
        KEY_CTRL = HidKeys.KEY_LEFTCTRL,
        KEY_CTRL_R = HidKeys.KEY_RIGHTCTRL,

        // Media Keys
        MEDIA_PLAYPAUSE = HidKeys.KEY_MEDIA_PLAYPAUSE,
        MEDIA_STOPCD = HidKeys.KEY_MEDIA_STOPCD,
        MEDIA_PREVIOUSSONG = HidKeys.KEY_MEDIA_PREVIOUSSONG,
        MEDIA_NEXTSONG = HidKeys.KEY_MEDIA_NEXTSONG,
        MEDIA_EJECTCD = HidKeys.KEY_MEDIA_EJECTCD,
        MEDIA_VOLUMEUP = HidKeys.KEY_MEDIA_VOLUMEUP,
        MEDIA_VOLUMEDOWN = HidKeys.KEY_MEDIA_VOLUMEDOWN,
        MEDIA_MUTE = HidKeys.KEY_MEDIA_MUTE,
        MEDIA_WWW = HidKeys.KEY_MEDIA_WWW,
        MEDIA_BACK = HidKeys.KEY_MEDIA_BACK,
        MEDIA_FORWARD = HidKeys.KEY_MEDIA_FORWARD,
        MEDIA_STOP = HidKeys.KEY_MEDIA_STOP,
        MEDIA_FIND = HidKeys.KEY_MEDIA_FIND,
        MEDIA_SCROLLUP = HidKeys.KEY_MEDIA_SCROLLUP,
        MEDIA_SCROLLDOWN = HidKeys.KEY_MEDIA_SCROLLDOWN,
        MEDIA_EDIT = HidKeys.KEY_MEDIA_EDIT,
        MEDIA_SLEEP = HidKeys.KEY_MEDIA_SLEEP,
        MEDIA_COFFEE = HidKeys.KEY_MEDIA_COFFEE,
        MEDIA_REFRESH = HidKeys.KEY_MEDIA_REFRESH,
        MEDIA_CALC = HidKeys.KEY_MEDIA_CALC,
    }

    public enum ModifierTokens : byte
    {
        ALT = HidModifiers.MOD_LALT,
        ALT_R = HidModifiers.MOD_RALT,
        SHIFT = HidModifiers.MOD_LSHIFT,
        SHIFT_R = HidModifiers.MOD_RSHIFT,
        META = HidModifiers.MOD_LMETA,
        META_R = HidModifiers.MOD_RMETA,
        CTRL = HidModifiers.MOD_LCTRL,
        CTRL_R = HidModifiers.MOD_RCTRL,
    }

    public static KeyCode ParseKeyCode(this string macro)
    {
        KeyCode keyCode = new(HidKeys.KEY_NONE);

        macro = macro.ToUpperInvariant();
        var tokens = macro.Split('+').Select(t => t.Trim());

        foreach (var token in tokens)
        {
            if (Enum.TryParse(typeof(ModifierTokens), token, true, out var modToken))
            {
                keyCode.Modifiers |= (byte) (ModifierTokens) modToken;
                continue;
            }

            if (!Enum.TryParse(typeof(KeyTokens), token, true, out var keyToken))
                throw new($"Unrecognized token {token}");

            keyCode.Key = (byte) (KeyTokens) keyToken;
            return keyCode;
        }

        return keyCode;
    }

    public static IEnumerable<string> ToMacroStrings(this KeyCode keyCode)
    {
        var result = Enum.GetValues<ModifierTokens>()
            .Where(mod => (keyCode.Modifiers & (byte) mod) == (byte) mod)
            .Select(mod => mod.ToString())
            .ToList();

        if (keyCode.Key != 0) result.Add(((KeyTokens) keyCode.Key).ToString());

        return result;
    }

    public static KeyCode SanitizeModifiers(this KeyCode keyCode)
    {
        if (keyCode.Modifiers == 0 || keyCode.Key != 0) return keyCode;

        // Turn the modifier key into a keypress
        KeyTokens? key = (ModifierTokens) keyCode.Modifiers switch
        {
            ModifierTokens.ALT => KeyTokens.KEY_ALT,
            ModifierTokens.ALT_R => KeyTokens.KEY_ALT_R,
            ModifierTokens.SHIFT => KeyTokens.KEY_SHIFT,
            ModifierTokens.SHIFT_R => KeyTokens.KEY_SHIFT_R,
            ModifierTokens.META => KeyTokens.KEY_META,
            ModifierTokens.META_R => KeyTokens.KEY_META_R,
            ModifierTokens.CTRL => KeyTokens.KEY_CTRL,
            ModifierTokens.CTRL_R => KeyTokens.KEY_CTRL_R,
            _ => null,
        };

        keyCode.Key = (byte) (key ?? 0);
        keyCode.Modifiers = 0;
        return keyCode;
    }
}