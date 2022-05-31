using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Models.ConfigurationActions;

public class TextInputConfigurationAction : IConfigurationAction
{
    public string Description { get; set; } = "";
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public string UserInput { get; set; } = "";

    public Action Execute { get; set; }
}