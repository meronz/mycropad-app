using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

// Json serializer uses these
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Mycropad.App.Shared.Entity;

public record KeyProfileV0
{
    public record KeyProfileV0KeyMap
    {
        public List<List<KeyCode>> KeyCodes { get; set; } = new();
    }
        
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public KeyProfileV0KeyMap Keymap { get; set; } = new();
    public bool IsDefault { get; set; }
    public LedsPattern LedsPattern { get; set; }
    public LedColor[] LedsMap { get; set; } = Array.Empty<LedColor>();
}
    
public record KeyProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<Keys, KeyRecord> Keymap { get; set; } = new();
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

        foreach (var key in Enum.GetValues<Keys>()) deviceKeymap.KeyCodes[key] = Keymap[key].KeyCodes;

        return deviceKeymap;
    }

    public KeyProfile() { }

    public KeyProfile(KeyProfileV0 v0)
    {
        Id = v0.Id;
        Name = v0.Name;
        IsDefault = v0.IsDefault;
        LedsPattern = v0.LedsPattern;
            
        Keymap = new();
        foreach (var key in Enum.GetValues<Keys>())
        {
            Keymap.Add(key, new()
            {
                KeyCodes = v0.Keymap.KeyCodes?.ElementAtOrDefault((int)key) ?? new(),
                Color = v0.LedsMap?.ElementAtOrDefault((int)key) ?? new (125, 125, 125),
                Name = key.ToString(),
            });
        }
    }
}