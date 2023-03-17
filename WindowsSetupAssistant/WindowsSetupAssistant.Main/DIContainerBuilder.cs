using System;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Logic.MainProcessExecutors;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders.SettingsSectionBuilders;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.UI.WindowResources.InstallsEditor;
using WindowsSetupAssistant.UI.WindowResources.MainWindow;

namespace WindowsSetupAssistant.Main;

/// <summary>
/// Builds the container for the local application dependencies. This is then passed to TeakTools.Common
/// dependency injection for library dependencies to get added
/// </summary>
[PublicAPI]
public class DiContainerBuilder
{
    /// <summary>
    /// Constructor so the null check on _mainWindowPersistentState is happy
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public DiContainerBuilder()
    {
        // This has to be done here because DeserializeObject creates another object in memory if there's already
        // a new() object in _mainWindowPersistentState that was causing problems, so we'll just do it 
        // in the constructor.
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        if (File.Exists(ApplicationPaths.StatePath))
        {
            var jsonStateRaw = File.ReadAllText(ApplicationPaths.StatePath);

            _sessionPersistentState = JsonConvert.DeserializeObject<SessionPersistentState>(jsonStateRaw, settings) ?? new SessionPersistentState();
        }
        else
        {
            _sessionPersistentState = new();
        }
    }
    
    private readonly ContainerBuilder _builder = new ();
    private ILogger? _logger;
    private SessionPersistentState _sessionPersistentState;
    
    //private ISettingsApplicationLocal _settingsApplicationLocal;

    /// <summary>
    /// Gets a built container with all local application and TeakTools.Common dependencies in it
    /// </summary>
    /// <returns>A built container with all local application and TeakTools.Common dependencies in it</returns>
    [SupportedOSPlatform("Windows7.0")]
    public IContainer GetBuiltContainer(ILogger? testingLogger = null)
    {
        // Takes testing logger or if that's null builds the normal one and adds to DI container
        RegisterLogger(testingLogger);

        // This is not injected, kind of. It adds the dictionary to Application.Current.Resources.MergedDictionaries
        AddThemeResourceMergedDictionary();
        
        // All of these methods set up and initialize all necessary resources and dependencies,
        // then register the thing for Dependency Injection

        DeserializeStateFromDiskIntoPersistentState();
        
        RegisterApplicationConfiguration();
        
        RegisterMainDependencies();

        RegisterTaskHelpers();

        RegisterMainWindowLoaders();

        RegisterSectionBuilders();
        
        RegisterUiDependencies();

        var container = _builder.Build();
        
        return container;
    }

    [SupportedOSPlatform("Windows7.0")]
    private void DeserializeStateFromDiskIntoPersistentState()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        if (File.Exists(ApplicationPaths.StatePath))
        {
            // Otherwise:
            var jsonStateRaw = File.ReadAllText(ApplicationPaths.StatePath);

            _sessionPersistentState = JsonConvert.DeserializeObject<SessionPersistentState>(jsonStateRaw, settings) ?? throw new NullReferenceException();
        }
        else
        {
            _sessionPersistentState = new SessionPersistentState();
        }
    }

    [SupportedOSPlatform("Windows7.0")]
    private void AddThemeResourceMergedDictionary()
    {
        var resourcePath = ApplicationPaths.DarkThemePath;
        var currentResource = new Uri(resourcePath, UriKind.RelativeOrAbsolute);
        
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = currentResource });
    }

    [SupportedOSPlatform("Windows7.0")]
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

    [SupportedOSPlatform("Windows7.0")]
    private void RegisterMainDependencies()
    {
        _builder.RegisterInstance(_sessionPersistentState).As<SessionPersistentState>().SingleInstance();
        
        if (_logger is not null)
            _logger.Debug("Deserialized JSON: {ToString}", _sessionPersistentState.ToString());
        
        _builder.RegisterType<ExceptionHandler>().AsSelf().SingleInstance();
        _builder.RegisterType<StartupScriptWriter>().AsSelf().SingleInstance();
        _builder.RegisterType<SystemRebooter>().AsSelf().SingleInstance();
        _builder.RegisterType<StateHandler>().AsSelf().SingleInstance();
        _builder.RegisterType<FinalCleanupHelper>().AsSelf().SingleInstance();
        
        _builder.RegisterType<SelectedSettingsExecutor>().AsSelf().SingleInstance();
        _builder.RegisterType<MainSetupProcessExecutor>().AsSelf().SingleInstance();
    }
    
    [SupportedOSPlatform("Windows7.0")]
    private void RegisterTaskHelpers()
    {
        _builder.RegisterType<DesktopHelper>().AsSelf();
        _builder.RegisterType<PowerHelper>().AsSelf();
        _builder.RegisterType<TaskbarHelper>().AsSelf();
        _builder.RegisterType<TimeHelper>().AsSelf();
        _builder.RegisterType<WindowHelper>().AsSelf();
        _builder.RegisterType<WindowsHostnameHelper>().AsSelf();
        _builder.RegisterType<WindowsUpdater>().AsSelf();
        _builder.RegisterType<DisplayHelper>().AsSelf();
        _builder.RegisterType<WindowsStoreApplicationsUninstaller>().AsSelf();
        
        _builder.RegisterType<ApplicationInstallExecutor>().AsSelf();
    }

    [SupportedOSPlatform("Windows7.0")]
    private void RegisterMainWindowLoaders()
    {
        _builder.RegisterType<RegistryFileAsOptionLoader>().AsSelf();
        _builder.RegisterType<AvailableApplicationsJsonLoader>().AsSelf();
        _builder.RegisterType<ProfileHandler>().AsSelf();
        _builder.RegisterType<StateHandler>().AsSelf();
        
        _builder.RegisterType<SettingsSectionsController>().AsSelf();
    }

    [SupportedOSPlatform("Windows7.0")]
    private void RegisterSectionBuilders()
    {
        _builder.RegisterType<TimeSettingsSectionBuilder>().AsSelf();
        _builder.RegisterType<TaskbarSettingsSectionBuilder>().AsSelf();
        _builder.RegisterType<DesktopSettingsSectionBuilder>().AsSelf();
        _builder.RegisterType<WindowSettingsSectionBuilder>().AsSelf();
        _builder.RegisterType<ApplicationsSettingsSectionBuilder>().AsSelf();
    }
    
    [SupportedOSPlatform("Windows7.0")]
    private void RegisterUiDependencies()
    {
        _builder.RegisterInstance(Dispatcher.CurrentDispatcher).AsSelf().SingleInstance();
        
        _builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
        _builder.RegisterType<MainWindow>().AsSelf().SingleInstance();
        
        _builder.RegisterType<InstallsEditorWindowViewModel>().AsSelf().SingleInstance();
        _builder.RegisterType<InstallsEditorWindow>().AsSelf().SingleInstance();
    }
}