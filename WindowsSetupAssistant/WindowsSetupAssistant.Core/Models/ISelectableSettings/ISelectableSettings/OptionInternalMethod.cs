using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

/// <summary>
/// Selectable item that when Executed will run internal code 
/// </summary>
public partial class OptionInternalMethod : ObservableObject, ISelectableSetting
{
    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";

    [ObservableProperty] private bool _isSelected;

    /// <summary>
    /// Action which will merge the .reg file when fired
    /// </summary>
    [JsonIgnore] public Action? ExecuteSetting { get; set; }
}