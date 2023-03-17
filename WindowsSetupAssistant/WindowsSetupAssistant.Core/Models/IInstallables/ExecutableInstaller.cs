﻿using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains the information needed to install an executable install file
/// </summary>
public partial class ExecutableInstaller : ObservableObject, IInstallable
{
    /// <summary>
    /// Path to the .exe or .msi
    /// </summary>
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// Optional arguments to run the .exe or .msi with
    /// </summary>
    public string Arguments { get; set; } = "";
    
    /// <summary>
    /// AutoHotkey V2 macro script to click through the installer
    /// </summary>
    public string AutoHotkeyMacro { get; set; } = "";
    
    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";
    
    [ObservableProperty] private bool _isSelected;
    
    /// <inheritdoc/>
    public void ExecuteInstall(ILogger logger)
    {
        var executableInstallerPath =
            FileSearcher.ReverseWalkDirectoriesFind(ApplicationPaths.ThisApplicationRunFromDirectoryPath, FileName, 8);
        
        logger.Information("Installing: {Path}", executableInstallerPath);

        var executableInstallerStartInfo = new ProcessStartInfo()
        {
            Arguments = "Arguments",
            CreateNoWindow = true,
            FileName = executableInstallerPath
        };
        
        Task.Run(() => Process.Start(executableInstallerStartInfo));
    }
}