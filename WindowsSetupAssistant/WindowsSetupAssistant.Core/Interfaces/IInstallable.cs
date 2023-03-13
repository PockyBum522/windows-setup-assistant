namespace WindowsSetupAssistant.Core.Interfaces;

public class IInstallable
{
    /// <summary>
    /// The name to display to the user next to a checkbox in this application
    /// </summary>
    public string DisplayName { get; set; } = "";
    
    /// <summary>
    /// Should we install this when the user clicks execute all installs
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// Runs the actual installation
    /// </summary>
    public void ExecuteInstall() { }
}