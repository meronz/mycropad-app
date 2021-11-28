using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        public void SwitchProfile(Guid profileId)
        {
            if (CurrentProfileId != profileId)
            {
                var profile = GetProfile(profileId);
                _deviceManager.SwitchKeymap(profile.Keymap);
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
            var keymap = new Keymap();
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

            _profiles = new();
            _profiles.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = "Default",
                IsDefault = true,
                Keymap = keymap,
            });
        }

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
                _deviceManager.SwitchKeymap(profile.Keymap);
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