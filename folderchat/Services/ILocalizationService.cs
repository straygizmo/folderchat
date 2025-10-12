using System.Globalization;

namespace folderchat.Services
{
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets a localized string for the given key
        /// </summary>
        string GetString(string key);

        /// <summary>
        /// Gets the current culture
        /// </summary>
        CultureInfo CurrentCulture { get; }

        /// <summary>
        /// Changes the current culture
        /// </summary>
        void ChangeCulture(string cultureName);

        /// <summary>
        /// Gets the list of supported cultures
        /// </summary>
        List<CultureInfo> SupportedCultures { get; }

        /// <summary>
        /// Event raised when the culture changes
        /// </summary>
        event EventHandler<CultureInfo>? CultureChanged;
    }
}