using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Logic.TaskHelpers;

public class ChocolateyHelper
{
    private readonly ILogger _logger;

    public ChocolateyHelper(ILogger logger)
    {
        _logger = logger;
    }
    
    public void ChocoInstall(string packageName, string installerArguments = "", string parameters = "")
    {
        _logger.Information("Installing {PackageName} with Chocolatey", packageName);

        var procInfo = new ProcessStartInfo();

        var argsString = $"upgrade {packageName}";

        if (!string.IsNullOrWhiteSpace(installerArguments))
        {
            argsString += $" --install-arguments='{installerArguments}'";
        }

        if (!string.IsNullOrWhiteSpace(parameters))
        {
            argsString += $" --params \"{parameters}\"";
        }

        procInfo.Arguments = argsString;

        procInfo.FileName = "choco";

        var proc = Process.Start(procInfo);

        proc?.WaitForExit();
    }
}