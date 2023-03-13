using System;
using System.Windows;
using WindowsSetupAssistant.Logic;
using WindowsSetupAssistant.Logic.TaskHelpers;

namespace WindowsSetupAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Main Application class
        /// </summary>
        public App()
        {
            var logger = DependencyInitializer.GetConfiguredLogger();

            var exceptionHandler = new ExceptionHandler(logger);

            MergeDarkThemeResource();
            
            var mainWindow = 
                new WindowResources.MainWindow(logger,
                    exceptionHandler);
            
            mainWindow.ShowDialog();
        }

        private static void MergeDarkThemeResource()
        {
            var resourcePath = ApplicationPaths.DarkThemePath;
            var currentResource = new Uri(resourcePath, UriKind.RelativeOrAbsolute);
        
            Current?.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = currentResource });
        }
    }
}