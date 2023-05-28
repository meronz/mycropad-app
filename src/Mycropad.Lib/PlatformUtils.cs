using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Claunia.PropertyList;
using Microsoft.Win32;

namespace Mycropad.Lib;

public static class PlatformUtils
{
    /// Returns the (first) virtual serial port for the specified usb device
    public static string FindSerialPort(uint vid, uint pid)
    {
#pragma warning disable CA1416
        if (IsLinux) return FindSerialPortLinux(vid, pid);

        if (IsWindows) return FindSerialPortWindows(vid, pid);

        if (IsMacOS) return FindSerialPortMacOs(vid, pid);
#pragma warning restore CA1416

        throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
    }

    [SupportedOSPlatform("linux")]
    private static string FindSerialPortLinux(uint vid, uint pid)
    {
        foreach (var devPath in Directory.EnumerateDirectories("/sys/class/tty/", "ttyACM*"))
        {
            var modAlias = File.ReadAllText(Path.Combine($"{devPath}/device/modalias"));
            var devVid = uint.Parse(modAlias[5..9], NumberStyles.HexNumber);
            var devPid = uint.Parse(modAlias[10..14], NumberStyles.HexNumber);
            if (devVid == vid && devPid == pid) return $"/dev/{devPath[15..]}";
        }

        throw new("Device not found");
    }

    [SupportedOSPlatform("windows")]
    private static string FindSerialPortWindows(uint vid, uint pid)
    {
        RegistryKey OpenSubKeyOrThrow(RegistryKey key, string name)
        {
            return key.OpenSubKey(name) ?? throw new($"Cannot open registry key {name}");
        }

        var re = new Regex($"^VID_{vid:x04}.PID_{pid:x04}", RegexOptions.IgnoreCase);
        var rk2 = OpenSubKeyOrThrow(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Enum");

        foreach (var s3 in rk2.GetSubKeyNames())
        {
            var rk3 = OpenSubKeyOrThrow(rk2, s3);
            foreach (var s in rk3.GetSubKeyNames())
            {
                if (!re.Match(s).Success) continue;

                var rk4 = OpenSubKeyOrThrow(rk3, s);
                foreach (var s2 in rk4.GetSubKeyNames())
                {
                    var rk5 = OpenSubKeyOrThrow(rk4, s2);
                    var rk6 = OpenSubKeyOrThrow(rk5, "Device Parameters");
                    var portName = rk6.GetValue("PortName")?.ToString();
                    if (!string.IsNullOrEmpty(portName) && SerialPort.GetPortNames().Contains(portName))
                        return portName;
                }
            }
        }

        throw new("Device not found");
    }

    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    private static string FindSerialPortMacOs(uint vid, uint pid)
    {
        using var proc = new Process
        {
            StartInfo = new()
            {
                FileName = "/usr/sbin/ioreg",
                Arguments = "-r -c IOUSBHostDevice -l -a",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        proc.Start();

        var ms = new MemoryStream();
        while (!proc.StandardOutput.EndOfStream)
        {
            var line = proc.StandardOutput.ReadLine();
            if(line is not null)
                ms.Write(proc.StandardOutput.CurrentEncoding.GetBytes(line));
        }
        if (!proc.WaitForExit(TimeSpan.FromSeconds(30))) throw new("Process did not terminate after 30 seconds.");
        if(proc.ExitCode != 0) throw new("Process did not terminate with 0 exit code.");

        ms.Position = 0;

        var devPath = ParseIORegPlist(ms, vid, pid);
        if (devPath is not null) return devPath;

        throw new("Device not found");
    }

    private static string? ParseIORegPlist(MemoryStream ms, uint vid, uint pid)
    {
        string? RecurseChildren(NSDictionary dict)
        {
            // We have reached the interesting key. Get the device path (IODialinDevice)
            if (dict.Compare("CFBundleIdentifier", "com.apple.driver.usb.cdc.acm") &&
                dict.Compare("idVendor", vid) &&
                dict.Compare("idProduct", pid))
            {
                foreach (var childDict in dict.TryEnumerate("IORegistryEntryChildren"))
                {
                    if (((NSDictionary)childDict).TryGetValue("IODialinDevice", out var ioDialinDevice))
                        return ((NSString)ioDialinDevice).Content;
                }
            }

            string? devPath = null;
            foreach (var child in dict.TryEnumerate("IORegistryEntryChildren"))
            {
                devPath = RecurseChildren((NSDictionary)child);
                if (devPath is not null) return devPath;
            }

            return devPath;
        }

        var rootArray = (NSArray)PropertyListParser.Parse(ms);

        string? devPath = null;
        foreach (var dict in rootArray)
        {
            devPath = RecurseChildren((NSDictionary)dict);
            if (devPath is not null) return devPath;
        }

        return devPath;
    }

    public static string GetHomeDirectory()
    {
        if (IsLinux)
        {
            var snapDataDir = Environment.GetEnvironmentVariable("SNAP_USER_COMMON");
            return !string.IsNullOrEmpty(snapDataDir)
                ? snapDataDir
                : Environment.GetEnvironmentVariable("HOME")!;
        }

        if (IsMacOS) return Environment.GetEnvironmentVariable("HOME")!;

        if (IsWindows)
            return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

        throw new NotSupportedException($"{RuntimeInformation.OSDescription} not supported");
    }


    private static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    private static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                                   RuntimeInformation.RuntimeIdentifier.StartsWith("maccatalyst");
    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}