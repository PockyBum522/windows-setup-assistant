using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Represents an application that will be installed through Chocolatey windows package manager
/// </summary>
public partial class ChocolateyInstaller : ObservableObject, IInstallable
{
    /// <summary>
    /// The ID of the package at https://community.chocolatey.org/packages (basically whatever would come after choco install)
    /// </summary>
    public string ChocolateyId { get; set; } = "";
    
    /// <summary>
    /// Any arguments chocolatey needs to install the package. These are optional and generally not needed
    /// </summary>
    public string Arguments { get; set; } = "";

    /// <summary>
    /// Any parameters chocolatey needs to install the package. These are optional and generally not needed
    /// </summary>
    public string Parameters { get; set; } = "";

    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";
    
    [ObservableProperty] private bool _isSelected;

    /// <inheritdoc/>
    public void ExecuteInstall(ILogger logger)
    {
        logger.Information("Installing {PackageName} with Chocolatey", ChocolateyId);

        var argsString = $"upgrade {ChocolateyId}";

        if (!string.IsNullOrWhiteSpace(Arguments))
        {
            argsString += $" --install-arguments='{Arguments}'";
        }

        if (!string.IsNullOrWhiteSpace(Parameters))
        {
            argsString += $" --params '{Parameters}'";
        }
        
        var installProcess = new Process();
        
        installProcess.StartInfo.FileName = "cmd.exe";
        installProcess.StartInfo.Arguments = $"/c choco {argsString}"; // Note the /c command, this is important for waitforexit
        installProcess.StartInfo.UseShellExecute = false;
        installProcess.StartInfo.RedirectStandardOutput = true;
        installProcess.StartInfo.RedirectStandardError = true;
        
        installProcess.Start();
        
        var stdOutput = installProcess.StandardOutput.ReadToEnd();
        
        var errorOutput = installProcess.StandardError.ReadToEnd();
        
        installProcess.WaitForExit();
        
        logger.Information("Chocolatey installer standard output: {StdOutput}", stdOutput);
        
        if (!string.IsNullOrWhiteSpace(errorOutput))
            logger.Warning("Chocolatey installer ERROR output: {ErrorOutput}", errorOutput);
    }
}