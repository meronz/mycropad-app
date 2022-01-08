using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Mycropad.Lib
{
    public static class PlatformUtils
    {
        ///
        /// Returns the (first) virtual serial port for the specified usb device
        ///
        public static string FindSerialPort(uint vid, uint pid)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return FindSerialPortLinux(vid, pid);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return FindSerialPortWindows(vid, pid);
            }

            throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
        }

        [SupportedOSPlatform("linux")]
        private static string FindSerialPortLinux(uint vid, uint pid)
        {
            foreach (var devPath in Directory.EnumerateDirectories("/sys/class/tty/", "ttyACM*"))
            {
                var modAlias = File.ReadAllText(Path.Combine($"{devPath}/device/modalias"));
                var devVid = uint.Parse(modAlias[5..9], System.Globalization.NumberStyles.HexNumber);
                var devPid = uint.Parse(modAlias[10..14], System.Globalization.NumberStyles.HexNumber);
                if (devVid == vid && devPid == pid)
                {
                    return $"/dev/{devPath[15..]}";
                }
            }
            throw new Exception("Device not found");
        }

        [SupportedOSPlatform("windows")]
        private static string FindSerialPortWindows(uint vid, uint pid)
        {
            var re = new Regex($"^VID_{vid:x04}.PID_{pid:x04}", RegexOptions.IgnoreCase);
            var rk2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (var s3 in rk2.GetSubKeyNames())
            {
                var rk3 = rk2.OpenSubKey(s3);
                foreach (var s in rk3.GetSubKeyNames())
                {
                    if (re.Match(s).Success)
                    {
                        var rk4 = rk3.OpenSubKey(s);
                        foreach (var s2 in rk4.GetSubKeyNames())
                        {
                            var rk5 = rk4.OpenSubKey(s2);
                            var rk6 = rk5.OpenSubKey("Device Parameters");
                            var portName = (string)rk6.GetValue("PortName");
                            if (!string.IsNullOrEmpty(portName) && SerialPort.GetPortNames().Contains(portName))
                                return (string)rk6.GetValue("PortName");
                        }
                    }
                }
            }
            throw new Exception("Device not found");
        }

        public static string GetHomeDirectory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var snapDataDir = Environment.GetEnvironmentVariable("SNAP_USER_COMMON");
                if (!string.IsNullOrEmpty(snapDataDir))
                {
                    return snapDataDir;
                }
                return Environment.GetEnvironmentVariable("HOME");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Environment.GetEnvironmentVariable("HOME");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }

            throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
        }
    }
}