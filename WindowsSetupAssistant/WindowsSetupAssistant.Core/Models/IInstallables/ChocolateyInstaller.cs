using System.Diagnostics;
using Serilog;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Represents an application that will be installed through Chocolatey windows package manager
/// </summary>
public class ChocolateyInstaller : IInstallable
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
    
    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <inheritdoc/>
    public void ExecuteInstall(ILogger logger)
    {
        logger.Information("Installing {PackageName} with Chocolatey", ChocolateyId);

        var procInfo = new ProcessStartInfo();

        var argsString = $"upgrade {ChocolateyId}";

        if (!string.IsNullOrWhiteSpace(Arguments))
        {
            argsString += $" --install-arguments='{Arguments}'";
        }

        if (!string.IsNullOrWhiteSpace(Parameters))
        {
            argsString += $" --params '{Parameters}'";
        }

        procInfo.Arguments = argsString;

        procInfo.FileName = "choco";

        var proc = Process.Start(procInfo);

        proc?.WaitForExit();   
    }
}