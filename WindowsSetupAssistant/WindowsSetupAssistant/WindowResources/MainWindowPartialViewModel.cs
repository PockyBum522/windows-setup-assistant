using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsSetupAssistant.WindowResources;

public partial class MainWindowPartialViewModel : ObservableObject
{
    //Common tasks
    [ObservableProperty] private bool _isCheckedUpdateWindows;
    [ObservableProperty] private bool _isCheckedSetSystemTimeZoneToEastern;
    [ObservableProperty] private bool _isCheckedSetFolderViewOptions;
    
    // Windows UI
    [ObservableProperty] private bool _isCheckedSetWindowsToDarkTheme;
    [ObservableProperty] private bool _isCheckedBlackTitleBars;
    [ObservableProperty] private bool _isCheckedDisableNewsAndInterestsOnTaskbar;
    [ObservableProperty] private bool _isCheckedSetWallpaperToDarkDefaultWallpaper;
    [ObservableProperty] private bool _isCheckedSetTaskbarSearchToHidden;
    [ObservableProperty] private bool _isCheckedSetTaskbarSearchToIcon;
    
    // Desktop
    [ObservableProperty] private bool _isCheckedDeleteShortcutsOffDesktop;
    [ObservableProperty] private bool _isCheckedDeleteIniFilesOffDesktop;
    [ObservableProperty] private bool _isCheckedRemoveRecycleBinOffDesktop;
        
    // Windows settings
    [ObservableProperty] private bool _isCheckedDisableSleepWhenOnAc;
    [ObservableProperty] private bool _isCheckedDisableNetworkThumbnails;
    
    // New hostname    
    [ObservableProperty] private string _textHostname = "";
    
    // Application installers
    [ObservableProperty] private bool _isCheckedInstallTeamViewer11;
    [ObservableProperty] private bool _isCheckedInstall7Zip;
    [ObservableProperty] private bool _isCheckedInstallGoogleChrome;
    [ObservableProperty] private bool _isCheckedInstallVisualStudioCode;
    [ObservableProperty] private bool _isCheckedInstallDotNet5DesktopRuntime;
    [ObservableProperty] private bool _isCheckedInstallDotNet6DesktopRuntime;
    [ObservableProperty] private bool _isCheckedInstallYubicoAuthenticator;
    [ObservableProperty] private bool _isCheckedInstallClcl;
    [ObservableProperty] private bool _isCheckedInstallMouseMover;
    [ObservableProperty] private bool _isCheckedInstallNgrok;
    [ObservableProperty] private bool _isCheckedInstallRufus;
    [ObservableProperty] private bool _isCheckedInstallWindowsUpdateBlocker;
    [ObservableProperty] private bool _isCheckedInstallArduino1819;
    [ObservableProperty] private bool _isCheckedInstallArduinoLatest;
    [ObservableProperty] private bool _isCheckedInstallOmnidome;
    [ObservableProperty] private bool _isCheckedInstallVeraCrypt;
    [ObservableProperty] private bool _isCheckedInstallPython;
    [ObservableProperty] private bool _isCheckedInstallFireFox;
    [ObservableProperty] private bool _isCheckedInstallMicrosoftEdge;
    [ObservableProperty] private bool _isCheckedInstallEverything;
    [ObservableProperty] private bool _isCheckedInstallTrueLaunchBar;
    [ObservableProperty] private bool _isCheckedInstallVlc;
    [ObservableProperty] private bool _isCheckedInstallAudacity;
    [ObservableProperty] private bool _isCheckedInstallHandbrake;
    [ObservableProperty] private bool _isCheckedInstallFileZilla;
    [ObservableProperty] private bool _isCheckedInstallWinScp;
    [ObservableProperty] private bool _isCheckedInstallPutty;
    [ObservableProperty] private bool _isCheckedInstallWinMerge;
    [ObservableProperty] private bool _isCheckedInstallKrita;
    [ObservableProperty] private bool _isCheckedInstallBlender;
    [ObservableProperty] private bool _isCheckedInstallPaintDotNet;
    [ObservableProperty] private bool _isCheckedInstallGimp;
    [ObservableProperty] private bool _isCheckedInstallIrfanView;
    [ObservableProperty] private bool _isCheckedInstallInkscape;
    [ObservableProperty] private bool _isCheckedInstallGreenShot;
    [ObservableProperty] private bool _isCheckedInstallFoxitReader;
    [ObservableProperty] private bool _isCheckedInstallLibreOffice;
    [ObservableProperty] private bool _isCheckedInstallCdBurnerXp;
    [ObservableProperty] private bool _isCheckedInstallRevo;
    [ObservableProperty] private bool _isCheckedInstallWinDirStat;
    [ObservableProperty] private bool _isCheckedInstallQBitTorrent;
    [ObservableProperty] private bool _isCheckedInstallZoom;
    [ObservableProperty] private bool _isCheckedInstallSteam;
    [ObservableProperty] private bool _isCheckedInstallDropBox;
    [ObservableProperty] private bool _isCheckedInstallObsidian;
    [ObservableProperty] private bool _isCheckedInstallPushBullet;
    [ObservableProperty] private bool _isCheckedInstallBitWarden;
    [ObservableProperty] private bool _isCheckedInstallPowerToys;
    [ObservableProperty] private bool _isCheckedInstallJetbrainsToolbox;
    [ObservableProperty] private bool _isCheckedInstallMp3Tag;
    [ObservableProperty] private bool _isCheckedInstallTelegram;
    [ObservableProperty] private bool _isCheckedInstallSyncBack;
    [ObservableProperty] private bool _isCheckedInstallDisplayFusion;
    [ObservableProperty] private bool _isCheckedInstallMusicBee;
    [ObservableProperty] private bool _isCheckedInstallRipcord;
}