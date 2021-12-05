using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable CA2101

namespace Mycropad.Lib.Window.Linux
{
    /// X11 Native bindings 
    /// some code taken from https://github.com/ajnewlands/X11.Net/
    internal static class X11
    {
        #region [ types ]
        internal enum Window : UInt32 { }
        internal enum Status : Int32 { }
        internal enum Atom : UInt64 { }
        internal enum RevertFocus : Int32 { }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XTextProperty
        {
            public string value;
            public Atom encoding;
            public int format;
            public ulong nitems;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct XClassHint
        {
            public IntPtr res_name;
            public IntPtr res_class;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XErrorEvent
        {
            public int type;
            public IntPtr display;
            public ulong resourceid;
            public ulong serial;
            public byte error_code;
            public byte request_code;
            public byte minor_code;
        }

        #endregion

        #region [ methods ]

        [DllImport("libX11.so.6")]
        internal static extern void XFree(IntPtr data);

        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        internal static extern IntPtr XOpenDisplay(string display);

        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        internal static extern IntPtr XDisplayName(string display);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int XErrorHandlerDelegate(IntPtr display, ref XErrorEvent ev);

        [DllImport("libX11.so.6")]
        internal static extern IntPtr XSetErrorHandler(XErrorHandlerDelegate del);

        [DllImport("libX11.so.6")]
        internal static extern Status XGetErrorText(IntPtr display, int code, IntPtr description, int length);

        [DllImport("libX11.so.6")]
        internal static extern Status XGetInputFocus(IntPtr display, ref Window focus_return, ref RevertFocus revert_to_return);

        [DllImport("libX11.so.6")]
        internal static extern Status XGetWMName(IntPtr display, Window window, ref XTextProperty textproperty_return);

        [DllImport("libX11.so.6")]
        internal static extern IntPtr XAllocClassHint();

        [DllImport("libX11.so.6")]
        internal static extern Status XGetClassHint(IntPtr display, Window window, IntPtr classHint);

        [DllImport("libXmu.so.6")]
        internal static extern Window XmuClientWindow(IntPtr display, Window window);

        [DllImport("libX11.so.6", EntryPoint = "XQueryTree")]
        private static extern int _XQueryTree(IntPtr display, Window window, ref Window WinRootReturn, ref Window WinParentReturn, ref IntPtr ChildrenListReturn, ref uint nChildrenListReturn);

        internal static int XQueryTree(IntPtr display, Window window, ref Window WinRootReturn, ref Window WinParentReturn, out List<Window> ChildrenListReturn)
        {
            IntPtr pChildren = default;
            uint nChildren = 0;

            var r = _XQueryTree(display, window, ref WinRootReturn, ref WinParentReturn,
                ref pChildren, ref nChildren);

            if (pChildren == IntPtr.Zero)
            {
                ChildrenListReturn = null;
                return r;
            }

            ChildrenListReturn = new();
            for (int i = 0; i < nChildren; i++)
            {
                var ptr = new IntPtr(pChildren.ToInt64() + i * sizeof(Window));
                ChildrenListReturn.Add((Window)Marshal.ReadInt64(ptr));
            }

            XFree(pChildren);

            return r;
        }

        #endregion
    }
}

#pragma warning restore CA2101