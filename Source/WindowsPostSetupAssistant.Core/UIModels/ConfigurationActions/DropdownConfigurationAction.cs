using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.UIModels.ConfigurationActions;

public class DropdownConfigurationAction : IConfigurationAction
{
    public DropdownConfigurationAction
    (
        string description, 
        List<string> dropdownOptions,
        Guid associatedModuleGuid,
        object? argumentsForModule = null
    )
    {
        Description = description;
        DropdownOptions = dropdownOptions;
        AssociatedModuleGuid = associatedModuleGuid;
        ArgumentsForModule = argumentsForModule;
    }
    
    public string Description { get; set; }
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public List<string> DropdownOptions { get; }

    public object? ArgumentsForModule { get; set; }
    
    public Guid AssociatedModuleGuid { get; }
}
