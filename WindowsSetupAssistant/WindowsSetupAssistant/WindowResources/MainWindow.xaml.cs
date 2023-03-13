using System.Collections.Generic;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Interfaces;
using WindowsSetupAssistant.Logic;
using WindowsSetupAssistant.Logic.TaskHelpers;
using WindowsSetupAssistant.Models;
using WindowsSetupAssistant.Models.IInstallables;

namespace WindowsSetupAssistant.WindowResources;

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
        
        var availableInstalls = 
            JsonConvert.DeserializeObject<List<BaseInstaller>>(availableInstallsJsonRaw, new JsonSerializerSettings()) ??
            new List<BaseInstaller>();

        foreach (var availableInstall in availableInstalls)
        {
            _currentState.MainWindowPartialViewModel.AvailableInstalls.Add(availableInstall);
        }
    }
    
    // private void SetUpDummyInstallableApplications()
    // {
    //     InstallableApplications.Add(new ExecutableInstaller()
    //         {
    //             DisplayName = "TeamViewer v11",
    //             FileName = "TeamViewer_11.exe"
    //         });
    //     
    //     InstallableApplications.Add(new ChocolateyInstaller()
    //         {
    //             DisplayName = "7-Zip",
    //             ChocolateyId = "7Zip"
    //         });
    //
    //     InstallableApplications.Add(new ChocolateyInstaller()
    //         {
    //             DisplayName = "Google Chrome",
    //             ChocolateyId = "googlechrome"
    //         });
    //     
    //     InstallableApplications.Add(new ArchiveInstaller()
    //     {
    //         DisplayName = "Yubico Authenticator (Modified)",
    //         DestinationPath = @"C:\PortableApplications\Yubico Authenticator\",
    //         ArchiveFilename = "Yubico Authenticator.7z"
    //     });
    //     
    //     InstallableApplications.Add(new PortableApplicationInstaller()
    //     {
    //         DisplayName = "CLCL",
    //         FolderName = "CLCL",
    //         DestinationPath = @"C:\PortableApplications\CLCL\"
    //     });
    //     
    //     var serializer = new JsonSerializer
    //     {
    //         NullValueHandling = NullValueHandling.Ignore
    //     };
    //
    //     using var jsonStateFileWriter = new StreamWriter(jsonInstallsFilePath);
    //     
    //     using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };
    //     
    //     serializer.Serialize(jsonStateWriter, InstallableApplications);
    //     
    //     
    //     var jsonStateRaw = File.ReadAllText(StatePath);
    //     
    //     MainWindowPartialViewModel =
    //         JsonConvert.DeserializeObject<MainWindowPartialViewModel>(jsonStateRaw, settings) ??
    //         new MainWindowPartialViewModel();
    // }
    
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
        chocolateyHelper.ChocoInstall("7Zip");

        var applicationInstallHelper = new ApplicationInstallHelper(_logger);
        
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

//Do everything that needs normal privileges
//
//And then run this as admin, too
//new ProcessElevator(Logger).ElevateThisProcessNow();
//
//And then close
//await Task.Delay(2000);
//
//Environment.Exit(0);


//
//private static bool IsProcessIntentionallyRunningAsAdmin(string[] args)
//{
//foreach (var argument in args)
//{
//if (!argument.Contains("RunningAsAdmin")) continue;
//
//Otherwise:
//_logger.Information("RunningAsAdmin switch found");
//
//return true;
//}
//
//Otherwise:
//if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) return false;
//
//throw new Exception("Process must NOT be running as admin but is");
//}
//}

//private void InstallTrueLaunchBar()
//{
//_logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
//
// TODO: Find silent install for this. Portable maybe?
//ReSharper disable once StringLiteralTypo
//var teamViewerFullPath = Path.Join(_runFromDirectory, "Resources", "truelaunchbar75.exe");
//
//Process.Start(teamViewerFullPath, "/silent");
//}

//private void RebootAndResumeThisScript(ScriptStage newScriptStage)
//{
//_logger.Information("Running {ThisName} script stage: {Stage}", 
//System.Reflection.MethodBase.GetCurrentMethod()?.Name, 
//newScriptStage);
//
//var startupFolderPath = Environment.ExpandEnvironmentVariables("%AllUsersProfile%") + @"\Start Menu\Programs\Startup";
//		
//var startupBatPath = Path.Join(startupFolderPath, "Re-RunAdminBatFileAutomatically.bat");
//
//Add batch file to startup that will call this after reboot
//ReSharper disable once StringLiteralTypo
//var batchFileContents = @$"
//echo Loading bootloader batch file as admin again.
//
//echo Delaying for internet...
//
//timeout 60
//
//echo Now re-running bootstrapper bat file as admin
//
//powershell start '{Environment.ProcessPath}' -v runas -ArgumentList ""RunningAsAdmin""
//";
//
//File.WriteAllText(startupBatPath, batchFileContents);
//
//Save current script state before reboot
//CurrentState.ScriptStage = newScriptStage;
//
//CurrentState.SaveCurrentState();
//
//Notify user
//_logger.Information("{Message}", "Exiting script temporarily. will reboot and re-run admin bat file on next startup...");
//
//Reboot
//Process.Start("shutdown", "/r /t 0");
//
//Hang for days
//Thread.Sleep(999999 * 1000);
//}
//
//Helper Functions:

//private static bool ConvertYesNoResponse(string response, bool defaultValue)
//{
//response = response.ToUpper();
//
//ReSharper disable once ConvertSwitchStatementToSwitchExpression because I think this is cleaner
//switch (response)
//{
//case "Y" or "YES" or "YEP" or "YEAH":
//return true;
//
//case "N" or "NO" or "NAH" or "NOPE":
//return false;
//}
//
//return defaultValue; 
//}

//private void Print(string message)
//{
//_logger.Information("{Message}", message);
//
//_logger.Information("{Message}", message);
//}
//
