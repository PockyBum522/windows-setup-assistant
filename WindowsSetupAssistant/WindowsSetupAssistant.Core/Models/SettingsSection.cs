using System.Collections.Generic;
using System.Collections.ObjectModel;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models;

/// <summary>
/// Section to hold settings to display to the user and let them select
/// </summary>
public class SettingsSection
{
    /// <summary>
    /// The settingsSection that is the parent of this one, if nested
    /// </summary>
    public SettingsSection? Parent { get; set; }

    /// <summary>
    /// The settingsSection(s) that are under of this one, if nested
    /// </summary>
    public List<SettingsSection> Children { get; set; } = new();
    
    /// <summary>
    /// Header for the section
    /// </summary>
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// The ISelectable settings to allow the user to select
    /// </summary>
    public ObservableCollection<ISelectable> Settings { get; set; } = new();
}