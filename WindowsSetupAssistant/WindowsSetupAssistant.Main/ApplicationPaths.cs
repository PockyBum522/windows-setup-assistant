using System;
using System.IO;

namespace WindowsSetupAssistant.Main
{
    internal static class ApplicationPaths
    {
        /// <summary>
        /// Per-user log folder path
        /// </summary>
        private static string LogAppBasePath =>
            Path.Combine(
                "C:",
                "Users",
                "Public",
                "Documents",
                "Logs",
                "WorkstationSetupScript");

        /// <summary>
        /// Actual log file path passed to the ILogger configuration
        /// </summary>
        public static string LogPath =>
            Path.Combine(
                LogAppBasePath,
                "Script.log");
        
        /// <summary>
        /// The directory the assembly is running from
        /// </summary>
        public static string ThisApplicationRunFromDirectoryPath => 
            Path.GetDirectoryName(Environment.ProcessPath) ?? "";

        /// <summary>
        /// The top level dir, useful for getting to configuration folders and resource folders
        /// This is the directory the bootstrapper bat file is in
        /// </summary>
        public static string SetupAssistantRootDir => 
            Path.Join(
                ApplicationPaths.ThisApplicationRunFromDirectoryPath, "../../../..");
        
        /// <summary>
        /// The full path to this application's running assembly
        /// </summary>
        public static string ThisApplicationProcessPath => 
            Environment.ProcessPath ?? "";

        /// <summary>
        /// The full path to the dark theme Styles.xaml which contains the rest of the style information
        /// </summary>
        public static string DarkThemePath =>
                Path.Join(
                    ThisApplicationRunFromDirectoryPath,
                    "Themes",
                    "SelenMetroDark",
                    "Styles.xaml");
        
        internal static class ResourcePaths
        {
            public static string InstallsFileJsonPath = 
                Path.Join(
                    SetupAssistantRootDir,
                    "Resources",
                    "Configuration",
                    "Available Installs.json");
        }
    }
}
