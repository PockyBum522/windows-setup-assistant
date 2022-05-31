using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Models.ConfigurationActions;

public class ApplicationInstallConfigurationAction : IConfigurationAction
{
    public string Description { get; set; } = "";
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }
    
    public string ChocoInstallerArguments { get; set; } = "";
    public string ChocoInstallerParameters { get; set; } = "";

    public Action Execute { get; set; }
}