using WindowsSetupAssistant.Interfaces;

namespace WindowsSetupAssistant.Models.ISelectables;

public class OptionRegistryFile : ISelectable
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    /// <inheritdoc />
    public string DesktopShortcutExePath { get; set; } = "";
    
    /// <inheritdoc />
    public string StartMenuShortcutExePath { get; set; } = "";
    
    /// <inheritdoc />
    public bool IsSelected { get; set; }
}