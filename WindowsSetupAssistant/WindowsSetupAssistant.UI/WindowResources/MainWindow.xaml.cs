using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Interfaces;
using WindowsSetupAssistant.Core.Logic;
using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.Core.Models.ViewModels;

namespace WindowsSetupAssistant.UI.WindowResources;

///<summary>
///Interaction logic for MainWindow.xaml
///</summary>
[ObservableObject]
#pragma warning disable MVVMTK0033
public partial class MainWindow
#pragma warning restore MVVMTK0033
{
    private readonly ILogger _logger;
    private readonly CurrentState _currentState;
    private readonly WindowsUpdater _windowsUpdater;
    private readonly WindowsSettingsHelper _windowsSettingsHelper;
    private readonly WindowsUiHelper _windowsUiHelper;

    /// <summary>
    /// Main window constructor, loads in JSON files that need to be shown as controls, sets DataContext, sets up
    /// exception handling, checks what stage is saved to disk and works whatever needs to happen if there
    /// is one.
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="exceptionHandler">Injected ExceptionHandler to use</param>
    public MainWindow(ILogger logger, ExceptionHandler exceptionHandler)
    {
        _logger = logger;

        exceptionHandler.SetupExceptionHandlingEvents();

        _currentState = new(_logger);

        _windowsSettingsHelper = new(_logger);
        _windowsUiHelper = new(_logger);
        
        DataContext = _currentState.MainWindowPartialViewModel;
        
        LoadAvailableInstallersFromJsonFile();
        
        InitializeComponent();

        _windowsUpdater = new(_logger);
        
        if (_currentState.ScriptStage == ScriptStageEnum.Uninitialized) return;
        
        // Otherwise:
        CheckStageAndWorkOnRerun();
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

    private void ExecuteAllSelected()
    {
        _currentState.ScriptStage = ScriptStageEnum.Uninitialized;
        _currentState.SaveCurrentStateForReboot();
        
        WorkAllUiRelatedCheckBoxes();
        
        WorkAllTimeRelatedCheckboxes();
        
        WorkAllWindowsSettingsRelatedCheckboxes();

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

    private void CheckStageAndWorkOnRerun()
    {
        // Set long sleep and monitor off times so it doesn't sleep during install
        _windowsSettingsHelper.SetPowerSettingsTo();
        
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
                    _windowsSettingsHelper.ChangeHostName(_currentState.MainWindowPartialViewModel.TextHostname);
                }

                _windowsSettingsHelper.SetPowerSettingsTo(140);

                _windowsUiHelper.CleanDesktopOfAllFilesMatching(new[] { ".lnk", ".ini" });
                
                _currentState.PromptToRebootComputerAndExit();
                
                break;
        }
    }
    
    private void LoadAvailableInstallersFromJsonFile()
    {
        var availableInstallsJsonRaw = File.ReadAllText(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        var availableInstalls = JsonConvert.DeserializeObject<List<IInstallable>>(availableInstallsJsonRaw, jsonSerializerSettings);
        
        if (availableInstalls is null) throw new NullReferenceException();
        
        _currentState.MainWindowPartialViewModel.AvailableInstalls.Clear();
        
        foreach (var availableInstall in availableInstalls)
        {
            _currentState.MainWindowPartialViewModel.AvailableInstalls.Add(availableInstall);
        }
    }
    
    private void WorkAllUiRelatedCheckBoxes()
    {
        var uiHelper = new WindowsUiHelper(_logger);

        if (_currentState.MainWindowPartialViewModel.IsCheckedSetWindowsToDarkTheme) uiHelper.ChangeWindowsThemeToDark();

        if (_currentState.MainWindowPartialViewModel.IsCheckedDisableNewsAndInterestsOnTaskbar) uiHelper.DisableNewsAndInterestsOnTaskbar();

        if (_currentState.MainWindowPartialViewModel.IsCheckedSetWallpaperToDarkDefaultWallpaper) uiHelper.SetWallpaperToDarkDefaultWallpaper();

        if (_currentState.MainWindowPartialViewModel.IsCheckedSetWallpaperToDarkDefaultWallpaper) uiHelper.BlackActiveAndInactiveTitleBars();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern) uiHelper.SetFolderViewOptions();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToHidden) uiHelper.CollapseSearchOnTaskbarToHidden();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToIcon) uiHelper.CollapseSearchOnTaskbarToIcon();
    }

    private void WorkAllTimeRelatedCheckboxes()
    {
        var timeHelper = new TimeHelper(_logger);

        if (_currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern) timeHelper.SetSystemTimeZone("Eastern Standard Time");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSyncSystemTime) timeHelper.SyncSystemTime();
    }
    
    private void WorkAllWindowsSettingsRelatedCheckboxes()
    {
        var windowsSettingsHelper = new WindowsSettingsHelper(_logger);

        if (_currentState.MainWindowPartialViewModel.IsCheckedDisableSleepWhenOnAc) windowsSettingsHelper.SetPowerSettingsTo();
        
        // TODO: if (_currentState.MainWindowPartialViewModel.IsCheckedDisableNetworkThumbnails) windowsSettingsHelper.DisableNetworkThumbnails();

        // TODO: Set all monitors scaling to 100%
    }
    
    // ReSharper disable once CognitiveComplexity because it's extremely linear and it's fine
    private void WorkAllApplicationInstallCheckboxes()
    {
        // Install 7zip no matter what because we need it later for the portable apps
        new ChocolateyInstaller(){ChocolateyId = "7Zip"}.ExecuteInstall(_logger);

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

        // TODO: Associate 7zFM.exe with .7z and .zip files
        
        // TODO: Set Chrome as default for .html files

        // TODO: Make default pictures app IrfanView if installed
        
        // TODO: DisplayFusion install did not make the shell icons when right clicking the desktop

        // TODO: Uninstall windows store version of spotify

        // TODO: Not implemented: InstallSshServerPersonal();
        
        // TODO: CopyPsToolsToWindowsFolder();
        
        // TODO: Start Jetbrains toolbox, see if installations can be performed automatically
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
        
        var fileDialog = new SaveFileDialog(){Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

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
        
        var fileDialog = new OpenFileDialog(){Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

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

    private void SetAllSettingsCheckBoxesToProfile(MainWindowPartialViewModel loadedProfileState)
    {
        _currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows = loadedProfileState.IsCheckedUpdateWindows;
        _currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern = loadedProfileState.IsCheckedSetSystemTimeZoneToEastern;
        _currentState.MainWindowPartialViewModel.IsCheckedSetFolderViewOptions = loadedProfileState.IsCheckedSetFolderViewOptions;

        // Windows UI
        _currentState.MainWindowPartialViewModel.IsCheckedSetWindowsToDarkTheme = loadedProfileState.IsCheckedSetWindowsToDarkTheme;
        _currentState.MainWindowPartialViewModel.IsCheckedBlackTitleBars = loadedProfileState.IsCheckedBlackTitleBars;
        _currentState.MainWindowPartialViewModel.IsCheckedDisableNewsAndInterestsOnTaskbar = loadedProfileState.IsCheckedDisableNewsAndInterestsOnTaskbar;
        _currentState.MainWindowPartialViewModel.IsCheckedSetWallpaperToDarkDefaultWallpaper = loadedProfileState.IsCheckedSetWallpaperToDarkDefaultWallpaper;
        _currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToHidden = loadedProfileState.IsCheckedSetTaskbarSearchToHidden;
        _currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToIcon = loadedProfileState.IsCheckedSetTaskbarSearchToIcon;

        // Desktop
        _currentState.MainWindowPartialViewModel.IsCheckedDeleteShortcutsOffDesktop = loadedProfileState.IsCheckedDeleteShortcutsOffDesktop;
        _currentState.MainWindowPartialViewModel.IsCheckedDeleteIniFilesOffDesktop = loadedProfileState.IsCheckedDeleteIniFilesOffDesktop;
        _currentState.MainWindowPartialViewModel.IsCheckedRemoveRecycleBinOffDesktop = loadedProfileState.IsCheckedRemoveRecycleBinOffDesktop;
	
        // Windows settings
        _currentState.MainWindowPartialViewModel.IsCheckedDisableSleepWhenOnAc = loadedProfileState.IsCheckedDisableSleepWhenOnAc;
        _currentState.MainWindowPartialViewModel.IsCheckedDisableNetworkThumbnails = loadedProfileState.IsCheckedDisableNetworkThumbnails;
        
        // Time settings
        _currentState.MainWindowPartialViewModel.IsCheckedSyncSystemTime = loadedProfileState.IsCheckedSyncSystemTime;
        _currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern = loadedProfileState.IsCheckedSetSystemTimeZoneToEastern;

        // New hostname    
        _currentState.MainWindowPartialViewModel.TextHostname = loadedProfileState.TextHostname;
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

// TODO: Single click instead of double click
        
// TODO: Hide recycle bin icon
        
// TODO: Collapse search to NOTHING
        
// TODO: Prompt to rename pc here

// TODO: Remove mail pinned icon
// TODO: Remove store pinned icon
// TODO: Remove explorer pinned icon
// TODO: Remove edge pinned icon

// TODO: Hide Meet Now By Clock

// TODO: Disable security notifications in startup
// TODO: Disable microsoft edge in startup
// TODO: Disable onedrive in startup

// TODO: DFusion titlebar button off

// TODO: Look through true launch bar folder and see what else should be installed 

// TODO: Prompt to setup windows hello

// TODO: Final reboot and notification saying it's all finished and log path
