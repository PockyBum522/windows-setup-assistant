using System.Collections.Generic;
using WindowsSetupAssistant.Core.Models.Settings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.Settings;

/// <summary>
/// Section to hold settings to display to the user and let them select
/// </summary>
public class SettingsSection
{
    /// <summary>
    /// Header for the section
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// The ISelectable settings to allow the user to select
    /// </summary>
    public List<ISelectableSetting> Settings { get; set; } = new();
}