using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

/// <summary>
/// Represents a registry file on the filesystem to give the user the option to merge to registry
/// </summary>
public partial class OptionPowerShellScript : ObservableObject, ISelectableSetting
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    [ObservableProperty] private bool _isSelected;
    
    /// <summary>
    /// True if script needs user interaction, false if it can run unattended
    /// </summary>
    [ObservableProperty] private bool _isInteractive;

    /// <summary>
    /// Full path to the .ps1 file
    /// </summary>
    public string FilePathToScript { get; set; } = "";

    /// <summary>
    /// Action which will merge the .reg file when fired
    /// </summary>
    [JsonIgnore] public Action? ExecuteSetting { get; set; }
}