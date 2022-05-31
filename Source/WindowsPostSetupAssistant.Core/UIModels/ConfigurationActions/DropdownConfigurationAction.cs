using System.Windows.Input;
using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Models.ConfigurationActions;

public class DropdownConfigurationAction : IConfigurationAction
{
    public string Description { get; set; } = "";
    
    public bool MarkedAsOptional { get; set; }
    public bool Enabled { get; set; }
    
    public List<string> DropdownOptions { get; set; } = new();
    
    public Action Execute { get; set; }
}
