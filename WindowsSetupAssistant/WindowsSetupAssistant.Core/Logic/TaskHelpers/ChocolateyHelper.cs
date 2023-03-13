using System.Diagnostics;
using Serilog;
using WindowsSetupAssistant.Core.Models.IInstallables;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

/// <summary>
/// Installs a ChocolateyInstaller
/// </summary>
public class ChocolateyHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to user</param>
    public ChocolateyHelper(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Installs the passed ChocolateyInstaller
    /// </summary>
    /// <param name="installer"></param>
    public void ChocoInstall(ChocolateyInstaller installer)
    {
        _logger.Information("Installing {PackageName} with Chocolatey", installer.ChocolateyId);

        var procInfo = new ProcessStartInfo();

        var argsString = $"upgrade {installer.ChocolateyId}";

        if (!string.IsNullOrWhiteSpace(installer.Arguments))
        {
            argsString += $" --install-arguments='{installer.Arguments}'";
        }

        if (!string.IsNullOrWhiteSpace(installer.Parameters))
        {
            argsString += $" --params \"{installer.Parameters}\"";
        }

        procInfo.Arguments = argsString;

        procInfo.FileName = "choco";

        var proc = Process.Start(procInfo);

        proc?.WaitForExit();
    }
}