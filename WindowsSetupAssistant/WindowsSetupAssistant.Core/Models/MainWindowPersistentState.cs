using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Models;

/// <summary>
/// This holds our main window checkboxes state and available installers loaded from json
/// </summary>
public partial class MainWindowPersistentState : ObservableObject
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
    
    /// <summary>
    /// What stage the setup is in
    /// </summary>
    [ObservableProperty] private ScriptStageEnum _scriptStage;
}