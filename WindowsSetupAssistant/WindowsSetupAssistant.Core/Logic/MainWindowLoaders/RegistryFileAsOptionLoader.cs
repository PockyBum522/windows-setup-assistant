using System.Diagnostics;
using System.IO;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowHelpers;

/// <summary>
/// Takes a path to a .reg file, and turns it into an option in the settings section matching its parent folder name
/// If no matching section name is found, a new section is made with that name
/// </summary>
public class RegistryFileAsOptionLoader
{
    /// <summary>
    /// Takes a path to a .reg file, and turns it into an option in the settings section matching its parent folder name
    /// If no matching section name is found, a new section is made with that name
    /// </summary>
    /// <param name="fullPathToRegistryFile">Full path to the .reg file to add as a selectable option</param>
    /// <param name="currentState">CurrentState to add the new option (and possibly new section) to</param>
    public void AddRegistryFileAsOption(string fullPathToRegistryFile, CurrentState currentState)
    {
        var parentFolderName =
            Path.GetFileName(
            Path.GetDirectoryName(fullPathToRegistryFile));
        
        // Make option

        var displayName = Path.GetFileNameWithoutExtension(fullPathToRegistryFile);
        
        var newOption = new OptionRegistryFile()
        {
            DisplayName = displayName,
            ExecuteSetting = () =>
            {
                var processStartInfo = new ProcessStartInfo()
                {
                    Verb = "runas",
                    FileName = "reg",
                    Arguments = $"import \"{fullPathToRegistryFile}\"",
                    UseShellExecute = true
                };

                var proc = Process.Start(processStartInfo);

                proc?.WaitForExit();
            }
        };
        
        var foundSection = false;
        
        foreach (var section in currentState.MainWindowPartialViewModel.SettingsSections)
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
        
        currentState.MainWindowPartialViewModel.SettingsSections.Add(newSection);
    }
}