using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Logic;
using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables;

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
        
        DataContext = _currentState.MainWindowPartialViewModel;
        
        LoadAvailableInstallersFromJsonFile();
        
        InitializeComponent();

        _windowsUpdater = new(_logger);
        
        if (_currentState.ScriptStage == ScriptStageEnum.Uninitialized) return;
        
        // Otherwise:
        CheckStageAndWorkOnRerun();
    }
    
    
    private void SetUpDummyInstallableApplications()
    {
        var installableApps = new List<BaseInstaller>();
        
        installableApps.Add(new ExecutableInstaller(_logger)
        {
            DisplayName = "TeamViewer v11",
            FileName = "TeamViewer_11.exe"
        });

        installableApps.Add(new ChocolateyInstaller(_logger)
        {
            DisplayName = "7-Zip",
            ChocolateyId = "7Zip"
        });

        installableApps.Add(new ChocolateyInstaller(_logger)
        {
            DisplayName = "Google Chrome",
            ChocolateyId = "googlechrome"
        });

        installableApps.Add(new ArchiveInstaller(_logger)
        {
            DisplayName = "Yubico Authenticator (Modified)",
            DestinationPath = @"C:\PortableApplications\Yubico Authenticator\",
            ArchiveFilename = "Yubico Authenticator.7z"
        });

        installableApps.Add(new PortableApplicationInstaller(_logger)
        {
            DisplayName = "CLCL",
            FolderName = "CLCL",
            DestinationPath = @"C:\PortableApplications\CLCL\"
        });

        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };

        Console.WriteLine(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);
        
        using var jsonStateFileWriter = new StreamWriter(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);

        using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };

        serializer.Serialize(jsonStateWriter, installableApps);


        // var jsonStateRaw = File.ReadAllText(StatePath);
        //
        // MainWindowPartialViewModel =
        //     JsonConvert.DeserializeObject<MainWindowPartialViewModel>(jsonStateRaw, settings) ??
        //     new MainWindowPartialViewModel();
    }
    
    private void ClearAll_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllCheckboxes();
    }

    private void ClearAllCheckboxes()
    {
        foreach (var propertyInfo in _currentState.MainWindowPartialViewModel.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_currentState.MainWindowPartialViewModel, false);
        }
    }

    private void SelectAll_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (var propertyInfo in _currentState.MainWindowPartialViewModel.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(_currentState.MainWindowPartialViewModel, true);
        }
    }

    private void ExecuteAllSelected()
    {
        _currentState.ScriptStage = ScriptStageEnum.Uninitialized;
        _currentState.SaveCurrentState();
        
        WorkAllUiRelatedCheckBoxes();
        
        WorkAllTimeRelatedCheckboxes();
        
        WorkAllWindowsSettingsRelatedCheckboxes();

        if (_currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows)
        {
            _windowsUpdater.UpdateWindows();
        
            _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedOnce;
            _currentState.SaveCurrentState();
            _currentState.RebootComputer();    
        }
        else
        {
            _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;
            _currentState.SaveCurrentState();

            CheckStageAndWorkOnRerun();
        }
    }

    private void CheckStageAndWorkOnRerun()
    {
        switch (_currentState.ScriptStage)
        {
            case ScriptStageEnum.WindowsHasBeenUpdatedOnce:
                
                _windowsUpdater.UpdateWindows();

                _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedTwice;
                
                _currentState.SaveCurrentState();
                _currentState.RebootComputer();
                
                break;
            
            case ScriptStageEnum.WindowsHasBeenUpdatedTwice:
                                
                _windowsUpdater.UpdateWindows();

                _currentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;
                
                _currentState.SaveCurrentState();
                _currentState.RebootComputer();

                break;
            
            case ScriptStageEnum.WindowsHasBeenUpdatedFully:

                if (_currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows)
                {
                    _windowsUpdater.UpdateWindows();
                }

                WorkAllApplicationInstallCheckboxes(); 
                
                _currentState.DeleteSavedChoicesAndStageOnDisk();
                
                break;
        }
    }
    
    private void LoadAvailableInstallersFromJsonFile()
    {
        var availableInstallsJsonRaw = File.ReadAllText(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);
        
        var availableInstalls = JsonConvert.DeserializeObject<List<BaseInstaller>>(availableInstallsJsonRaw, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new DefaultSerializationBinder()
        });

        if (availableInstalls is null) throw new NullReferenceException();
        
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
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToHidden) uiHelper.SetFolderViewOptions();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToIcon) uiHelper.SetFolderViewOptions();
    }

    private void WorkAllTimeRelatedCheckboxes()
    {
        var timeHelper = new TimeHelper(_logger);

        if (_currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern) timeHelper.SetSystemTimeZone("Eastern Standard Time");
    }
    
    private void WorkAllWindowsSettingsRelatedCheckboxes()
    {
        var windowsSettingsHelper = new WindowsSettingsHelper(_logger);

        if (_currentState.MainWindowPartialViewModel.IsCheckedDisableSleepWhenOnAc) windowsSettingsHelper.DisableSleepWhenPoweredByAc();
        
        // TODO: if (_currentState.MainWindowPartialViewModel.IsCheckedDisableNetworkThumbnails) windowsSettingsHelper.DisableNetworkThumbnails();

        // TODO: Set all monitors scaling to 100%
    }
    
    // ReSharper disable once CognitiveComplexity because it's extremely linear and it's fine
    private void WorkAllApplicationInstallCheckboxes()
    {
        var chocolateyHelper = new ChocolateyHelper(_logger);

        // Install 7zip no matter what because we need it later for the portable apps
        chocolateyHelper.ChocoInstall(new ChocolateyInstaller(_logger){ChocolateyId = "7Zip"});

        foreach (var installer in _currentState.MainWindowPartialViewModel.AvailableInstalls)
        {
            if (!installer.IsSelected) continue;

            installer.ExecuteInstall();
        }
        
        // TODO: Associate 7zFM.exe with .7z and .zip files
        
        // TODO: Set Chrome as default for .html files

        // TODO: Make default pictures app IrfanView if installed
        
        // TODO: Displayfusion install did not make the shell icons when right clicking the desktop

        // TODO: Uninstall windows store version of spotify

        // TODO: Not implemented: InstallSshServerPersonal();
        
        // TODO: CopyPsToolsToWindowsFolder();
        
        // TODO: Start Jetbrains toolbox, see if installations can be performed automatically
    }

    private void SelectAllCommon_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllCheckboxes();
        
        //SelectAllCommon();
    }

    private void StartExecution_OnClick(object sender, RoutedEventArgs e)
    {
        ExecuteAllSelected();
    }

    private void RunTestMethod_OnClick(object sender, RoutedEventArgs e)
    {
        var message = string.Join("\r\n", _currentState.MainWindowPartialViewModel.AvailableInstalls);

        _logger.Information("{CurrentInstallsStates}", message);

    }

    private void AvailableInstallsListView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var listView = (ListView)sender;
        var scv = (ScrollViewer)listView.Parent;

        var scrollAmount = e.Delta / 2f;
        
        scv.ScrollToVerticalOffset(scv.VerticalOffset - scrollAmount);
        
        e.Handled = true;
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

// TODO: Displayfusion titlebar button off

// TODO: Look through true launch bar folder and see what else should be installed 

// TODO: Prompt to setup windows hello

// TODO: Final reboot and notification saying it's all finished and log path
