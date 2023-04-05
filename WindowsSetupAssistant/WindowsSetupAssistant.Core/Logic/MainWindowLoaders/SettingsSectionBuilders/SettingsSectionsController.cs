using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using WindowsSetupAssistant.Core.Models;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders.SettingsSectionBuilders;

/// <summary>
/// Handles 
/// </summary>
public class SettingsSectionsController
{
    private readonly SessionPersistentState _sessionPersistentState;
    private readonly RegistryFileAsOptionLoader _registryFileAsOptionLoader;
    private readonly PowershellScriptAsOptionLoader _powershellScriptAsOptionLoader;
    private readonly TimeSettingsSectionBuilder _timeSettingsSectionBuilder;
    private readonly TaskbarSettingsSectionBuilder _taskbarSettingsSectionBuilder;
    private readonly ApplicationsSettingsSectionBuilder _applicationsSettingsSectionBuilder;
    private readonly DesktopSettingsSectionBuilder _desktopSettingsSectionBuilder;
    private readonly WindowSettingsSectionBuilder _windowSettingsSectionBuilder;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="registryFileAsOptionLoader">Loads registry files from disk and turns them into OptionRegistryFile(s)</param>
    /// <param name="powershellScriptAsOptionLoader">Powershell script option loader</param>
    /// <param name="timeSettingsSectionBuilder">Time settings section builder</param>
    /// <param name="taskbarSettingsSectionBuilder">Taskbar settings section builder</param>
    /// <param name="applicationsSettingsSectionBuilder">Applications settings section builder</param>
    /// <param name="desktopSettingsSectionBuilder">Desktop settings section builder</param>
    /// <param name="windowSettingsSectionBuilder">Window settings section builder</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public SettingsSectionsController(
        SessionPersistentState sessionPersistentState,
        RegistryFileAsOptionLoader registryFileAsOptionLoader,
        PowershellScriptAsOptionLoader powershellScriptAsOptionLoader,
        TimeSettingsSectionBuilder timeSettingsSectionBuilder,
        TaskbarSettingsSectionBuilder taskbarSettingsSectionBuilder,
        ApplicationsSettingsSectionBuilder applicationsSettingsSectionBuilder,
        DesktopSettingsSectionBuilder desktopSettingsSectionBuilder,
        WindowSettingsSectionBuilder windowSettingsSectionBuilder)
    { 
        _sessionPersistentState = sessionPersistentState;
        _registryFileAsOptionLoader = registryFileAsOptionLoader;
        _powershellScriptAsOptionLoader = powershellScriptAsOptionLoader;
        _timeSettingsSectionBuilder = timeSettingsSectionBuilder;
        _taskbarSettingsSectionBuilder = taskbarSettingsSectionBuilder;
        _applicationsSettingsSectionBuilder = applicationsSettingsSectionBuilder;
        _desktopSettingsSectionBuilder = desktopSettingsSectionBuilder;
        _windowSettingsSectionBuilder = windowSettingsSectionBuilder;
    }
    
    /// <summary>
    /// Sets up all the sections with any setting options that are executed with C# code internal to this application
    /// Then loads any .reg files in the Resources folder and merges them into these sections.
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void LoadAllSettingsSections()
    {
        var timeSection = _timeSettingsSectionBuilder.MakeSection();
        _sessionPersistentState.SettingsSections.Add(timeSection);

        var taskbarSection = _taskbarSettingsSectionBuilder.MakeSection();
        _sessionPersistentState.SettingsSections.Add(taskbarSection);
        
        var desktopSection = _desktopSettingsSectionBuilder.MakeSection();
        _sessionPersistentState.SettingsSections.Add(desktopSection);

        var windowSection = _windowSettingsSectionBuilder.MakeSection();
        _sessionPersistentState.SettingsSections.Add(windowSection);
        
        var applicationsSection = _applicationsSettingsSectionBuilder.MakeSection();
        _sessionPersistentState.SettingsSections.Add(applicationsSection);

        // Load registry files from disk
        var registryFilePaths = GetAllRegistryFilePathsFromResources();

        foreach (var filePath in registryFilePaths)
        {
            _registryFileAsOptionLoader.AddRegistryFileAsOption(filePath);
        }
        
        var powershellFilePaths = GetAllPowershellScriptFilePathsFromResources();

        foreach (var filePath in powershellFilePaths)
        {
            _powershellScriptAsOptionLoader.AddPowershellScriptAsOption(filePath);
        }
    }

    private List<string> GetAllPowershellScriptFilePathsFromResources()
    {
        var topLevelDirectories =
            Directory.GetDirectories(ApplicationPaths.ResourcePaths.ResourceDirectoryPowershellScripts);

        var returnPaths = new List<string>();
        
        foreach (var directoryPath in topLevelDirectories)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath, "*.ps1"))
            {
                returnPaths.Add(filePath);
            }
        }

        return returnPaths;
    }

    [SupportedOSPlatform("Windows7.0")]
    private List<string> GetAllRegistryFilePathsFromResources()
    {
        var topLevelDirectories =
            Directory.GetDirectories(ApplicationPaths.ResourcePaths.ResourceDirectoryRegistryFiles);

        var returnPaths = new List<string>();
        
        foreach (var directoryPath in topLevelDirectories)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath, "*.reg"))
            {
                returnPaths.Add(filePath);
            }
        }

        return returnPaths;
    }
}