using Serilog;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains data for an archive in the \Resources\Portable Applications\ folder
/// Mostly what to show for the display name and where to install it if the user selects it
/// </summary>
public class ArchiveInstaller : BaseInstaller
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public ArchiveInstaller(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Path to the folder containing the portable application
    /// </summary>
    public string ArchiveFilename { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";

    /// <inheritdoc/>
    public override void ExecuteInstall()
    {
        
    }
}