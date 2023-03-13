using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Serilog;
using WindowsSetupAssistant.Core.Logic;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.Core.Models.ViewModels;
using WindowsSetupAssistant.UI.WindowResources;

namespace WindowsSetupAssistant.Main;


/// <summary>
/// Builds the container for the local application dependencies. This is then passed to TeakTools.Common
/// dependency injection for library dependencies to get added
/// </summary>
public class DiContainerBuilder
{
    private readonly ContainerBuilder _builder = new ();
    private ILogger? _logger;
    //private ISettingsApplicationLocal _settingsApplicationLocal;

    /// <summary>
    /// Gets a built container with all local application and TeakTools.Common dependencies in it
    /// </summary>
    /// <returns>A built container with all local application and TeakTools.Common dependencies in it</returns>
    public IContainer GetBuiltContainer(ILogger? testingLogger = null)
    {
        // Takes testing logger or if that's null builds the normal one and adds to DI container
        RegisterLogger(testingLogger);

        // This is not injected, kind of. It adds the dictionary to Application.Current.Resources.MergedDictionaries
        AddThemeResourceMergedDictionary();
        
        // All of these methods set up and initialize all necessary resources and dependencies,
        // then register the thing for Dependency Injection
        
        RegisterApplicationConfiguration();
        
        RegisterMainDependencies();

        RegisterInstallerModels();
        
        RegisterUiDependencies();
        
        var container = _builder.Build();
        
        return container;
    }

    private void AddThemeResourceMergedDictionary()
    {
        var resourcePath = ApplicationPaths.DarkThemePath;
        var currentResource = new Uri(resourcePath, UriKind.RelativeOrAbsolute);
        
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = currentResource });
    }

    private void RegisterLogger(ILogger? testingLogger)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ApplicationPaths.LogPath) ?? "");

        if (testingLogger is not null)
        {
            _builder.RegisterInstance(testingLogger).As<ILogger>().SingleInstance();
            return;
        }

        // Otherwise, if it is null, make new logger:
        _logger = new LoggerConfiguration()
            .Enrich.WithProperty("WindowsSetupAssistantApplication", "WindowsSetupAssistantSerilogContext")
            .MinimumLevel.Debug()
            .WriteTo.File(ApplicationPaths.LogPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();
        
        _builder.RegisterInstance(_logger).As<ILogger>().SingleInstance();
    }
    private void RegisterApplicationConfiguration()
    {
        // _settingsApplicationLocal = 
        //     new ConfigurationBuilder<ISettingsApplicationLocal>()
        //         .UseIniFile(ApplicationPaths.PathSettingsApplicationLocalIniFile)
        //         .Build();
        //
        // _builder.RegisterInstance(_settingsApplicationLocal).As<ISettingsApplicationLocal>().SingleInstance();
        //
        // SetupAnyBlankConfigurationPropertiesAsDefaults();
    }

    private void SetupAnyBlankConfigurationPropertiesAsDefaults()
    {
        // if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.AdobeReaderExecutablePath))
        //     _settingsApplicationLocal.AdobeReaderExecutablePath = @"C:\Program Files\Adobe\Acrobat DC\Acrobat\Acrobat.exe";
        //
        // if (string.IsNullOrWhiteSpace(_settingsApplicationLocal.FoxitReaderExecutablePath))
        //     _settingsApplicationLocal.FoxitReaderExecutablePath = @"C:\Program Files (x86)\FoxitReader Basic\FoxitPDFReader.exe";
    }

    private void RegisterMainDependencies()
    {
        _builder.RegisterType<ExceptionHandler>().AsSelf().SingleInstance();

        //_builder.RegisterType<AutofacContractResolver>().AsSelf().SingleInstance();
    }
    
    private void RegisterInstallerModels()
    {
        _builder.RegisterType<ArchiveInstaller>().AsSelf();
        _builder.RegisterType<ChocolateyInstaller>().AsSelf();
        _builder.RegisterType<ExecutableInstaller>().AsSelf();
        _builder.RegisterType<PortableApplicationInstaller>().AsSelf();
    }
    
    private void RegisterUiDependencies()
    {
        _builder.RegisterInstance(Dispatcher.CurrentDispatcher).AsSelf().SingleInstance();
        
        _builder.RegisterType<MainWindowPartialViewModel>().AsSelf().SingleInstance();
        _builder.RegisterType<MainWindow>().AsSelf().SingleInstance();
    }
}