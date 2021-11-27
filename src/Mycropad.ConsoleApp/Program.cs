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


            var keymap = new Keymap();
            keymap.KeyCodes[0].Add(new(Consts.KEY_S));
            keymap.KeyCodes[0].Add(new(Consts.KEY_A));
            keymap.KeyCodes[0].Add(new(Consts.KEY_L));
            keymap.KeyCodes[0].Add(new(Consts.KEY_V));
            keymap.KeyCodes[0].Add(new(Consts.KEY_A));
            keymap.KeyCodes[0].Add(new(Consts.KEY_T));
            keymap.KeyCodes[0].Add(new(Consts.KEY_O));
            keymap.KeyCodes[0].Add(new(Consts.KEY_R));
            keymap.KeyCodes[0].Add(new(Consts.KEY_E));
            keymap.KeyCodes[1].Add(new(Consts.KEY_1));
            keymap.KeyCodes[2].Add(new(Consts.KEY_2));
            keymap.KeyCodes[3].Add(new(Consts.KEY_3));
            keymap.KeyCodes[4].Add(new(Consts.KEY_4));
            keymap.KeyCodes[5].Add(new(Consts.KEY_5));
            keymap.KeyCodes[6].Add(new(Consts.KEY_6));
            keymap.KeyCodes[7].Add(new(Consts.KEY_7));
            keymap.KeyCodes[8].Add(new(Consts.KEY_8));
            keymap.KeyCodes[9].Add(new(Consts.KEY_9));
            keymap.KeyCodes[10].Add(new(Consts.KEY_A));

            MycropadDevice.Instance.Start();

            while (MycropadDevice.Instance.Connected == false)
            {
                Console.Write(".");
                await Task.Delay(100);
            }

            MycropadDevice.Instance.NewKeymap(keymap);

        }
    }
}