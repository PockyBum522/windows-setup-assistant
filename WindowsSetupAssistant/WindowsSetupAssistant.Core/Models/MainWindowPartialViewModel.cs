using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ViewModels;

/// <summary>
/// This holds our main window checkboxes state and available installers loaded from json
/// </summary>
public partial class MainWindowPartialViewModel : ObservableObject
{
    // Settings sections
    [ObservableProperty] private ObservableCollection<SettingsSection> _settingsSections = new();
    
    // Application installers
    [ObservableProperty] private ObservableCollection<IInstallable> _availableInstalls = new();
    
    [ObservableProperty] private string _textHostname = "";
    
    // Power values from user
    [ObservableProperty] private string _textMonitorTimeoutOnAc = "";
    [ObservableProperty] private string _textMonitorTimeoutOnBattery = "";
    [ObservableProperty] private string _textStandbyTimeoutOnAc = "";
    [ObservableProperty] private string _textStandbyTimeoutOnBattery = "";
    [ObservableProperty] private string _textHibernateTimeoutOnAc = "";
    [ObservableProperty] private string _textHibernateTimeoutOnBattery = "";
    
    // Update windows
    [ObservableProperty] private bool _isCheckedUpdateWindows;
}