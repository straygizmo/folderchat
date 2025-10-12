namespace folderchat.Forms.ProviderSettings
{
    /// <summary>
    /// Interface for provider-specific settings controls
    /// </summary>
    public interface IProviderSettingsControl
    {
        /// <summary>
        /// Load settings from application settings
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Save settings to application settings
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Validate the current settings
        /// </summary>
        /// <returns>True if settings are valid, false otherwise</returns>
        bool ValidateSettings();

        /// <summary>
        /// Get validation error message if any
        /// </summary>
        string GetValidationError();
    }
}
