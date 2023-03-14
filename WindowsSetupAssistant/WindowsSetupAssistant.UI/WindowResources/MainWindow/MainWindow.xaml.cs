using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Logic.MainWindowHelpers;
using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;
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
    private readonly WindowsUpdater _windowsUpdater;
    private readonly PowerHelper _powerHelper;
    private readonly WindowsHostnameHelper _windowsHostnameHelper;
    private readonly TimeSettingsSectionBuilder _timeSettingsSectionBuilder;
    private readonly TaskbarSettingsSectionBuilder _taskbarSettingsSectionBuilder;
    private readonly DesktopSettingsSectionBuilder _desktopSettingsSectionBuilder;
    private readonly WindowSettingsSectionBuilder _windowSettingsSectionBuilder;
    private readonly CurrentState _currentState;

    /// <summary>
    /// Main window constructor, loads in JSON files that need to be shown as controls, sets DataContext, sets up
    /// exception handling, checks what stage is saved to disk and works whatever needs to happen if there
    /// is one.
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="windowsUpdater">Windows update helper</param>
    /// <param name="powerHelper">Windows power settings helper</param>
    /// <param name="windowsHostnameHelper">Windows hostname helper</param>
    /// <param name="availableApplicationsJsonLoader">Loads "Available Applications.json" file into CurrentState</param>
    /// <param name="timeSettingsSectionBuilder">Time settings section builder</param>
    /// <param name="taskbarSettingsSectionBuilder">Taskbar settings section builder</param>
    /// <param name="desktopSettingsSectionBuilder">Desktop settings section builder</param>
    /// <param name="windowSettingsSectionBuilder">Window settings section builder</param>
    public MainWindow(
        ILogger logger, 
        WindowsUpdater windowsUpdater,
        PowerHelper powerHelper,
        WindowsHostnameHelper windowsHostnameHelper,
        AvailableApplicationsJsonLoader availableApplicationsJsonLoader,
        TimeSettingsSectionBuilder timeSettingsSectionBuilder,
        TaskbarSettingsSectionBuilder taskbarSettingsSectionBuilder,
        DesktopSettingsSectionBuilder desktopSettingsSectionBuilder,
        WindowSettingsSectionBuilder windowSettingsSectionBuilder)
    {
        _logger = logger;
        _windowsUpdater = windowsUpdater;
        _powerHelper = powerHelper;
        _windowsHostnameHelper = windowsHostnameHelper;
        _timeSettingsSectionBuilder = timeSettingsSectionBuilder;
        _taskbarSettingsSectionBuilder = taskbarSettingsSectionBuilder;
        _desktopSettingsSectionBuilder = desktopSettingsSectionBuilder;
        _windowSettingsSectionBuilder = windowSettingsSectionBuilder;
        _currentState = new(_logger);

        DataContext = _currentState.MainWindowPartialViewModel;
        
        availableApplicationsJsonLoader.LoadAvailableInstallersFromJsonFile(_currentState);

        LoadAllSettingsSections();
        
        InitializeComponent();

        if (_currentState.ScriptStage == ScriptStageEnum.Uninitialized) return;

        // Otherwise:
        CheckStageAndWorkOnRerun();
    }
    
    private void Test_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void LoadAllSettingsSections()
    {
        var timeSection = _timeSettingsSectionBuilder.MakeSection();
        _currentState.MainWindowPartialViewModel.SettingsSections.Add(timeSection);

        var taskbarSection = _taskbarSettingsSectionBuilder.MakeSection();
        _currentState.MainWindowPartialViewModel.SettingsSections.Add(taskbarSection);
        
        var desktopSection = _desktopSettingsSectionBuilder.MakeSection();
        _currentState.MainWindowPartialViewModel.SettingsSections.Add(desktopSection);

        var windowSection = _windowSettingsSectionBuilder.MakeSection();
        _currentState.MainWindowPartialViewModel.SettingsSections.Add(windowSection);

        // Load registry files from disk
        var registryFilePaths = GetAllRegistryFilePathsFromResources();

        foreach (var filePath in registryFilePaths)
        {
            new RegistryFileAsOptionLoader().AddRegistryFileAsOption(filePath, _currentState);
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

    private void StartExecution_OnClick(object sender, RoutedEventArgs e)
    {
        ExecuteAllSelected();
    }

    private void AvailableInstallsListView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var listView = (ListView)sender;
        var scv = (ScrollViewer)listView.Parent;

        var scrollAmount = e.Delta / 2f;

        scv.ScrollToVerticalOffset(scv.VerticalOffset - scrollAmount);

        e.Handled = true;
    }

    private void SaveProfile_OnClick(object sender, RoutedEventArgs e)
    {
        var profilesDirectoryPath = Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Configuration",
                "Profiles")
            .Replace("/", @"\");

        profilesDirectoryPath = Path.GetFullPath(profilesDirectoryPath);

        Console.WriteLine(profilesDirectoryPath);

        var fileDialog = new SaveFileDialog()
            { Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

        if (fileDialog.ShowDialog() != true) return;

        var fullSelectedFilePath = fileDialog.FileName;

        _currentState.SaveCurrentStateAsProfile(fullSelectedFilePath);
    }

    private void LoadProfile_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllControls();

        var profilesDirectoryPath = Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Configuration",
                "Profiles")
            .Replace("/", @"\");

        profilesDirectoryPath = Path.GetFullPath(profilesDirectoryPath);

        Console.WriteLine(profilesDirectoryPath);

        var fileDialog = new OpenFileDialog()
            { Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

        if (fileDialog.ShowDialog() != true) return;

        var fullSelectedFilePath = fileDialog.FileName;

        var loadedProfileState = _currentState.GetStateFromDisk(fullSelectedFilePath);

        foreach (var install in loadedProfileState.AvailableInstalls)
        {
            if (install.IsSelected)
                GetInstallInListByDisplayName(install.DisplayName).IsSelected = true;
        }

        SetAllSettingsCheckBoxesToProfile(loadedProfileState);

        _currentState.MainWindowPartialViewModel.TextHostname = loadedProfileState.TextHostname;
    }
    
    private void SelectAll_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (var propertyInfo in _currentState.MainWindowPartialViewModel.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_currentState.MainWindowPartialViewModel, true);
        }

        foreach (var installer in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            installer.IsSelected = true;
        }
    }
    
    private void ClearAll_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllControls();
    }

    private void ClearAllControls()
    {
        foreach (var propertyInfo in _currentState.MainWindowPartialViewModel.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_currentState.MainWindowPartialViewModel, false);
        }

        foreach (var installer in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            installer.IsSelected = false;
        }

        _currentState.MainWindowPartialViewModel.TextHostname = "";
    }

    private void ExecuteAllSelected()
    {
        _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedOnce;
        _currentState.SaveCurrentStateForReboot();

        ExecuteSelectedSettingsInAllSections();

        if (_currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows)
        {
            _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedOnce;
            _currentState.SaveCurrentStateForReboot();
            
            _windowsUpdater.UpdateWindows();
        
            _currentState.RebootComputerAndExit();    
        }
        else
        {
            _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;
            _currentState.SaveCurrentStateForReboot();
        
            CheckStageAndWorkOnRerun();
        }
    }

    private void ExecuteSelectedSettingsInAllSections()
    {
        foreach (var section in _currentState.MainWindowPartialViewModel.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (setting.IsSelected)
                {
                    _logger.Debug("Executing setting: {Name}", setting.DisplayName);
                    
                    setting.ExecuteSetting?.Invoke();
                }
            }
        }
    }

    private void CheckStageAndWorkOnRerun()
    {
        // Set long sleep and monitor off times so it doesn't sleep during install
        // _windowsSettingsHelper.SetPowerSettingsTo();

        switch (_currentState.ScriptStage)
        {
            case ScriptStageEnum.WindowsHasBeenUpdatedOnce:

                _windowsUpdater.UpdateWindows();

                _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedTwice;

                _currentState.SaveCurrentStateForReboot();
                _currentState.RebootComputerAndExit();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedTwice:

                _windowsUpdater.UpdateWindows();

                _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;

                _currentState.SaveCurrentStateForReboot();
                _currentState.RebootComputerAndExit();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedFully:

                if (_currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows)
                {
                     _windowsUpdater.UpdateWindows();
                }

                WorkAllApplicationInstallCheckboxes();

                _currentState.DeleteSavedChoicesAndStageOnDisk();

                if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextHostname))
                {
                    _windowsHostnameHelper.ChangeHostName(_currentState.MainWindowPartialViewModel.TextHostname);
                }

                SetPowerSettingsToUserChoicesAtStart();

                _currentState.PromptToRebootComputerAndExit();

                break;
        }
    }

    private void SetPowerSettingsToUserChoicesAtStart()
    {
        var monitorTimeoutOnAc = 20; // Start with defaults
        var monitorTimeoutOnBattery = 5;
        var standbyTimeoutOnAc = 60;
        var standbyTimeoutOnBattery = 10;
        var hibernateTimeoutOnAc = 0;
        
        if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextMonitorTimeoutOnAc))
            monitorTimeoutOnAc = int.Parse(_currentState.MainWindowPartialViewModel.TextMonitorTimeoutOnAc);
        
        if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextMonitorTimeoutOnBattery))
            monitorTimeoutOnBattery = int.Parse(_currentState.MainWindowPartialViewModel.TextMonitorTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextStandbyTimeoutOnAc))
            standbyTimeoutOnAc = int.Parse(_currentState.MainWindowPartialViewModel.TextStandbyTimeoutOnAc);

        if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextStandbyTimeoutOnBattery))
            standbyTimeoutOnBattery = int.Parse(_currentState.MainWindowPartialViewModel.TextStandbyTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_currentState.MainWindowPartialViewModel.TextHibernateTimeoutOnAc))
            hibernateTimeoutOnAc = int.Parse(_currentState.MainWindowPartialViewModel.TextHibernateTimeoutOnAc);
        
        _powerHelper.SetPowerSettingsTo(monitorTimeoutOnAc, monitorTimeoutOnBattery, standbyTimeoutOnAc, standbyTimeoutOnBattery, hibernateTimeoutOnAc);
    }

    // ReSharper disable once CognitiveComplexity because it's extremely linear and it's fine
    private void WorkAllApplicationInstallCheckboxes()
    {
        // Install 7zip no matter what because we need it later for the portable apps
        new ChocolateyInstaller() { ChocolateyId = "7Zip" }.ExecuteInstall(_logger);

        // Run non-executable installers and get them out of the way first since they require no user interaction
        foreach (var installer in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            if (!installer.IsSelected) continue;

            if (installer is not ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }

        // Do the executableInstallers last
        foreach (var installer in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            if (!installer.IsSelected) continue;

            if (installer is ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }
    }

    private void SetAllSettingsCheckBoxesToProfile(MainWindowPartialViewModel loadedProfileState)
    {

    }

    private IInstallable GetInstallInListByDisplayName(string displayName)
    {
        foreach (var install in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            if (displayName == install.DisplayName)
                return install;
        }

        // Just return something it can't act on that will affect the main window
        return new ArchiveInstaller();
    }

}