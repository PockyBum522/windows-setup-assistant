using WindowsSetupAssistant.Interfaces;

namespace WindowsSetupAssistant.Models.IInstallables;

/// <summary>
/// Base installer class from which all Installers inherit
/// </summary>
public class BaseInstaller : ISelectable
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
    /// Friendlier readout for debugging
    /// </summary>
    /// <returns>String with name and if is selected</returns>
    public override string ToString()
    {
        return $"Install Name: {DisplayName} - IsSelected: {IsSelected}";
    }
}