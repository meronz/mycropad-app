using System;
using System.Linq;


namespace Mycropad.Lib.Macro
{
    public static class MacroExtensions
    {
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
        };

        public static KeyCode ParseKeyCode(this string macro)
        {
            KeyCode keyCode = new(HidKeys.KEY_NONE, HidModifiers.MOD_NONE);

            macro = macro.ToUpperInvariant();
            var tokens = macro.Split('+').Select(t => t.Trim());

            foreach (var token in tokens)
            {
                if (Enum.TryParse(typeof(ModifierTokens), token, true, out object modToken))
                {
                    keyCode.Modifiers |= (byte)(ModifierTokens)modToken;
                    continue;
                }

                if (Enum.TryParse(typeof(KeyTokens), token, true, out object keyToken))
                {
                    keyCode.Key = (byte)(KeyTokens)keyToken;
                    return keyCode;
                }

                throw new Exception($"Unrecognized token {token}");
            }

            return keyCode;
        }

        public static string ToMacroString(this KeyCode keyCode)
        {
            var stringResult = "";
            foreach (var mod in Enum.GetValues<ModifierTokens>())
            {
                if ((keyCode.Modifiers & (byte)mod) == (byte)mod)
                {
                    stringResult += $"{mod} + ";
                }
            }

            stringResult += ((KeyTokens)keyCode.Key).ToString();

            return stringResult;
        }
    }
}