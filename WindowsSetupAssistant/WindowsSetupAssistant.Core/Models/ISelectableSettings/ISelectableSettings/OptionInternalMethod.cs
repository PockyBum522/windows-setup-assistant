using System;
using Newtonsoft.Json;
using WindowsSetupAssistant.Core.Models.Settings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

/// <summary>
/// Selectable item that when Executed will run internal code 
/// </summary>
public class OptionInternalMethod : ISelectableSetting
{
    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";
    
    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Action which will merge the .reg file when fired
    /// </summary>
    [JsonIgnore] public Action? ExecuteSetting { get; set; }
}