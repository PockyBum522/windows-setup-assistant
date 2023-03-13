using System.Windows;
using Autofac;
using WindowsSetupAssistant.Core.Logic;
using WindowsSetupAssistant.UI.WindowResources;

namespace WindowsSetupAssistant.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public IContainer DependencyContainer { get; private set;  } 
        
        private readonly DiContainerBuilder _mainBuilder = new ();
        
        /// <summary>
        /// Overridden OnStartup, this is our composition root and has the most basic work going on to start the app
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            DependencyContainer = _mainBuilder.GetBuiltContainer();
            
            using var scope = DependencyContainer.BeginLifetimeScope();

            // Attach unhandled exception logging
            scope.Resolve<ExceptionHandler>().SetupExceptionHandlingEvents();
            
            scope.Resolve<MainWindow>().Show();
        }
    }
}