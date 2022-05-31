using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Logic.Modules;

public class InstallWslModule : IModule
{
    private readonly ILogger _logger;

    public InstallWslModule(ILogger logger)
    {
        _logger = logger;

        if (string.IsNullOrEmpty(Arguments)) Arguments = "Ubuntu";
    }

    public string Arguments { get; set; } = "";

    public bool ValidateArguments => CheckArguments();
    
    public Action Execute => InstallWsl;

    private void InstallWsl()
    {
        if (!ValidateArguments)
            throw new ArgumentException("Arguments for InstallWslModule were not able to be verified by " +
                                        $"{nameof(CheckArguments)} full arguments are: {Arguments}");

        // Otherwise, if arguments are checked and safe:
        
    }
    
    private bool CheckArguments()
    {
        const string unsafeChars = @"""<>:/|?\*.{}=+!@#$%^&()";
        
        if (string.IsNullOrWhiteSpace(Arguments)) return false;

        foreach (var unsafeChar in unsafeChars)
        {
            if (Arguments.Contains(unsafeChar)) return false;
        }
        
        return true;
    }
}