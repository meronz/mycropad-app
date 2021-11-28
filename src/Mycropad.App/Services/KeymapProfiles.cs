using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mycropad.App.Entity;
using Mycropad.Lib;

namespace Mycropad.App.Services
{
    public class ProfileManager : IEnumerable<KeymapProfile>
    {
        private readonly ILogger<ProfileManager> _logger;

        private readonly List<KeymapProfile> _profiles = new();

        public ProfileManager(ILogger<ProfileManager> logger)
        {
            _logger = logger;
            Load();
        }


        public void Load()
        {
            // TODO:
            var keymap = new Keymap();
            keymap.KeyCodes[0].Add(new(HidKeys.KEY_0));
            keymap.KeyCodes[1].Add(new(HidKeys.KEY_1));
            keymap.KeyCodes[2].Add(new(HidKeys.KEY_2));
            keymap.KeyCodes[3].Add(new(HidKeys.KEY_3));
            keymap.KeyCodes[4].Add(new(HidKeys.KEY_4));
            keymap.KeyCodes[5].Add(new(HidKeys.KEY_5));
            keymap.KeyCodes[6].Add(new(HidKeys.KEY_6));
            keymap.KeyCodes[7].Add(new(HidKeys.KEY_7));
            keymap.KeyCodes[8].Add(new(HidKeys.KEY_8));
            keymap.KeyCodes[9].Add(new(HidKeys.KEY_9));
            keymap.KeyCodes[10].Add(new(HidKeys.KEY_A));

            _profiles.Clear();
            _profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = "Default",
                IsDefault = true,
                Keymap = keymap,
            });

            keymap = new Keymap();
            keymap.KeyCodes[0].Add(new(HidKeys.KEY_F1));
            keymap.KeyCodes[1].Add(new(HidKeys.KEY_F2));
            keymap.KeyCodes[2].Add(new(HidKeys.KEY_F3));
            keymap.KeyCodes[3].Add(new(HidKeys.KEY_F4));
            keymap.KeyCodes[4].Add(new(HidKeys.KEY_F5));
            keymap.KeyCodes[5].Add(new(HidKeys.KEY_F6));
            keymap.KeyCodes[6].Add(new(HidKeys.KEY_F7));
            keymap.KeyCodes[7].Add(new(HidKeys.KEY_F8));
            keymap.KeyCodes[8].Add(new(HidKeys.KEY_F9));
            keymap.KeyCodes[9].Add(new(HidKeys.KEY_F10));
            keymap.KeyCodes[10].Add(new(HidKeys.KEY_F11));

            _profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = "VisualStudio Code",
                Keymap = keymap,
            });

            keymap = new Keymap();
            keymap.KeyCodes[0].Add(new(HidKeys.KEY_A));
            keymap.KeyCodes[1].Add(new(HidKeys.KEY_B));
            keymap.KeyCodes[2].Add(new(HidKeys.KEY_C));
            keymap.KeyCodes[3].Add(new(HidKeys.KEY_D));
            keymap.KeyCodes[4].Add(new(HidKeys.KEY_E));
            keymap.KeyCodes[5].Add(new(HidKeys.KEY_F));
            keymap.KeyCodes[6].Add(new(HidKeys.KEY_G));
            keymap.KeyCodes[7].Add(new(HidKeys.KEY_H));
            keymap.KeyCodes[8].Add(new(HidKeys.KEY_8));
            keymap.KeyCodes[9].Add(new(HidKeys.KEY_9));
            keymap.KeyCodes[10].Add(new(HidKeys.KEY_A));

            _profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = "Kicad",
                Keymap = keymap,
            });
        }

        public void Save()
        {
            // todo:
        }

        public KeymapProfile GetDefault()
        {
            return _profiles.First(x => x.IsDefault);
        }

        public KeymapProfile GetProfile(Guid profileId)
        {
            return _profiles.First(x => x.Id == profileId);
        }

        public void DeleteProfile(Guid profileId)
        {
            var profile = GetProfile(profileId);
            _profiles.Remove(profile);

            Save();
        }

        public IEnumerator<KeymapProfile> GetEnumerator()
        {
            return _profiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_profiles).GetEnumerator();
        }
    }
}