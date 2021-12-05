using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Mycropad.Lib.Window.Linux;

namespace Mycropad.Lib.Window
{
    [SupportedOSPlatform("linux")]
    public class WindowAPI_Linux_X11 : IWindowAPI
    {
        private readonly ILogger<IWindowAPI> _logger;
        private readonly IntPtr _display;
        private readonly X11.XErrorHandlerDelegate OnError;

        public WindowAPI_Linux_X11(ILogger<IWindowAPI> logger)
        {
            _logger = logger;

            var pDisplayText = X11.XDisplayName(null);
            var DisplayText = Marshal.PtrToStringAnsi(pDisplayText);
            if (DisplayText == "")
            {
                throw new Exception("No display configured for X11; check the value of the DISPLAY variable is set correctly");
            }

            _logger?.LogInformation($"Connecting to X11 Display {DisplayText}");
            _display = X11.XOpenDisplay(null);
            if (_display == IntPtr.Zero)
            {
                throw new Exception("Unable to open the default X display");
            }

            OnError = ErrorHandler;
            X11.XSetErrorHandler(OnError);
        }


        private X11.Window GetFocusedWindow()
        {
            X11.Window focusWindow = default;
            X11.RevertFocus revertFocus = default;
            var status = X11.XGetInputFocus(_display, ref focusWindow, ref revertFocus);
            if (status == 0 || focusWindow == 0)
            {
                throw new Exception("Unable to get the current focus");
            }

            return focusWindow;
        }

        private X11.Window GetTopWindow(X11.Window start)
        {
            X11.Window w = start;
            X11.Window parent = start;
            X11.Window root = 0;
            int status;

            _logger?.LogInformation("getting top window");
            while (parent != root)
            {
                w = parent;
                status = X11.XQueryTree(_display, w, ref root, ref parent, out var _);

                if (status == 0)
                {
                    throw new Exception("fail");
                }

                _logger?.LogInformation($"get parent (window: {(ulong)w})");
            }

            _logger?.LogInformation($"success (window: {(ulong)w})");

            return w;
        }

        private X11.Window GetNamedWindow(X11.Window start)
        {
            X11.Window w;
            _logger?.LogInformation("Getting named window");

            w = X11.XmuClientWindow(_display, start);

            return w;
        }

        private string GetWindowTitle(X11.Window window)
        {
            X11.Status status;
            X11.XTextProperty textProperty = default;
            status = X11.XGetWMName(_display, window, ref textProperty);
            if (status == 0)
            {
                throw new Exception("XGetWMName fail");
            }

            return textProperty.value;
        }

        private string GetWindowClass(X11.Window window)
        {
            X11.Status status;
            var classPtr = X11.XAllocClassHint();
            if (classPtr == IntPtr.Zero)
            {
                throw new Exception("XAllocClassHint fail");
            }

            status = X11.XGetClassHint(_display, window, classPtr);
            if (status == 0)
            {
                X11.XFree(classPtr);
                throw new Exception("XGetClassHint fail");
            }

            string name = "";
            string @class = "";

            var classHint = Marshal.PtrToStructure<X11.XClassHint>(classPtr);
            if (classHint.res_name != IntPtr.Zero)
            {
                name = Marshal.PtrToStringAnsi(classHint.res_name);
                X11.XFree(classHint.res_name);
            }

            if (classHint.res_class != IntPtr.Zero)
            {
                @class = Marshal.PtrToStringAnsi(classHint.res_class);
                X11.XFree(classHint.res_class);
            }

            X11.XFree(classPtr);
            return $"{name}-{@class}";
        }

        private int ErrorHandler(IntPtr display, ref X11.XErrorEvent ev)
        {
            var description = Marshal.AllocHGlobal(1024);
            X11.XGetErrorText(_display, ev.error_code, description, 1024);
            var desc = Marshal.PtrToStringAnsi(description);
            _logger?.LogWarning($"X11 Error: {desc}");
            Marshal.FreeHGlobal(description);
            return 0;
        }

        public string CurrentWindowTitle
        {
            get
            {
                var w = GetFocusedWindow();
                w = GetTopWindow(w);
                w = GetNamedWindow(w);
                return GetWindowTitle(w);
            }
        }

        public string CurrentWindowApplication
        {
            get
            {
                var w = GetFocusedWindow();
                w = GetTopWindow(w);
                w = GetNamedWindow(w);
                return GetWindowClass(w);
            }
        }
    }
}