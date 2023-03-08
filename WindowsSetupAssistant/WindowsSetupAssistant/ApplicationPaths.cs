using System;
using System.IO;

namespace WindowsSetupAssistant
{
    internal static class ApplicationPaths
    {
        /// <summary>
        /// Per-user log folder path
        /// </summary>
        public static string LogAppBasePath =>
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
        
        
        public static string PortableAppsDestinationDirectory => 
                @"C:\PortableApplications"; 
        
        public static string PortableAppsSourceDirectory =>
            Path.Join(
                @"D:\Dropbox", 
                "Apps", 
                "New Workstation Setup", 
                "PortableApplications");

        public static string ThisApplicationRunFromDirectoryPath
            => Path.GetDirectoryName(Environment.ProcessPath) ?? "";
        
        public static string ThisApplicationProcessPath 
            => Environment.ProcessPath ?? "";

        public static string DarkThemePath
            =>
                Path.Join(
                    ThisApplicationRunFromDirectoryPath,
                    "Themes",
                    "SelenMetroDark",
                    "Styles.xaml");
    }
}
