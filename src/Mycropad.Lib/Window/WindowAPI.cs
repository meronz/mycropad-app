using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Mycropad.Lib.Window
{
    public class WindowAPI : IWindowAPI
    {
        private readonly WindowAPI_Linux_X11 _base;

        public WindowAPI(ILogger<IWindowAPI> logger)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _base = new WindowAPI_Linux_X11(logger);
            }
            else
            {
                throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
            }
        }


#pragma warning disable CA1416 // Platform check is made on object construction

        public string CurrentWindowTitle => _base.CurrentWindowTitle;
        public string CurrentWindowApplication => _base.CurrentWindowApplication;

#pragma warning restore CA1416
    }
}