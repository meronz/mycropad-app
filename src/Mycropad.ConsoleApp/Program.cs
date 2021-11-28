using System;
using System.Threading.Tasks;
using Mycropad.Lib;
using Mycropad.Lib.Device;

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

        public static async Task Main()
        {
            await Task.Delay(0);
            // var keymap = new List<Mapping>();
            // var sp = new ScriptParser();
            // foreach (var line in _script.Split(Environment.NewLine))
            // {
            //     keymap.Add(
            //         sp.ParseScript(
            //             line.Trim().Replace(Environment.NewLine, "")
            //         )
            //     );
            // }

            // Console.WriteLine(keymap.ToJSONString());
            // return;


            // var keymap = new Keymap();
            // keymap.KeyCodes[0].Add(new(HidKeys.KEY_0));
            // keymap.KeyCodes[1].Add(new(HidKeys.KEY_1));
            // keymap.KeyCodes[2].Add(new(HidKeys.KEY_2));
            // keymap.KeyCodes[3].Add(new(HidKeys.KEY_3));
            // keymap.KeyCodes[4].Add(new(HidKeys.KEY_4));
            // keymap.KeyCodes[5].Add(new(HidKeys.KEY_5));
            // keymap.KeyCodes[6].Add(new(HidKeys.KEY_6));
            // keymap.KeyCodes[7].Add(new(HidKeys.KEY_7));
            // keymap.KeyCodes[8].Add(new(HidKeys.KEY_8));
            // keymap.KeyCodes[9].Add(new(HidKeys.KEY_9));
            // keymap.KeyCodes[10].Add(new(HidKeys.KEY_A));

            // MycropadDevice_Serial.Instance.Start();

            // while (MycropadDevice_Serial.Instance.Connected == false)
            // {
            //     Console.Write(".");
            //     await Task.Delay(100);
            // }

            // MycropadDevice_Serial.Instance.SetKeymap(keymap);

        }
    }
}