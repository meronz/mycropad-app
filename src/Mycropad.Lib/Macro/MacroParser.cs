using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;


namespace Mycropad.Lib.Macro
{
    public class MacroParser
    {
        public enum ModifierTokens : byte
        {
            ALT = Consts.KEY_LEFTALT,
            ALT_R = Consts.KEY_RIGHTALT,
            SHIFT = Consts.KEY_LEFTSHIFT,
            SHIFT_R = Consts.KEY_RIGHTSHIFT,
            META = Consts.KEY_LEFTMETA,
            META_R = Consts.KEY_RIGHTMETA,
            CTRL = Consts.KEY_LEFTCTRL,
            CTRL_R = Consts.KEY_RIGHTCTRL,
        }

        public enum KeyTokens : byte
        {
            ESC = Consts.KEY_ESC,
            TAB = Consts.KEY_TAB,
            SPACE = Consts.KEY_SPACE,
            BACK = Consts.KEY_BACKSPACE,
            ENTER = Consts.KEY_ENTER,
            UP = Consts.KEY_UP,
            DOWN = Consts.KEY_DOWN,
            LEFT = Consts.KEY_LEFT,
            RIGHT = Consts.KEY_RIGHT,
            HOME = Consts.KEY_HOME,
            PAGE_UP = Consts.KEY_PAGEUP,
            PAGE_DOWN = Consts.KEY_PAGEDOWN,
            END = Consts.KEY_END,
            DEL = Consts.KEY_DELETE,
            F1 = Consts.KEY_F1,
            F2 = Consts.KEY_F2,
            F3 = Consts.KEY_F3,
            F4 = Consts.KEY_F4,
            F5 = Consts.KEY_F5,
            F6 = Consts.KEY_F6,
            F7 = Consts.KEY_F7,
            F8 = Consts.KEY_F8,
            F9 = Consts.KEY_F9,
            F10 = Consts.KEY_F10,
            F11 = Consts.KEY_F11,
            F12 = Consts.KEY_F12,
            F13 = Consts.KEY_F13,
            F14 = Consts.KEY_F14,
            F15 = Consts.KEY_F15,
            F16 = Consts.KEY_F16,
            F17 = Consts.KEY_F17,
            F18 = Consts.KEY_F18,
            F19 = Consts.KEY_F19,
            F20 = Consts.KEY_F20,
            F21 = Consts.KEY_F21,
            F22 = Consts.KEY_F22,
            F23 = Consts.KEY_F23,
            F24 = Consts.KEY_F24,
            A = Consts.KEY_A,
            B = Consts.KEY_B,
            C = Consts.KEY_C,
            D = Consts.KEY_D,
            E = Consts.KEY_E,
            F = Consts.KEY_F,
            G = Consts.KEY_G,
            H = Consts.KEY_H,
            I = Consts.KEY_I,
            J = Consts.KEY_J,
            K = Consts.KEY_K,
            L = Consts.KEY_L,
            M = Consts.KEY_M,
            N = Consts.KEY_N,
            O = Consts.KEY_O,
            P = Consts.KEY_P,
            Q = Consts.KEY_Q,
            R = Consts.KEY_R,
            S = Consts.KEY_S,
            T = Consts.KEY_T,
            U = Consts.KEY_U,
            V = Consts.KEY_V,
            W = Consts.KEY_W,
            X = Consts.KEY_X,
            Y = Consts.KEY_Y,
            Z = Consts.KEY_Z,
        };

        public static KeyCode Parse(string macro)
        {
            KeyCode keyCode = new(0, 0);

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
    }
}