using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace folderchat.Services
{
    // SettingsProvider that saves user settings in %APPDATA%\folderchat
    public sealed class AppDataRoamingSettingsProvider : SettingsProvider
    {
        private const string FileName = "folderchat.user.settings.json";
        private static readonly object _sync = new();
        private Dictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);
        private bool _loaded = false;
        private string? _effectivePath;

        public override string ApplicationName
        {
            get => AppDomain.CurrentDomain.FriendlyName;
            set { /* no-op */ }
        }

        public override void Initialize(string name, NameValueCollection? config)
        {
            base.Initialize(name ?? nameof(AppDataRoamingSettingsProvider), config ?? new());
        }

        private string GetSettingsFilePath()
        {
            if (_effectivePath != null) return _effectivePath;

            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "folderchat");
            Directory.CreateDirectory(dir);
            _effectivePath = Path.Combine(dir, FileName);
            return _effectivePath;
        }

        private void Load()
        {
            if (_loaded) return;
            lock (_sync)
            {
                if (_loaded) return;
                var path = GetSettingsFilePath();
                if (File.Exists(path))
                {
                    try
                    {
                        var json = File.ReadAllText(path);
                        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                        if (dict != null)
                            _cache = new Dictionary<string, string>(dict, StringComparer.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        _cache = new(StringComparer.OrdinalIgnoreCase);
                    }
                }
                _loaded = true;
            }
        }

        private void Save()
        {
            lock (_sync)
            {
                var path = GetSettingsFilePath();
                var json = JsonSerializer.Serialize(_cache, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            Load();
            var result = new SettingsPropertyValueCollection();
            foreach (SettingsProperty prop in properties)
            {
                var spv = new SettingsPropertyValue(prop);
                if (_cache.TryGetValue(prop.Name, out var raw))
                {
                    spv.PropertyValue = ConvertFromString(raw, prop.PropertyType);
                    spv.IsDirty = false;
                }
                else
                {
                    var def = prop.DefaultValue as string;
                    if (def != null)
                    {
                        spv.PropertyValue = ConvertFromString(def, prop.PropertyType);
                    }
                    else
                    {
                        spv.PropertyValue = GetDefaultForType(prop.PropertyType);
                    }
                    spv.IsDirty = false;
                }
                result.Add(spv);
            }
            return result;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            Load();
            bool changed = false;
            foreach (SettingsPropertyValue spv in values)
            {
                var str = ConvertToString(spv.PropertyValue);
                if (!_cache.TryGetValue(spv.Name, out var existing) || !string.Equals(existing, str, StringComparison.Ordinal))
                {
                    _cache[spv.Name] = str;
                    changed = true;
                }
            }
            if (changed)
            {
                try { Save(); } catch { /* Writing failure is suppressed */ }
            }
        }

        private static object? GetDefaultForType(Type t)
        {
            if (t.IsValueType) return Activator.CreateInstance(t);
            return null;
        }

        private static object? ConvertFromString(string raw, Type targetType)
        {
            try
            {
                if (targetType == typeof(string)) return raw;
                if (targetType.IsEnum) return Enum.Parse(targetType, raw, ignoreCase: true);
                if (targetType == typeof(bool)) return bool.Parse(raw);
                if (targetType == typeof(int)) return int.Parse(raw, CultureInfo.InvariantCulture);
                if (targetType == typeof(long)) return long.Parse(raw, CultureInfo.InvariantCulture);
                if (targetType == typeof(short)) return short.Parse(raw, CultureInfo.InvariantCulture);
                if (targetType == typeof(float)) return float.Parse(raw, CultureInfo.InvariantCulture);
                if (targetType == typeof(double)) return double.Parse(raw, CultureInfo.InvariantCulture);
                if (targetType == typeof(decimal)) return decimal.Parse(raw, CultureInfo.InvariantCulture);
                return Convert.ChangeType(raw, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return GetDefaultForType(targetType);
            }
        }

        private static string ConvertToString(object? value)
        {
            if (value == null) return string.Empty;
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }
    }
}

namespace folderchat.Properties
{
    [global::System.Configuration.SettingsProvider(typeof(folderchat.Services.AppDataRoamingSettingsProvider))]
    internal sealed partial class Settings { }
}