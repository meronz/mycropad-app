using System;
using Mycropad.Lib.Types;

namespace Mycropad.App.Entity
{
    public record KeymapProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Keymap Keymap { get; set; }
        public bool IsDefault { get; set; }
        public int Position { get; set; }
    }
}