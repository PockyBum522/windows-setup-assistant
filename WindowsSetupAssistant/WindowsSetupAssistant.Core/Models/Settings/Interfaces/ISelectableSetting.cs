using System;
using Newtonsoft.Json;

namespace WindowsSetupAssistant.Core.Models.Settings.Interfaces;

/// <summary>
/// Applied to any control that will show in the MainWindow and is a selectable option/installer
/// </summary>
public interface ISelectableSetting
{
    /// <summary>
    /// The name to display to the user next to a checkbox in this application
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Should we install this when the user clicks execute all installs
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Action which when executed will make the option change
    /// </summary>
    [JsonIgnore] public Action ExecuteSetting { get; set; }
}