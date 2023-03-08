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
using WindowsSetupAssistant.Models.ISelectables.Installers;

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
    
    private readonly List<ISelectable> InstallableApplications = new ();

    public MainWindow(ILogger logger, ExceptionHandler exceptionHandler)
    {
        _logger = logger;

        exceptionHandler.SetupExceptionHandlingEvents();

        _currentState = new(_logger);
        
        DataContext = _currentState.MainWindowPartialViewModel;

        SetUpDummyInstallableApplications();
        
        InitializeComponent();

        _windowsUpdater = new(_logger);
        
        if (_currentState.ScriptStage == ScriptStageEnum.Uninitialized) return;
        
        // Otherwise:
        CheckStageAndWorkOnRerun();
    }

    private void SelectAllForPersonalPCs_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO: Update what this selects
        
        ClearAllCheckboxes();
            
        SelectAllCommon();
        
        // Windows Settings
        _currentState.MainWindowPartialViewModel.IsCheckedBlackTitleBars = true;
        _currentState.MainWindowPartialViewModel.IsCheckedSetFolderViewOptions = true;
        _currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToHidden = true;
        
        // Installs
        _currentState.MainWindowPartialViewModel.IsCheckedInstallSteam = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallDropBox = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallObsidian = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallPushBullet = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallBitWarden = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallJetbrainsToolbox = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallMp3Tag = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallTelegram = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallSyncBack = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallDisplayFusion = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallMusicBee = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallYubicoAuthenticator = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallClcl = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallNgrok = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallWindowsUpdateBlocker = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallArduinoLatest = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallArduino1819 = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallVeraCrypt = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallQBitTorrent = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallZoom = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallTrueLaunchBar = true;
    }

    private void SelectAllForOscExhibitPCs_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO: Update what this selects

        ClearAllCheckboxes();
        
        SelectAllCommon();
        
        _currentState.MainWindowPartialViewModel.IsCheckedSetTaskbarSearchToIcon = true;
    }

    private void SelectAllCommon()
    {
        // TODO: Update what this selects
        
        //Common tasks
        _currentState.MainWindowPartialViewModel.IsCheckedUpdateWindows = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedSetSystemTimeZoneToEastern = true; 

        // Windows UI
        _currentState.MainWindowPartialViewModel.IsCheckedSetWindowsToDarkTheme =  true;
        _currentState.MainWindowPartialViewModel.IsCheckedDisableNewsAndInterestsOnTaskbar = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedSetWallpaperToDarkDefaultWallpaper = true;

        // Windows settings
        _currentState.MainWindowPartialViewModel.IsCheckedDisableSleepWhenOnAc = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedDisableNetworkThumbnails = true;

        // Application installers
        _currentState.MainWindowPartialViewModel.IsCheckedInstallTeamViewer11 = 
        _currentState.MainWindowPartialViewModel.IsCheckedInstall7Zip = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallGoogleChrome = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallVisualStudioCode = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallPython = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallFireFox = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallMicrosoftEdge = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallEverything = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallVlc = true; 
        _currentState.MainWindowPartialViewModel.IsCheckedInstallAudacity = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallHandbrake = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallFileZilla = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallWinScp = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallPutty = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallWinMerge = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallKrita = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallBlender = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallPaintDotNet = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallGimp = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallIrfanView = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallInkscape = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallRufus = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallGreenShot = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallFoxitReader = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallLibreOffice = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallCdBurnerXp = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallRevo = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallWinDirStat = true;
        _currentState.MainWindowPartialViewModel.IsCheckedInstallPowerToys = true;
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
    
    private void SetUpDummyInstallableApplications()
    {
        InstallableApplications.Add(new ExecutableInstaller()
            {
                DisplayName = "TeamViewer v11",
                FileName = "TeamViewer_11.exe"
            });
        
        InstallableApplications.Add(new ChocolateyInstaller()
            {
                DisplayName = "7-Zip",
                ChocolateyId = "7Zip"
            });

        InstallableApplications.Add(new ChocolateyInstaller()
            {
                DisplayName = "Google Chrome",
                ChocolateyId = "googlechrome"
            });
        
        InstallableApplications.Add(new ArchiveInstaller()
        {
            DisplayName = "Yubico Authenticator (Modified)",
            DestinationPath = @"C:\PortableApplications\Yubico Authenticator\",
            ArchiveFilename = "Yubico Authenticator.7z"
        });
        
        InstallableApplications.Add(new PortableApplicationInstaller()
        {
            DisplayName = "CLCL",
            FolderName = "CLCL",
            DestinationPath = @"C:\PortableApplications\CLCL\"
        });
        
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        using var jsonStateFileWriter = new StreamWriter(@"D:\Dropbox\Apps\New Workstation Setup\WindowsSetupAssistant\Resources\Configuration\AvailableInstalls.json");
        
        using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };
        
        serializer.Serialize(jsonStateWriter, InstallableApplications);
        
        
        // var jsonStateRaw = File.ReadAllText(StatePath);
        //
        // MainWindowPartialViewModel =
        //     JsonConvert.DeserializeObject<MainWindowPartialViewModel>(jsonStateRaw, settings) ??
        //     new MainWindowPartialViewModel();
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
        chocolateyHelper.ChocoInstall("7Zip");

        var applicationInstallHelper = new ApplicationInstallHelper(_logger);
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallTeamViewer11) applicationInstallHelper.InstallTeamViewer11();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstall7Zip)
        {
            // TODO: Associate 7zFM.exe with .7z and .zip files
        }

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallGoogleChrome)
        {
            chocolateyHelper.ChocoInstall("GoogleChrome");
        
            // TODO: Set Chrome as default for .html files
        }
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallVisualStudioCode) chocolateyHelper.ChocoInstall("VSCode");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallTrueLaunchBar) applicationInstallHelper.InstallTrueLaunchBar();
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallDotNet5DesktopRuntime) chocolateyHelper.ChocoInstall("dotnet5-desktop-runtime");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallDotNet6DesktopRuntime) chocolateyHelper.ChocoInstall("dotnet6-desktop-runtime");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallYubicoAuthenticator) applicationInstallHelper.InstallArchiveFromPortableApplications("Yubico Authenticator.7z");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallClcl) applicationInstallHelper.InstallDirectoryFromPortableApplications("CLCL"); // TODO: Add shortcut to startup
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallMouseMover) applicationInstallHelper.InstallDirectoryFromPortableApplications("MouseMover");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallNgrok) applicationInstallHelper.InstallDirectoryFromPortableApplications("ngrok");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallRufus) applicationInstallHelper.InstallDirectoryFromPortableApplications("Rufus");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallWindowsUpdateBlocker) applicationInstallHelper.InstallDirectoryFromPortableApplications("Windows Update Blocker v1.1");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallArduino1819) applicationInstallHelper.InstallArchiveFromPortableApplications("arduino-1.8.19 (For Line Wobbler).7z");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallArduinoLatest) applicationInstallHelper.InstallArchiveFromPortableApplications("arduino-latest (2.0.4).7z");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallOmnidome) applicationInstallHelper.InstallArchiveFromPortableApplications("Omnidome.7z");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallVeraCrypt) applicationInstallHelper.InstallArchiveFromPortableApplications("VeraCrypt_1_25_9.7z");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallPython) chocolateyHelper.ChocoInstall("Python");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallFireFox) chocolateyHelper.ChocoInstall("FireFox");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallMicrosoftEdge) chocolateyHelper.ChocoInstall("Microsoft-Edge");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallEverything) chocolateyHelper.ChocoInstall("Everything");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallVlc) chocolateyHelper.ChocoInstall("VLC");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallAudacity)
        {
            chocolateyHelper.ChocoInstall("Audacity");

            chocolateyHelper.ChocoInstall("Audacity-Lame");
        
            chocolateyHelper.ChocoInstall("Audacity-ffmpeg");
        }

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallHandbrake) chocolateyHelper.ChocoInstall("HandBrake");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallFileZilla) chocolateyHelper.ChocoInstall("FileZilla");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallWinScp) chocolateyHelper.ChocoInstall("WinScp");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallPutty) chocolateyHelper.ChocoInstall("Putty");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallWinMerge) chocolateyHelper.ChocoInstall("WinMerge");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallKrita) chocolateyHelper.ChocoInstall("Krita");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallBlender) chocolateyHelper.ChocoInstall("Blender");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallPaintDotNet) chocolateyHelper.ChocoInstall("Paint.net");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallGimp) chocolateyHelper.ChocoInstall("GIMP");

        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallIrfanView)
        {
            chocolateyHelper.ChocoInstall("IrfanView", "", "/desktop /thumbs /group");
            
            chocolateyHelper.ChocoInstall("IrfanViewPlugins");
            
            // TODO: Make default pictures app IrfanView
        }
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallInkscape) chocolateyHelper.ChocoInstall("Inkscape");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallGreenShot) chocolateyHelper.ChocoInstall("GreenShot");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallFoxitReader) chocolateyHelper.ChocoInstall("FoxitReader");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallLibreOffice) chocolateyHelper.ChocoInstall("LibreOffice-Fresh");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallCdBurnerXp) chocolateyHelper.ChocoInstall("CDBurnerXP");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallRevo) chocolateyHelper.ChocoInstall("Revo-Uninstaller");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallWinDirStat) chocolateyHelper.ChocoInstall("WinDirStat");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallQBitTorrent) chocolateyHelper.ChocoInstall("QBitTorrent");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallZoom) chocolateyHelper.ChocoInstall("Zoom");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallSteam) chocolateyHelper.ChocoInstall("Steam-Client");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallDropBox) chocolateyHelper.ChocoInstall("DropBox");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallObsidian) chocolateyHelper.ChocoInstall("Obsidian");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallPushBullet) chocolateyHelper.ChocoInstall("PushBullet");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallBitWarden) chocolateyHelper.ChocoInstall("BitWarden");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallPowerToys) chocolateyHelper.ChocoInstall("PowerToys");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallJetbrainsToolbox) chocolateyHelper.ChocoInstall("JetbrainsToolbox");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallMp3Tag) chocolateyHelper.ChocoInstall("MP3Tag");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallTelegram) chocolateyHelper.ChocoInstall("Telegram");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallSyncBack) chocolateyHelper.ChocoInstall("SyncBack");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallDisplayFusion) chocolateyHelper.ChocoInstall("DisplayFusion"); // TODO: This did not make the shell icons when right clicking the desktop
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallMusicBee) chocolateyHelper.ChocoInstall("MusicBee");
        
        if (_currentState.MainWindowPartialViewModel.IsCheckedInstallRipcord) chocolateyHelper.ChocoInstall("ripcord");
                
        // TODO: Uninstall windows store version of spotify

        // TODO: Not implemented: InstallSshServerPersonal();
        
        // TODO: CopyPsToolsToWindowsFolder();
        
        // TODO: Start Jetbrains toolbox, see if installations can be performed automatically
    }

    private void SelectAllCommon_OnClick(object sender, RoutedEventArgs e)
    {
        ClearAllCheckboxes();
        
        SelectAllCommon();
    }

    private void StartExecution_OnClick(object sender, RoutedEventArgs e)
    {
        ExecuteAllSelected();
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
