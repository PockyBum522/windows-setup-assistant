namespace WindowsPostSetupAssistant.Core.Interfaces;

public interface IModule
{
    public Guid ModuleGuid { get; }

    public object Arguments { get; set; }
    
    public bool ValidateArguments { get; }
    
    public Action Execute { get; }
}