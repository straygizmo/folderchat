using folderchat.Services;
using System.Globalization;

namespace folderchat
{
    internal static class Program
    {
        public static ILocalizationService? LocalizationService { get; private set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("FolderChat Debug Console");
            Console.WriteLine("------------------------");
            Console.WriteLine("This window displays debug information and error messages.");
            Console.WriteLine("You can disable this window in the project's debug settings if you don't need it.");
            Console.WriteLine();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Initialize localization service
            LocalizationService = new LocalizationService();

            Application.Run(new Forms.MainForm());
        }
    }
}