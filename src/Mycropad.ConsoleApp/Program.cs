﻿using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Mycropad.Lib.Device;
using Mycropad.Lib.Types;

namespace Mycropad.ConsoleApp
{
    public class Program
    {
        //         private static string _script =
        // @"MACRO SHIFT ALT 1
        // MACRO SHIFT ALT 2
        // MACRO SHIFT ALT 3
        // MACRO SHIFT ALT 4
        // MACRO SHIFT ALT 5
        // MACRO SHIFT ALT 6
        // MACRO SHIFT ALT 7
        // MACRO SHIFT ALT 8
        // MACRO SHIFT ALT 9
        // MACRO SHIFT ALT 0
        // DELAY 1000";

        [SupportedOSPlatform("linux")]
        public static async Task Main()
        {
            var rand = new Random();
            await Task.Delay(0);
            MycropadDeviceSerial.Instance.Start();

            while (MycropadDeviceSerial.Instance.Connected == false)
            {
                Console.Write(".");
                await Task.Delay(100);
            }

            MycropadDeviceSerial.Instance.LedsSwitchPattern(0);

            var map = new LedColor[8];
            while (Console.KeyAvailable && Console.ReadLine() == "q")
            {
                for (var i = 0; i < 8; i++)
                {
                    map[i] = new(
                        (byte)rand.Next(0, 0xFF),
                        (byte)rand.Next(0, 0xFF),
                        (byte)rand.Next(0, 0xFF)
                    );
                }

                MycropadDeviceSerial.Instance.LedsSetFixedMap(map);
                await Task.Delay(1000);
            }

        }
    }
}