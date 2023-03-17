using System.Runtime.Versioning;
using System.Windows;
using Autofac;
using WindowsSetupAssistant.Core.Logic.Application;
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
        protected override async void OnStartup(StartupEventArgs e)
        {
            var dependencyContainer = _mainBuilder.GetBuiltContainer();
            
            _scope = dependencyContainer.BeginLifetimeScope();
            
            var exceptionHandler = _scope.Resolve<ExceptionHandler>(); 
            
            exceptionHandler.SetupExceptionHandlingEvents();
            
            // MainWindow and ViewModel setup
            _mainWindow = _scope.Resolve<MainWindow>();
            var mainWindowViewModel = _scope.Resolve<MainWindowViewModel>();
            _mainWindow.DataContext = mainWindowViewModel;
            
            _mainWindow.Show();

            await mainWindowViewModel.ExecuteNextSetupProcessStage();
        }
    }
}