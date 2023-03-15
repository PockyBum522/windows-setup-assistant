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
public class RegistryFileAsOptionLoader
{
    private readonly ILogger _logger;
    private readonly MainWindowPersistentState _mainWindowPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="mainWindowPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public RegistryFileAsOptionLoader(
        ILogger logger,
        MainWindowPersistentState mainWindowPersistentState)
    {
        _logger = logger;
        _mainWindowPersistentState = mainWindowPersistentState;
    }
    
    /// <summary>
    /// Takes a path to a .reg file, and turns it into an option in the settings section matching its parent folder name
    /// If no matching section name is found, a new section is made with that name
    /// </summary>
    /// <param name="fullPathToRegistryFile">Full path to the .reg file to add as a selectable option</param>
    public void AddRegistryFileAsOption(string fullPathToRegistryFile)
    {
        var parentFolderName =
            Path.GetFileName(
            Path.GetDirectoryName(fullPathToRegistryFile));
        
        // Make option

        var displayName = Path.GetFileNameWithoutExtension(fullPathToRegistryFile);

        var importArguments = $"import \"{fullPathToRegistryFile}\"";
        
        var newOption = new OptionRegistryFile()
        {
            DisplayName = displayName,
            ExecuteSetting = () =>
            {
                var processStartInfo = new ProcessStartInfo()
                {
                    Verb = "runas",
                    FileName = "reg",
                    Arguments = importArguments,
                    UseShellExecute = true
                };

                _logger.Information("About to import reg file: {FilePath} with arguments: reg {Arguments}", fullPathToRegistryFile, importArguments);
                
                var proc = Process.Start(processStartInfo);

                proc?.WaitForExit();
            }
        };
        
        var foundSection = false;
        
        foreach (var section in _mainWindowPersistentState.SettingsSections)
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
        
        _mainWindowPersistentState.SettingsSections.Add(newSection);
    }
}