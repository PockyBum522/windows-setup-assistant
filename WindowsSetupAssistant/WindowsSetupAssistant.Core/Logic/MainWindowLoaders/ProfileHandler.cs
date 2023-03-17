using System;
using System.IO;
using Microsoft.Win32;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.Interfaces;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders;

/// <summary>
/// Handles saving/loading profiles
/// </summary>
public class ProfileHandler
{
    private readonly StateHandler _stateHandler;
    private readonly SessionPersistentState _sessionPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="stateHandler">Injected</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public ProfileHandler(
        StateHandler stateHandler,
        SessionPersistentState sessionPersistentState)
    {
        _stateHandler = stateHandler;
        _sessionPersistentState = sessionPersistentState;
    }
    
    /// <summary>
    /// Prompts the user to browse to where they want to save a profile to on disk, then saves it
    /// </summary>
    public void PromptUserToBrowseAndSaveProfile()
    {
        var profilesDirectoryPath = Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Configuration",
                "Profiles")
            .Replace("/", @"\");

        Console.WriteLine(profilesDirectoryPath);

        var fileDialog = new SaveFileDialog()
            { Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

        if (fileDialog.ShowDialog() != true) return;

        var fullSelectedFilePath = fileDialog.FileName;

        _stateHandler.SaveCurrentStateAsJson(fullSelectedFilePath);
    }

    /// <summary>
    /// Prompts the user to browse for a profile JSON file then sets the current main persistant state to match its state
    /// </summary>
    public void PromptUserForProfileThenLoadIt()
    {
        var profilesDirectoryPath = Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Configuration",
                "Profiles")
            .Replace("/", @"\");

        Console.WriteLine(profilesDirectoryPath);

        var fileDialog = new OpenFileDialog()
            { Filter = "JSON Files | *.json", InitialDirectory = profilesDirectoryPath };

        if (fileDialog.ShowDialog() != true) return;

        var fullSelectedFilePath = fileDialog.FileName;

        var loadedProfileState = _stateHandler.GetStateFromJson(fullSelectedFilePath);

        foreach (var install in loadedProfileState.AvailableInstalls)
        {
            if (install.IsSelected)
                GetInstallInListByDisplayName(install.DisplayName).IsSelected = true;
        }
        
        foreach (var section in loadedProfileState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (setting.IsSelected)
                    GetSettingInListByDisplayName(setting.DisplayName).IsSelected = true;
            }
        }

        _sessionPersistentState.IsCheckedUpdateWindows = loadedProfileState.IsCheckedUpdateWindows;
        
        _sessionPersistentState.TextHostname = loadedProfileState.TextHostname;

        _sessionPersistentState.TextMonitorTimeoutOnAc = loadedProfileState.TextMonitorTimeoutOnAc;
        _sessionPersistentState.TextMonitorTimeoutOnBattery = loadedProfileState.TextMonitorTimeoutOnBattery;
        _sessionPersistentState.TextStandbyTimeoutOnAc = loadedProfileState.TextStandbyTimeoutOnAc;
        _sessionPersistentState.TextStandbyTimeoutOnBattery = loadedProfileState.TextStandbyTimeoutOnBattery;
        _sessionPersistentState.TextHibernateTimeoutOnAc = loadedProfileState.TextHibernateTimeoutOnAc;
        _sessionPersistentState.TextHibernateTimeoutOnBattery = loadedProfileState.TextHibernateTimeoutOnBattery;
    }
    
    private IInstallable GetInstallInListByDisplayName(string displayName)
    {
        foreach (var install in _sessionPersistentState.AvailableInstalls)
        {
            if (displayName == install.DisplayName)
                return install;
        }

        // Just return something it can't act on that will affect the main window
        return new ArchiveInstaller();
    }
    
    private ISelectableSetting GetSettingInListByDisplayName(string displayName)
    {
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (displayName == setting.DisplayName)
                    return setting;
            }
        }

        // Just return something it can't act on that will affect the main window
        return new OptionRegistryFile();
    }
}