using System;
using System.Collections.Generic;
using System.Linq;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Entity
{
    public record KeyProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Dictionary<Keys, KeyRecord> Keymap { get; set; }
        public bool IsDefault { get; set; }
        public LedsPattern LedsPattern { get; set; }

        public IEnumerable<LedColor> GetDeviceLedMap()
        { 
            return Keymap.Keys
                .Where(k => k < Keys.RotClick)
                .Select(k => Keymap[k].Color)
                .ToArray();
        }

        public DeviceKeymap GetDeviceKeyMap()
        {
            var deviceKeymap = new DeviceKeymap();

            foreach (var key in Enum.GetValues<Keys>())
            {
                deviceKeymap.KeyCodes[key] = Keymap[key].KeyCodes;
            }

            return deviceKeymap;
        }
    }
}