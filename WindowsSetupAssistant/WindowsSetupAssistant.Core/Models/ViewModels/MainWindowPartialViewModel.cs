using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ViewModels;

/// <summary>
/// This holds our main window checkboxes state and available installers loaded from json
/// </summary>
public partial class MainWindowPartialViewModel : ObservableObject
{
    //Common tasks
    [ObservableProperty] private bool _isCheckedUpdateWindows;
    [ObservableProperty] private bool _isCheckedSetSystemTimeZoneToEastern;
    [ObservableProperty] private bool _isCheckedSetFolderViewOptions;
    
    // Time 
    [ObservableProperty] private bool _isCheckedSyncSystemTime;
    
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
    [ObservableProperty] private ObservableCollection<IInstallable> _availableInstalls = new();
}