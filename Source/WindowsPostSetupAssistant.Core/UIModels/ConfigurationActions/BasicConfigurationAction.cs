using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.UIModels.ConfigurationActions;

public class BasicConfigurationAction : IConfigurationAction
{
    public BasicConfigurationAction
    (
        string description, 
        Guid associatedModuleGuid,
        object? argumentsForModule = null
    )
    {
        Description = description;
        AssociatedModuleGuid = associatedModuleGuid;
        ArgumentsForModule = argumentsForModule;
    }
    
    public string Description { get; }
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public object? ArgumentsForModule { get; set; }

    public Guid AssociatedModuleGuid { get; }
}