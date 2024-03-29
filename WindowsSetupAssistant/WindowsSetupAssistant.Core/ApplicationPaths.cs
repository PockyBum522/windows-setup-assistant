﻿using System;
using System.IO;

namespace WindowsSetupAssistant.Core
{
    /// <summary>
    /// Contains the few paths for this application that must be hardcoded
    /// </summary>
    public static class ApplicationPaths
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
            Path.GetFullPath(
                Path.Join(
                    ThisApplicationRunFromDirectoryPath, "../../../../.."));
        
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

        /// <summary>
        /// Where to put the JSON file representing what state the setup is in, state is based on user selection in
        /// MainWindow
        /// </summary>
        public static string StatePath => @"C:\Users\Public\Documents\CSharpInstallerScriptState.json";

        /// <summary>
        /// Contains paths specific to resources such as JSON files, configuration files
        /// </summary>
        public static class ResourcePaths
        {
            /// <summary>
            /// Path to the file containing all information about available installers to show in main window on load
            /// </summary>
            public static string InstallsFileJsonPath =>
                Path.GetFullPath(
                    Path.Join(
                        SetupAssistantRootDir,
                        "Resources",
                        "Configuration",
                        "Available Installs.json"));
            
            /// <summary>
            /// Path to the file containing all information about available installers to show in main window on load
            /// </summary>
            public static string ResourceDirectoryRegistryFiles =>
                Path.GetFullPath(
                    Path.Join(
                        SetupAssistantRootDir,
                        "Resources",
                        "Configuration",
                        "Registry Files"));
            
            /// <summary>
            /// Path to the file containing all information about available powershell scripts to show in main window on load
            /// </summary>
            public static string ResourceDirectoryPowershellScripts =>
                Path.GetFullPath(
                    Path.Join(
                        SetupAssistantRootDir,
                        "Resources",
                        "Configuration",
                        "Powershell Scripts"));
        }
    }
}
