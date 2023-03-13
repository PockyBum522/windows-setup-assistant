using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.ISelectables;

/// <summary>
/// Represents a registry file on the filesystem to give the user the option to merge to registry
/// </summary>
public class OptionRegistryFile : ISelectable
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";
    
    /// <inheritdoc />
    public bool IsSelected { get; set; }
}