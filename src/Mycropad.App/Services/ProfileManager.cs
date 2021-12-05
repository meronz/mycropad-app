using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Mycropad.App.Entity;
using Mycropad.Lib.Enums;
using Mycropad.Lib.Types;

namespace Mycropad.App.Services
{
    public class ProfileManager : IEnumerable<KeymapProfile>
    {
        private readonly string FileName = "profiles.json";
        private readonly ILogger<ProfileManager> _logger;
        private readonly DeviceManager _deviceManager;
        private List<KeymapProfile> _profiles;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true
        };

        public ProfileManager(ILogger<ProfileManager> logger, DeviceManager deviceManager)
        {
            _logger = logger;
            _deviceManager = deviceManager;
            Load();
        }

        public Guid CurrentProfileId { get; private set; }
        public Action OnProfilesUpdated { get; set; }

        private static Keymap _defaultKeymap;
        public static Keymap DefaultKeymap
        {
            get
            {
                if (_defaultKeymap == null)
                {
                    _defaultKeymap = new Keymap();
                    _defaultKeymap.For(Keys.Key1).Add(new(HidKeys.KEY_F1));
                    _defaultKeymap.For(Keys.Key2).Add(new(HidKeys.KEY_F2));
                    _defaultKeymap.For(Keys.Key3).Add(new(HidKeys.KEY_F3));
                    _defaultKeymap.For(Keys.Key4).Add(new(HidKeys.KEY_F4));
                    _defaultKeymap.For(Keys.Key5).Add(new(HidKeys.KEY_F5));
                    _defaultKeymap.For(Keys.Key6).Add(new(HidKeys.KEY_F6));
                    _defaultKeymap.For(Keys.Key7).Add(new(HidKeys.KEY_F7));
                    _defaultKeymap.For(Keys.Key8).Add(new(HidKeys.KEY_F8));
                    _defaultKeymap.For(Keys.RotCW).Add(new(HidKeys.KEY_Y, HidModifiers.MOD_LCTRL));
                    _defaultKeymap.For(Keys.RotCCW).Add(new(HidKeys.KEY_Z, HidModifiers.MOD_LCTRL));
                    _defaultKeymap.For(Keys.RotClick).Add(new(HidKeys.KEY_F11));
                }
                return _defaultKeymap;
            }
        }

        public void Load()
        {
            try
            {
                _logger.LogInformation("Load profiles");
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
                LoadFromFile(path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in {nameof(ProfileManager)}.{nameof(Load)}");
                Reset();
            }

            CurrentProfileId = GetDefault().Id;
            OnProfilesUpdated?.Invoke();
        }

        public void Reset()
        {
            _logger.LogInformation("Reset to default profiles");
            LoadDefault();
            Save();

            CurrentProfileId = GetDefault().Id;
            OnProfilesUpdated?.Invoke();
        }

        public void SwitchProfile(Guid profileId, bool force = false)
        {
            if (CurrentProfileId != profileId || force)
            {
                var profile = GetProfile(profileId);
                _deviceManager.SwitchKeymap(profile.Keymap);
                if (profile.LedsPattern == LedsPattern.Fixed)
                {
                    if (profile.LedsMap == null || !profile.LedsMap.Any())
                    {
                        profile.LedsMap = DefaultLedsMap();
                    }
                    _deviceManager.LedsSetFixedMap(profile.LedsMap);
                }

                _deviceManager.LedsSwitchPattern(profile.LedsPattern);

                CurrentProfileId = profileId;

                if (profile.IsDefault)
                {
                    _deviceManager.SetDefaultKeymap(profile.Keymap);
                }

                OnProfilesUpdated?.Invoke();
            }
        }

        private void LoadDefault()
        {
            _profiles = new();
            _profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = "Default",
                IsDefault = true,
                Keymap = DefaultKeymap,
                LedsPattern = LedsPattern.Rainbow,
                LedsMap = DefaultLedsMap()
            });
        }

        private static LedColor[] DefaultLedsMap() => Enumerable.Range(0, 8)
                .Select(x => new LedColor(0x82, 0x00, 0xAC))
                .ToArray();


        private void LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            _profiles = JsonSerializer.Deserialize<List<KeymapProfile>>(json, _jsonOptions);
            if (!_profiles.Any()) { throw new Exception("Empty profiles"); }
        }

        public void Save()
        {
            _logger.LogInformation("Save profiles");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
            var json = JsonSerializer.Serialize(_profiles, _jsonOptions);
            File.WriteAllText(path, json);
        }

        public KeymapProfile GetDefault()
        {
            return _profiles.First(x => x.IsDefault);
        }

        public KeymapProfile GetProfile(Guid profileId)
        {
            return _profiles.First(x => x.Id == profileId);
        }

        public void AddProfile(KeymapProfile profile)
        {
            _profiles.Add(profile);
            Save();
        }

        public void DeleteProfile(Guid profileId)
        {
            var profile = GetProfile(profileId);
            _profiles.Remove(profile);
            Save();
        }

        public void UpdateProfile(KeymapProfile profile)
        {
            DeleteProfile(profile.Id);
            _profiles.Add(profile);
            Save();

            if (profile.Id == CurrentProfileId)
            {
                SwitchProfile(profile.Id, true);
            }
        }

        public IEnumerator<KeymapProfile> GetEnumerator() =>
            _profiles
                .OrderBy(x => !x.IsDefault)
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _profiles
                .OrderBy(x => !x.IsDefault)
                .GetEnumerator();
    }
}