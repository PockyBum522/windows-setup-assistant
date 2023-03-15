using System;
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
    public partial class App
    {
        private readonly DiContainerBuilder _mainBuilder = new ();
        private ILifetimeScope _scope;
        private MainWindow _mainWindow;

        /// <summary>
        /// Overridden OnStartup, this is our composition root and has the most basic work going on to start the app
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            var dependencyContainer = _mainBuilder.GetBuiltContainer();
            
            _scope = dependencyContainer.BeginLifetimeScope();
            
            _mainWindow = _scope.Resolve<MainWindow>();
            var persistentState = _scope.Resolve<MainWindowPersistentState>();
            _mainWindow.DataContext = persistentState;

            Console.WriteLine();
            
            _mainWindow.Show();
        }
    }
}