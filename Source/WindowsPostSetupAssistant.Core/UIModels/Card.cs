using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Models;

public class Card
{
    public string Title { get; set; } = "";
    
    public List<IConfigurationAction> CardActions { get; set; } = new();
}