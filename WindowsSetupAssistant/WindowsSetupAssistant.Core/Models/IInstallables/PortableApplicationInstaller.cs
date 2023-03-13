using Serilog;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Model for deserializing JSON representing a portable application folder to copy according to the settings
/// stored in the JSON for that particular installer
/// </summary>
public class PortableApplicationInstaller : BaseInstaller
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public PortableApplicationInstaller(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Name of the folder containing the portable application, must be located in \Resources\Portable Applications\
    /// </summary>
    public string FolderName { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";
    
    /// <summary>
    /// If user wants to create a desktop shortcut, what exe should that shortcut point to? Blank or whitespace
    /// if no shortcut desired
    /// </summary>
    public string DesktopShortcutExePath { get; set; } = "";
    
    /// <summary>
    /// If user wants to create a Start Menu shortcut, what exe should that shortcut point to? Blank or whitespace
    /// if no shortcut desired
    /// </summary>
    public string StartMenuShortcutExePath { get; set; } = "";
    
    /// <inheritdoc/>
    public override void ExecuteInstall()
    {
        
    }
}