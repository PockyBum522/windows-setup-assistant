using System;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

/// <summary>
/// Represents a registry file on the filesystem to give the user the option to merge to registry
/// </summary>
public class SelectableSettingOptionRegistryFile : ISelectableSetting
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    /// <inheritdoc />
    public bool IsSelected { get; set; }

    /// <summary>
    /// Full path to the .reg file
    /// </summary>
    public string FilePathToReg { get; set; } = "";
    
    /// <summary>
    /// Action which will merge the .reg file when fired
    /// </summary>
    public Action ExecuteSetting { get; set; } = new(() => { });
}