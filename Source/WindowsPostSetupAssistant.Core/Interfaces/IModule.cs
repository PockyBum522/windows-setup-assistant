using WindowsPostSetupAssistant.Core.Logic.Modules;

namespace WindowsPostSetupAssistant.Core.Interfaces;

public interface IModule
{
    public object Arguments { get; set; }
    
    public bool ValidateArguments { get; }
    
    public Action Execute { get; }
}