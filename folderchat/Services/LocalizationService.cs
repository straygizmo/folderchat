using System.Globalization;
using System.Resources;
using System.Reflection;

namespace folderchat.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;
        private readonly List<CultureInfo> _supportedCultures;

        public event EventHandler<CultureInfo>? CultureChanged;

        public LocalizationService()
        {
            // Initialize resource manager
            _resourceManager = new ResourceManager("folderchat.Resources.Strings", Assembly.GetExecutingAssembly());

            // Initialize supported cultures
            _supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("ja-JP")
            };

            // Load saved culture preference or use system default
            var savedCulture = Properties.Settings.Default.PreferredLanguage;
            if (!string.IsNullOrEmpty(savedCulture))
            {
                try
                {
                    _currentCulture = new CultureInfo(savedCulture);
                }
                catch
                {
                    _currentCulture = CultureInfo.CurrentUICulture;
                }
            }
            else
            {
                _currentCulture = CultureInfo.CurrentUICulture;
            }

            // Apply the culture
            ApplyCulture(_currentCulture);
        }

        public CultureInfo CurrentCulture => _currentCulture;

        public List<CultureInfo> SupportedCultures => _supportedCultures;

        public string GetString(string key)
        {
            try
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                return value ?? key; // Return key if translation not found
            }
            catch
            {
                return key; // Return key if any error occurs
            }
        }

        public void ChangeCulture(string cultureName)
        {
            try
            {
                var newCulture = new CultureInfo(cultureName);

                // Check if culture is supported
                if (!_supportedCultures.Any(c => c.Name == newCulture.Name))
                {
                    // If not supported, find the closest match
                    var closestMatch = _supportedCultures.FirstOrDefault(c =>
                        c.TwoLetterISOLanguageName == newCulture.TwoLetterISOLanguageName);

                    newCulture = closestMatch ?? _supportedCultures[0]; // Default to English
                }

                _currentCulture = newCulture;
                ApplyCulture(_currentCulture);

                // Save preference
                Properties.Settings.Default.PreferredLanguage = _currentCulture.Name;
                Properties.Settings.Default.Save();

                // Raise culture changed event
                CultureChanged?.Invoke(this, _currentCulture);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error changing culture: {ex.Message}");
            }
        }

        private void ApplyCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}