using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Models.ConfigurationActions;

public class BasicConfigurationAction : IConfigurationAction
{
    public string Description { get; set; }
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }
    
    public void ExecuteAction()
    {
        throw new NotImplementedException();
    }
}