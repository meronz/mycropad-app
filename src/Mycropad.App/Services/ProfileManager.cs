using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mycropad.App.Entity;
using Mycropad.Lib;
using Mycropad.Lib.Enums;

namespace Mycropad.App.Services
{
    public class ProfileManager : IEnumerable<KeyProfile>, IDisposable
    {
        private const string PROFILES_FILENAME = "profiles.json";

        private static Dictionary<Keys, KeyRecord> _defaultDeviceKeymap;
        private readonly DeviceManager _deviceManager;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
        };

        private readonly ILogger<ProfileManager> _logger;
        private List<KeyProfile> _profiles;

        public ProfileManager(ILogger<ProfileManager> logger, DeviceManager deviceManager)
        {
            _logger = logger;
            _deviceManager = deviceManager;
            _deviceManager.OnDeviceConnected += DeviceConnected;
            Load();
        }

        public Guid CurrentProfileId { get; private set; }
        public Func<Task> OnProfilesUpdated { get; set; }

        public static Dictionary<Keys, KeyRecord> DefaultKeymap =>
            _defaultDeviceKeymap ??= new()
            {
                {Keys.Key1, new(HidKeys.KEY_F1)},
                {Keys.Key2, new(HidKeys.KEY_F2)},
                {Keys.Key3, new(HidKeys.KEY_F3)},
                {Keys.Key4, new(HidKeys.KEY_F4)},
                {Keys.Key5, new(HidKeys.KEY_F5)},
                {Keys.Key6, new(HidKeys.KEY_F6)},
                {Keys.Key7, new(HidKeys.KEY_F7)},
                {Keys.Key8, new(HidKeys.KEY_F8)},
                {Keys.RotCW, new(HidKeys.KEY_Y, HidModifiers.MOD_LCTRL)},
                {Keys.RotCCW, new(HidKeys.KEY_Z, HidModifiers.MOD_LCTRL)},
                {Keys.RotClick, new(HidKeys.KEY_F11)},
            };

        public void Dispose()
        {
            _deviceManager.OnDeviceConnected += DeviceConnected;
            GC.SuppressFinalize(this);
        }

        public IEnumerator<KeyProfile> GetEnumerator()
        {
            return _profiles
                .OrderBy(x => !x.IsDefault)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _profiles
                .OrderBy(x => !x.IsDefault)
                .GetEnumerator();
        }

        private void DeviceConnected()
        {
            try
            {
                SwitchProfile(CurrentProfileId, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProfileManager.DeviceConnected");
            }
        }

        public void Load()
        {
            try
            {
                _logger.LogInformation("Load profiles");
                var dirPath = Path.Combine(PlatformUtils.GetHomeDirectory(), "mycropad");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                var path = Path.Combine(dirPath, PROFILES_FILENAME);
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
            if (CurrentProfileId == profileId && !force) return;

            var profile = GetProfile(profileId);
            _deviceManager.SwitchKeymap(profile.GetDeviceKeyMap());
            if (profile.LedsPattern == LedsPattern.Fixed) _deviceManager.LedsSetFixedMap(profile.GetDeviceLedMap());

            _deviceManager.LedsSwitchPattern(profile.LedsPattern);

            CurrentProfileId = profileId;

            if (profile.IsDefault) _deviceManager.SetDefaultKeymap(profile.GetDeviceKeyMap());

            OnProfilesUpdated?.Invoke();
        }

        private void LoadDefault()
        {
            _profiles = new()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Default",
                    IsDefault = true,
                    Keymap = DefaultKeymap,
                    LedsPattern = LedsPattern.Rainbow,
                },
            };
        }

        private void LoadFromFile(string path)
        {
            var json = File.ReadAllText(path);
            _profiles = JsonSerializer.Deserialize<List<KeyProfile>>(json, _jsonOptions);
            if (!(_profiles?.Any() ?? false)) throw new("Empty profiles");
        }

        private void Save()
        {
            _logger.LogInformation("Save profiles");
            var dirPath = Path.Combine(PlatformUtils.GetHomeDirectory(), "mycropad");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            var path = Path.Combine(dirPath, PROFILES_FILENAME);
            var json = JsonSerializer.Serialize(_profiles, _jsonOptions);
            File.WriteAllText(path, json);
        }

        private KeyProfile GetDefault()
        {
            return _profiles.First(x => x.IsDefault);
        }

        public KeyProfile GetProfile(Guid profileId)
        {
            return _profiles.First(x => x.Id == profileId);
        }

        public void AddProfile(KeyProfile profile)
        {
            _profiles.Add(profile);
            Save();
        }

        private void DeleteProfile(Guid profileId)
        {
            var profile = GetProfile(profileId);
            _profiles.Remove(profile);
            Save();
        }

        public void UpdateProfile(KeyProfile profile)
        {
            DeleteProfile(profile.Id);
            _profiles.Add(profile);
            Save();

            if (profile.Id == CurrentProfileId) SwitchProfile(profile.Id, true);
        }
    }
}