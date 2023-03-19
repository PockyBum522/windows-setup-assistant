using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Models;

/// <summary>
/// This holds our main window checkboxes state and available installers loaded from json
/// </summary>
public partial class SessionPersistentState : ObservableObject
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

    /// <summary>
    /// Friendlier readout of helpful information for debugging
    /// </summary>
    /// <returns>Information about some of the properties and installers</returns>
    public override string ToString()
    {
        var returnString = $"IsCheckedUpdateWindows: {IsCheckedUpdateWindows}" + Environment.NewLine +

                           $"TextMonitorTimeoutOnAc: {TextMonitorTimeoutOnAc}" + Environment.NewLine;

        if (AvailableInstalls.Count > 4)
        {
            returnString +=
                $"Installable 0 DisplayName: {AvailableInstalls[0].DisplayName}, Installable 0 IsSelected: {AvailableInstalls[0].IsSelected}" +
                Environment.NewLine +
                $"Installable 1 DisplayName: {AvailableInstalls[1].DisplayName}, Installable 1 IsSelected: {AvailableInstalls[1].IsSelected}" +
                Environment.NewLine +
                $"Installable 2 DisplayName: {AvailableInstalls[2].DisplayName}, Installable 2 IsSelected: {AvailableInstalls[2].IsSelected}" +
                Environment.NewLine +
                $"Installable 3 DisplayName: {AvailableInstalls[3].DisplayName}, Installable 3 IsSelected: {AvailableInstalls[3].IsSelected}";
        }
        
        return returnString;
    }
}