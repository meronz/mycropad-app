using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Mycropad.Lib.Window;

namespace Mycropad.ConsoleApp
{
    public class Program
    {
        [SupportedOSPlatform("linux")]
        public static async Task Main()
        {
            var currentProcess = Process.GetCurrentProcess();
            await Task.Delay(0);

            var ws = new WindowAPI(null);

            var iterations = 0;

            while (true)
            {
                var a = ws.CurrentWindowApplication;
                var b = ws.CurrentWindowTitle;
                iterations++;

                if (iterations % 10000 == 0)
                {
                    Console.WriteLine($"WorkingSet64 {currentProcess.WorkingSet64}");
                }
            }
        }
    }
}