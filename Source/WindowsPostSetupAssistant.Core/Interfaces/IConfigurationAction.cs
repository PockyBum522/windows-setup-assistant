namespace WindowsPostSetupAssistant.Core.Interfaces;

public interface IConfigurationAction
{
    public string Description { get; }
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }

    public Guid AssociatedModuleGuid { get; }
}