namespace WindowsSetupAssistant.Models.IInstallables;

public class ExecutableInstaller : BaseInstaller
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
}