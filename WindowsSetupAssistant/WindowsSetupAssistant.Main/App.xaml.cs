using System.Runtime.Versioning;
using System.Windows;
using Autofac;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.UI.WindowResources.MainWindow;

namespace WindowsSetupAssistant.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public partial class App
    {
        private readonly DiContainerBuilder _mainBuilder = new ();
        private ILifetimeScope? _scope;
        private MainWindow? _mainWindow;

        /// <summary>
        /// Overridden OnStartup, this is our composition root and has the most basic work going on to start the app
        /// </summary>
        /// <param name="e">Startup event args</param>
        [SupportedOSPlatform("Windows7.0")]
        protected override void OnStartup(StartupEventArgs e)
        {
            var dependencyContainer = _mainBuilder.GetBuiltContainer();
            
            _scope = dependencyContainer.BeginLifetimeScope();
            
            var exceptionHandler = _scope.Resolve<ExceptionHandler>(); 
            
            exceptionHandler.SetupExceptionHandlingEvents();
            
            _mainWindow = _scope.Resolve<MainWindow>();
            _mainWindow.DataContext = _scope.Resolve<MainWindowPersistentState>();
            
            _mainWindow.Show();
        }
    }
}