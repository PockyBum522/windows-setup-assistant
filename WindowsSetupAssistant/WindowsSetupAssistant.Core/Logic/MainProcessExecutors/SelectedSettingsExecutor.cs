using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainProcessExecutors;

/// <summary>
/// Handles executing any settings the user selected during the main setup process execution
/// </summary>
public class SelectedSettingsExecutor : ObservableObject
{
    private readonly ILogger _logger;
    private readonly SessionPersistentState _sessionPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public SelectedSettingsExecutor(
        ILogger logger,
        SessionPersistentState sessionPersistentState)
    { 
        _logger = logger;
        _sessionPersistentState = sessionPersistentState;
    }
    
    
    /// <summary>
    /// Handles executing any settings the user selected during the main setup process execution
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void ExecuteSelectedSettingsInAllSections()
    {
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (!setting.IsSelected) continue;
                if (setting is OptionPowerShellScript) continue;    // We'll work these later because they have interactive and noninteractive possibilities
                
                // Otherwise:
                _logger.Debug("Executing setting: {Name}", setting.DisplayName);

                if (setting is OptionInternalMethod internalMethodSetting)
                {
                    var actualInternalMethodFromDeserializedName = GetInternalMethodSettingByName(internalMethodSetting.DisplayName);
                    
                    if (actualInternalMethodFromDeserializedName is null) throw new NullReferenceException();
                    if (actualInternalMethodFromDeserializedName.ExecuteSetting is null) throw new NullReferenceException();
                    
                    actualInternalMethodFromDeserializedName.ExecuteSetting.Invoke();
                }
                
                if (setting is OptionRegistryFile registryFileSetting)
                {
                    if (registryFileSetting is null) throw new NullReferenceException();

                    ImportRegistryFile(registryFileSetting);
                }
            }
        }
    }

    private OptionInternalMethod GetInternalMethodSettingByName(string displayName)
    {
        foreach (var settingsSection in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in settingsSection.Settings)
            {
                if (setting.DisplayName == displayName)
                    return (OptionInternalMethod)setting;
            }   
        }

        // This should never happen
        throw new NotImplementedException();
    }

    /// <summary>
    /// Filters for powershell scripts that are interactive and invokes them
    /// </summary>
    public void WorkAllInteractivePowershellScripts()
    {
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (!setting.IsSelected) continue;
                var script = setting as OptionPowerShellScript;
                if (script == null) continue;
                if (!script.IsInteractive) continue;

                RunPowershellScript(script);
            }
        }
    }

    /// <summary>
    /// Filters for powershell scripts that are NOT interactive and invokes them
    /// </summary>
    public void WorkAllNonInteractivePowershellScripts()
    {
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (!setting.IsSelected) continue;
                var script = setting as OptionPowerShellScript;
                if (script == null) continue;
                if (script.IsInteractive) continue;

                RunPowershellScript(script);
            }
        }
    }
    
    private void ImportRegistryFile(OptionRegistryFile registryFileSetting)
    {
        var importArguments = $"import \"{registryFileSetting.FilePathToReg}\"";
        
        var processStartInfo = new ProcessStartInfo()
        {
            Verb = "runas",
            FileName = "reg",
            Arguments = importArguments,
            UseShellExecute = true
        };

        _logger.Information(
            "About to import reg file: {FilePath} with arguments: reg {Arguments}", 
            registryFileSetting.FilePathToReg, importArguments);
            
        var process = Process.Start(processStartInfo);

        if (process is null) throw new NullReferenceException();

        process.WaitForExit();
    }

    private void RunPowershellScript(OptionPowerShellScript scriptToRun)
    {
        // Otherwise:
        _logger.Debug("Executing powershell script: {Name}", scriptToRun.DisplayName);
    
        var command = $"-ExecutionPolicy Unrestricted -File \"{scriptToRun.FilePathToScript}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "pwsh",
            Arguments = command,
            UseShellExecute = true, // Needed for running as admin
            Verb = "runas", // Run as administrator
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;

        _logger.Information(
            "About to run .ps1 file: pwsh {Arguments}",
            scriptToRun.FilePathToScript, processStartInfo.Arguments);

        var proc = Process.Start(processStartInfo);

        process.Start();
        process.WaitForExit();
    }
}