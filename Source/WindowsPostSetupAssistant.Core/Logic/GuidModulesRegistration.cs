using WindowsPostSetupAssistant.Core.Interfaces;
using WindowsPostSetupAssistant.Core.Logic.Modules;

namespace WindowsPostSetupAssistant.Core.Logic;

public class GuidModulesRegistration
{
    private readonly ILogger _logger;

    public GuidModulesRegistration(ILogger logger)
    {
        _logger = logger;
    }
    
    public IModule GetAssociatedModule(Guid guidToLookup)
    {
        if (guidToLookup == new InstallWslModule(_logger).ModuleGuid)
        {
            return new InstallWslModule(_logger);
        }
        
        if (guidToLookup == new InstallChocolateyApplication(_logger).ModuleGuid)
        {
            return new InstallChocolateyApplication(_logger);
        }
        
        _logger.Error("Could not locate module associated with GUID: {GuidToLookup}", guidToLookup);
        
        throw new ArgumentException($"Could not locate module associated with GUID: {guidToLookup}");
    }
}