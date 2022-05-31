namespace WindowsPostSetupAssistant.Core.Interfaces;

public interface IConfigurationAction
{
    public string Description { get; set; }
    
    bool MarkedAsOptional { get; set; }
    bool Enabled { get; set; }

    public Action Execute { get; set; }
}