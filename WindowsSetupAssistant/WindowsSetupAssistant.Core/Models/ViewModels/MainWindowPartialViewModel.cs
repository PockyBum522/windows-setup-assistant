using System.Collections.ObjectModel;
using System.ComponentModel;
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
    [ObservableProperty] private bool _isCheckedSetFolderasdfasdfasViewOptions;
    
    // Time 
    [ObservableProperty] private bool _isCheckedSyncSystemTime;
    
    // Windows UI
    [ObservableProperty] private bool _isCheckedSetWindowsToDarkTheme;
    [ObservableProperty] private bool _isCheckedBlackTitleBars;
    [ObservableProperty] private bool _isCheckedDisableNewsAndInterestsOnTaskbar;
    [ObservableProperty] private bool _isCheckedSetWallpaperToDarkDefaultWallpaper;
    
    /// <summary>
    /// If this is true, taskbar search icon will be completely hidden
    /// </summary>
    public bool IsCheckedSetTaskbarSearchToHidden
    {
        get => _isCheckedSetTaskbarSearchToHidden;
        set
        {
            SetProperty(ref _isCheckedSetTaskbarSearchToHidden, value);

            if (IsCheckedSetTaskbarSearchToHidden &&
                IsCheckedSetTaskbarSearchToIcon)
            {
                IsCheckedSetTaskbarSearchToIcon = false;
            }
        }
    }

    /// <summary>
    /// If this is true, taskbar search icon will be collapsed to just an icon
    /// </summary>
    public bool IsCheckedSetTaskbarSearchToIcon
    {
        get => _isCheckedSetTaskbarSearchToIcon;
        set
        {
            SetProperty(ref _isCheckedSetTaskbarSearchToIcon, value);

            if (IsCheckedSetTaskbarSearchToIcon &&
                IsCheckedSetTaskbarSearchToHidden)
            {
                IsCheckedSetTaskbarSearchToHidden = false;
            }
        }
    }

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
    
    private bool _isCheckedSetTaskbarSearchToHidden;
    private bool _isCheckedSetTaskbarSearchToIcon;
}