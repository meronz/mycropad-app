using System;
using System.Collections.Generic;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Entity
{
    public record KeymapProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Keymap Keymap { get; set; }
        public bool IsDefault { get; set; }
        public LedsPattern LedsPattern { get; set; }
        public LedColor[] LedsMap { get; set; }
        public string[] KeyNames { get; set; }
    }
}