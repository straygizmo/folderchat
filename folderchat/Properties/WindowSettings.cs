using System.Configuration;

namespace folderchat.Properties
{
    // Add settings for window position and size in a partial class without editing Settings.Designer.cs
    // These settings will be saved under the exe directory or LocalAppData by the existing ExeDirectorySettingsProvider.
    internal sealed partial class Settings
    {
        // Window state (Normal / Maximized)
        [UserScopedSetting]
        [DefaultSettingValue("Normal")]
        public string MainWindow_State
        {
            get => (string)this["MainWindow_State"];
            set => this["MainWindow_State"] = value;
        }

        // 位置・サイズ（ピクセル）
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MainWindow_Left
        {
            get => (int)this["MainWindow_Left"];
            set => this["MainWindow_Left"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MainWindow_Top
        {
            get => (int)this["MainWindow_Top"];
            set => this["MainWindow_Top"] = value;
        }

        // Initial value is 0. If 0 or an extreme value at startup, it will be corrected to a safe size.
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MainWindow_Width
        {
            get => (int)this["MainWindow_Width"];
            set => this["MainWindow_Width"] = value;
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int MainWindow_Height
        {
            get => (int)this["MainWindow_Height"];
            set => this["MainWindow_Height"] = value;
        }
    }
}
