namespace WindowsPostSetupAssistant.Core.Interfaces;

public interface IModule
{
    public string Arguments { get; set; }
    
    public bool ValidateArguments { get; }
    
    public Action Execute { get; }
}