using WindowsSetupAssistant.Interfaces;

namespace WindowsSetupAssistant.Models.ISelectables.Installers;

/// <summary>
/// Represents an application that will be installed through Chocolatey windows package manager
/// </summary>
public class ChocolateyInstaller : ISelectable
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    /// <summary>
    /// The ID of the package at https://community.chocolatey.org/packages (basically whatever would come after choco install)
    /// </summary>
    public string ChocolateyId { get; set; } = "";
    
    /// <summary>
    /// Any arguments chocolatey needs to install the package. These are optional and generally not needed
    /// </summary>
    public string Arguments { get; set; } = "";
    
    /// <inheritdoc />
    public bool IsSelected { get; set; }
}