using System.Windows;
using Autofac;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.UI.WindowResources;
using WindowsSetupAssistant.UI.WindowResources.MainWindow;

namespace WindowsSetupAssistant.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly DiContainerBuilder _mainBuilder = new ();
        
        /// <summary>
        /// Overridden OnStartup, this is our composition root and has the most basic work going on to start the app
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var dependencyContainer = _mainBuilder.GetBuiltContainer();
            
            using var scope = dependencyContainer.BeginLifetimeScope();

            // Attach unhandled exception logging
            scope.Resolve<ExceptionHandler>().SetupExceptionHandlingEvents();
            
            scope.Resolve<MainWindow>().Show();
        }
    }
}