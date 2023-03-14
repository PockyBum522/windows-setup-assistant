﻿using System;
using Newtonsoft.Json;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

/// <summary>
/// Represents a registry file on the filesystem to give the user the option to merge to registry
/// </summary>
public class OptionRegistryFile : ISelectableSetting
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
    [JsonIgnore] public Action? ExecuteSetting { get; set; }
    
}