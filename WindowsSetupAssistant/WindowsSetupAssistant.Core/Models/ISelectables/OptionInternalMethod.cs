using System;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ISelectables;

/// <summary>
/// Selectable item that when Executed will run internal code 
/// </summary>
public class OptionInternalMethod : ISelectable
{
    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";
    
    /// <inheritdoc/>
    public bool IsSelected { get; set; }
    
    /// <summary>
    /// Action which will run the method that sets the setting when fired
    /// </summary>
    public Action ExecuteSetting { get; set; } = new(() => { });
}