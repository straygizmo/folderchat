using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace folderchat.Services
{
    // SettingsProvider that saves user settings in the same directory as the executable (exe).
    // If writing is not possible due to permissions, it falls back to LocalAppData\folderchat.
    public sealed class ExeDirectorySettingsProvider : SettingsProvider
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
            base.Initialize(name ?? nameof(ExeDirectorySettingsProvider), config ?? new());
        }

        private static string GetPrimaryFilePath()
        {
            var baseDir = AppContext.BaseDirectory?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) ?? ".";
            return Path.Combine(baseDir, FileName);
        }

        private static string GetFallbackFilePath()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "folderchat");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, FileName);
        }

        private string ResolveWritablePath()
        {
            if (_effectivePath != null) return _effectivePath;
            var primary = GetPrimaryFilePath();
            try
            {
                var dir = Path.GetDirectoryName(primary)!;
                Directory.CreateDirectory(dir);
                var testPath = primary + ".write_test";
                File.WriteAllText(testPath, "ok");
                File.Delete(testPath);
                _effectivePath = primary;
            }
            catch
            {
                _effectivePath = GetFallbackFilePath();
            }
            return _effectivePath!;
        }

        private static string ResolveReadablePath()
        {
            var primary = GetPrimaryFilePath();
            if (File.Exists(primary)) return primary;
            var fallback = GetFallbackFilePath();
            if (File.Exists(fallback)) return fallback;
            return primary; // If not found, return primary (used when creating new)
        }

        private void Load()
        {
            if (_loaded) return;
            lock (_sync)
            {
                if (_loaded) return;
                var path = ResolveReadablePath();
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
                var path = ResolveWritablePath();
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
                try { Save(); } catch { /* Writing failure is suppressed (may improve with fallback) */ }
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
    // Use existing Properties.Settings.Default.* as is, only switch the save destination to directly under exe
    // Do not edit the auto-generated Settings.Designer.cs, add attributes to the partial class
    [global::System.Configuration.SettingsProvider(typeof(folderchat.Services.ExeDirectorySettingsProvider))]
    internal sealed partial class Settings { }
}
