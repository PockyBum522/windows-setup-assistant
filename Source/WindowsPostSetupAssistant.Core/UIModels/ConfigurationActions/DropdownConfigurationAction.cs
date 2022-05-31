using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.UIModels.ConfigurationActions;

public class DropdownConfigurationAction : IConfigurationAction
{
    public DropdownConfigurationAction
    (
        string description, 
        List<string> dropdownOptions,
        Guid associatedModuleGuid
    )
    {
        Description = description;
        DropdownOptions = dropdownOptions;
        AssociatedModuleGuid = associatedModuleGuid;
    }
    
    public string Description { get; set; }
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public List<string> DropdownOptions { get; }
    
    public Guid AssociatedModuleGuid { get; }
}
