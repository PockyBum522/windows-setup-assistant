using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.UI.WindowResources.MainWindow.SettingsSections;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow;

///<summary>
///Interaction logic for MainWindow.xaml
///</summary>
[ObservableObject]
#pragma warning disable MVVMTK0033
public partial class MainWindow
#pragma warning restore MVVMTK0033
{
    private readonly ILogger _logger;
    private readonly MainWindowPersistentState _mainWindowPersistentState;
    private readonly StateHandler _stateHandler;
    private readonly SystemRebooter _systemRebooter;
    private readonly StartupScriptWriter _startupScriptWriter;
    private readonly WindowsUpdater _windowsUpdater;
    private readonly PowerHelper _powerHelper;
    private readonly WindowsHostnameHelper _windowsHostnameHelper;
    private readonly RegistryFileAsOptionLoader _registryFileAsOptionLoader;
    private readonly TimeSettingsSectionBuilder _timeSettingsSectionBuilder;
    private readonly TaskbarSettingsSectionBuilder _taskbarSettingsSectionBuilder;
    private readonly DesktopSettingsSectionBuilder _desktopSettingsSectionBuilder;
    private readonly WindowSettingsSectionBuilder _windowSettingsSectionBuilder;
    private readonly FinalCleanupHelper _finalCleanupHelper;
    private readonly ProfileHandler _profileHandler;

    /// <summary>
    /// Main window constructor, loads in JSON files that need to be shown as controls, sets DataContext, sets up
    /// exception handling, checks what stage is saved to disk and works whatever needs to happen if there
    /// is one.
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="systemRebooter">Injected</param>
    /// <param name="startupScriptWriter">Creates the startup script that reruns the program on reboot</param>
    /// <param name="windowsUpdater">Windows update helper</param>
    /// <param name="powerHelper">Windows power settings helper</param>
    /// <param name="windowsHostnameHelper">Windows hostname helper</param>
    /// <param name="registryFileAsOptionLoader">Loads registry files from disk and turns them into OptionRegistryFile(s)</param>
    /// <param name="availableApplicationsJsonLoader">Loads "Available Applications.json" file into CurrentState</param>
    /// <param name="timeSettingsSectionBuilder">Time settings section builder</param>
    /// <param name="taskbarSettingsSectionBuilder">Taskbar settings section builder</param>
    /// <param name="desktopSettingsSectionBuilder">Desktop settings section builder</param>
    /// <param name="windowSettingsSectionBuilder">Window settings section builder</param>
    /// <param name="mainWindowPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    /// <param name="finalCleanupHelper">Injected to clean up all files on disk when application is finished</param>
    /// <param name="profileHandler">Injected to save/load profiles for this window</param>
    /// <param name="stateHandler">Injected to handle state loading and saving, and profile loading and saving</param>
    public MainWindow(
        ILogger logger,
        MainWindowPersistentState mainWindowPersistentState,
        FinalCleanupHelper finalCleanupHelper,
        ProfileHandler profileHandler,
        StateHandler stateHandler,
        SystemRebooter systemRebooter,
        StartupScriptWriter startupScriptWriter,
        WindowsUpdater windowsUpdater,
        PowerHelper powerHelper,
        WindowsHostnameHelper windowsHostnameHelper,
        RegistryFileAsOptionLoader registryFileAsOptionLoader,
        AvailableApplicationsJsonLoader availableApplicationsJsonLoader,
        TimeSettingsSectionBuilder timeSettingsSectionBuilder,
        TaskbarSettingsSectionBuilder taskbarSettingsSectionBuilder,
        DesktopSettingsSectionBuilder desktopSettingsSectionBuilder,
        WindowSettingsSectionBuilder windowSettingsSectionBuilder)
    {
        _logger = logger;
        _mainWindowPersistentState = mainWindowPersistentState;
        _finalCleanupHelper = finalCleanupHelper;
        _profileHandler = profileHandler;
        _stateHandler = stateHandler;
        _systemRebooter = systemRebooter;
        _startupScriptWriter = startupScriptWriter;
        _windowsUpdater = windowsUpdater;
        _powerHelper = powerHelper;
        _windowsHostnameHelper = windowsHostnameHelper;
        _registryFileAsOptionLoader = registryFileAsOptionLoader;
        _timeSettingsSectionBuilder = timeSettingsSectionBuilder;
        _taskbarSettingsSectionBuilder = taskbarSettingsSectionBuilder;
        _desktopSettingsSectionBuilder = desktopSettingsSectionBuilder;
        _windowSettingsSectionBuilder = windowSettingsSectionBuilder;
        
        availableApplicationsJsonLoader.LoadAvailableInstallersFromJsonFile();
        
        LoadAllSettingsSections();
        
        InitializeComponent();
    }

    private void ExecuteAllSelected()
    {
        _mainWindowPersistentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedOnce;
        _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);
        _startupScriptWriter.CreateRebootScriptInStartup();

        // Set long sleep and monitor off times so it doesn't sleep during install
        _powerHelper.SetPowerSettingsTo();
        
        ExecuteSelectedSettingsInAllSections();

        if (_mainWindowPersistentState.IsCheckedUpdateWindows)
        {
            _windowsUpdater.UpdateWindowsAndReboot();
        
            _systemRebooter.RebootComputerAndExit();    
        }
        else
        {
            _mainWindowPersistentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;
            _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);
        
            CheckStageAndWorkOnRerun();
        }
    }

    private void CheckStageAndWorkOnRerun()
    {
        // Set long sleep and monitor off times so it doesn't sleep during install
        _powerHelper.SetPowerSettingsTo();

        switch (_mainWindowPersistentState.ScriptStage)
        {
            case ScriptStageEnum.WindowsHasBeenUpdatedOnce:

                _windowsUpdater.UpdateWindows();

                _mainWindowPersistentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedTwice;

                _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);
                _systemRebooter.RebootComputerAndExit();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedTwice:

                _windowsUpdater.UpdateWindows();

                _mainWindowPersistentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;

                _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);
                _systemRebooter.RebootComputerAndExit();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedFully:

                if (_mainWindowPersistentState.IsCheckedUpdateWindows)
                {
                    _windowsUpdater.UpdateWindows();
                }

                WorkAllApplicationInstallCheckboxes();

                _finalCleanupHelper.DeleteSavedChoicesAndStageOnDisk();

                if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextHostname))
                {
                    _windowsHostnameHelper.ChangeHostName(_mainWindowPersistentState.TextHostname);
                }

                SetPowerSettingsToUserChoicesAtStart();

                _systemRebooter.PromptToRebootComputerAndExit();

                break;
        }
    }
    
    private void LoadAllSettingsSections()
    {
        var timeSection = _timeSettingsSectionBuilder.MakeSection();
        _mainWindowPersistentState.SettingsSections.Add(timeSection);

        var taskbarSection = _taskbarSettingsSectionBuilder.MakeSection();
        _mainWindowPersistentState.SettingsSections.Add(taskbarSection);
        
        var desktopSection = _desktopSettingsSectionBuilder.MakeSection();
        _mainWindowPersistentState.SettingsSections.Add(desktopSection);

        var windowSection = _windowSettingsSectionBuilder.MakeSection();
        _mainWindowPersistentState.SettingsSections.Add(windowSection);

        // Load registry files from disk
        var registryFilePaths = GetAllRegistryFilePathsFromResources();

        foreach (var filePath in registryFilePaths)
        {
            _registryFileAsOptionLoader.AddRegistryFileAsOption(filePath);
        }
    }

    private List<string> GetAllRegistryFilePathsFromResources()
    {
        var topLevelDirectories =
            Directory.GetDirectories(ApplicationPaths.ResourcePaths.ResourceDirectoryRegistryFiles);

        var returnPaths = new List<string>();
        
        foreach (var directoryPath in topLevelDirectories)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath, "*.reg"))
            {
                returnPaths.Add(filePath);
            }
        }

        return returnPaths;
    }

    private void StartExecution_OnClick(object sender, RoutedEventArgs e) => ExecuteAllSelected();
    private void SaveProfile_OnClick(object sender, RoutedEventArgs e) => _profileHandler.PromptUserToBrowseAndSaveProfile();

    private void LoadProfile_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllControls();
        
        _profileHandler.PromptUserForProfileThenLoadIt();
    }
    
    private void SelectAll_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (var propertyInfo in _mainWindowPersistentState.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_mainWindowPersistentState, true);
        }

        foreach (var installer in _mainWindowPersistentState.AvailableInstalls)
        {
            installer.IsSelected = true;
        }
        
        foreach (var section in _mainWindowPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                setting.IsSelected = true;
            }
        }
    }
    
    private void ClearAll_OnClick(object sender, RoutedEventArgs e) => ClearAllControls();

    private void ClearAllControls()
    {
        foreach (var propertyInfo in _mainWindowPersistentState.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_mainWindowPersistentState, false);
        }

        foreach (var installer in _mainWindowPersistentState.AvailableInstalls)
        {
            installer.IsSelected = false;
        }

        foreach (var section in _mainWindowPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                setting.IsSelected = false;
            }
        }
        
        _mainWindowPersistentState.TextHostname = "";


        _mainWindowPersistentState.IsCheckedUpdateWindows = false;
        
        _mainWindowPersistentState.TextHostname = "";

        _mainWindowPersistentState.TextMonitorTimeoutOnAc = "";
        _mainWindowPersistentState.TextMonitorTimeoutOnBattery = "";
        _mainWindowPersistentState.TextStandbyTimeoutOnAc = "";
        _mainWindowPersistentState.TextStandbyTimeoutOnBattery = "";
        _mainWindowPersistentState.TextHibernateTimeoutOnAc = "";
        _mainWindowPersistentState.TextHibernateTimeoutOnBattery = "";
    }
    
    private void ExecuteSelectedSettingsInAllSections()
    {
        foreach (var section in _mainWindowPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (!setting.IsSelected) continue;
                
                // Otherwise:
                _logger.Debug("Executing setting: {Name}", setting.DisplayName);
                    
                setting.ExecuteSetting?.Invoke();
            }
        }
    }

    private void SetPowerSettingsToUserChoicesAtStart()
    {
        var monitorTimeoutOnAc = 20; // Start with defaults
        var monitorTimeoutOnBattery = 5;
        var standbyTimeoutOnAc = 60;
        var standbyTimeoutOnBattery = 10;
        var hibernateTimeoutOnAc = 0;
        
        if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextMonitorTimeoutOnAc))
            monitorTimeoutOnAc = int.Parse(_mainWindowPersistentState.TextMonitorTimeoutOnAc);
        
        if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextMonitorTimeoutOnBattery))
            monitorTimeoutOnBattery = int.Parse(_mainWindowPersistentState.TextMonitorTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextStandbyTimeoutOnAc))
            standbyTimeoutOnAc = int.Parse(_mainWindowPersistentState.TextStandbyTimeoutOnAc);

        if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextStandbyTimeoutOnBattery))
            standbyTimeoutOnBattery = int.Parse(_mainWindowPersistentState.TextStandbyTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_mainWindowPersistentState.TextHibernateTimeoutOnAc))
            hibernateTimeoutOnAc = int.Parse(_mainWindowPersistentState.TextHibernateTimeoutOnAc);
        
        _powerHelper.SetPowerSettingsTo(monitorTimeoutOnAc, monitorTimeoutOnBattery, standbyTimeoutOnAc, standbyTimeoutOnBattery, hibernateTimeoutOnAc);
    }

    // ReSharper disable once CognitiveComplexity because it's extremely linear and it's fine
    private void WorkAllApplicationInstallCheckboxes()
    {
        // Install 7zip no matter what because we need it later for the portable apps
        new ChocolateyInstaller() { ChocolateyId = "7Zip" }.ExecuteInstall(_logger);

        // Run non-executable installers and get them out of the way first since they require no user interaction
        foreach (var installer in _mainWindowPersistentState.AvailableInstalls)
        {
            if (!installer.IsSelected) continue;

            if (installer is not ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }

        // Do the executableInstallers last
        foreach (var installer in _mainWindowPersistentState.AvailableInstalls)
        {
            if (!installer.IsSelected) continue;

            if (installer is ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }
    }

    private void AvailableInstallsListView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e) => ControlHelpers.OnPreviewMouseWheelMove(sender, e);

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        CheckStageAndWorkOnRerun();
    }
}