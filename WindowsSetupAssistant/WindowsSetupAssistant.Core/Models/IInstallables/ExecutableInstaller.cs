using System.Diagnostics;
using Serilog;
using WindowsSetupAssistant.Core.Logic;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains the information needed to install an executable install file
/// </summary>
public class ExecutableInstaller : BaseInstaller
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public ExecutableInstaller(ILogger logger)
    {
        _logger = logger;
    }
    
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
    public override void ExecuteInstall()
    {
        var executableInstallerPath =
            FileSearcher.ReverseWalkDirectoriesFind(ApplicationPaths.ThisApplicationRunFromDirectoryPath, FileName, 8);

        var executableInstallerStartInfo = new ProcessStartInfo()
        {
            Arguments = "Arguments",
            CreateNoWindow = true,
            FileName = executableInstallerPath
        };

        Process.Start(executableInstallerStartInfo);
    }
}