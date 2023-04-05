using System;
using System.Diagnostics;
using System.IO;
using Serilog;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders;

/// <summary>
/// Takes a path to a .reg file, and turns it into an option in the settings section matching its parent folder name
/// If no matching section name is found, a new section is made with that name
/// </summary>
public class PowershellScriptAsOptionLoader
{
    private readonly ILogger _logger;
    private readonly SessionPersistentState _sessionPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public PowershellScriptAsOptionLoader(
        ILogger logger,
        SessionPersistentState sessionPersistentState)
    {
        _logger = logger;
        _sessionPersistentState = sessionPersistentState;
    }
    
    /// <summary>
    /// Takes a path to a .ps1 file, and turns it into an option in the settings section matching its parent folder name
    /// If no matching section name is found, a new section is made with that name
    /// </summary>
    /// <param name="fullPathToScript">Full path to the .reg file to add as a selectable option</param>
    public void AddPowershellScriptAsOption(string fullPathToScript)
    {
        var parentFolderName =
            Path.GetFileName(
            Path.GetDirectoryName(fullPathToScript));
        
        // Make option

        var displayName = Path.GetFileNameWithoutExtension(fullPathToScript);
        
        if (string.IsNullOrEmpty(fullPathToScript))
        {
            throw new ArgumentException("Script path cannot be null or empty.");
        }

        var newOption = new OptionPowerShellScript
        {
            FilePathToScript = fullPathToScript,
            DisplayName = displayName,
            // Save all powershell scripts as interactive so they run last just in case until we come up with a better  way to configure them. 
            IsInteractive = true
        };

        var foundSection = false;
        
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            if (section.DisplayName != parentFolderName) continue;
            
            // Otherwise:
            foundSection = true;
            section.Settings.Add(newOption);
        }

        if (foundSection) return;
        
        // Otherwise:
        var newSection = new SettingsSection()
        {
            DisplayName = parentFolderName ?? ""
        };
            
        newSection.Settings.Add(newOption);
        
        _sessionPersistentState.SettingsSections.Add(newSection);
    }
}