using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.UIModels.ConfigurationActions;

public class TextInputConfigurationAction : IConfigurationAction
{
    public TextInputConfigurationAction    
    (
        string description, 
        Guid associatedModuleGuid
    )
    {
        Description = description;
        AssociatedModuleGuid = associatedModuleGuid;
    }

    public string Description { get; }

    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public string UserInput { get; set; } = "";

    public Guid AssociatedModuleGuid { get; }
}