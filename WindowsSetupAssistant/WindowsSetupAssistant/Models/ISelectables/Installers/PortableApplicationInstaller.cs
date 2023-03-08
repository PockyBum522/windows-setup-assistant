using WindowsSetupAssistant.Interfaces;

namespace WindowsSetupAssistant.Models.ISelectables.Installers;

public class PortableApplicationInstaller : ISelectable
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    /// <summary>
    /// Name of the folder containing the portable application, must be located in \Resources\Portable Applications\
    /// </summary>
    public string FolderName { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";
    
    /// <inheritdoc />
    public string DesktopShortcutExePath { get; set; } = "";
    
    /// <inheritdoc />
    public string StartMenuShortcutExePath { get; set; } = "";
    
    /// <inheritdoc />
    public bool IsSelected { get; set; }
}