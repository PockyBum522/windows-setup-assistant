using System.Diagnostics;
using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Logic.Modules;

public class InstallWslModule : IModule
{
    private readonly ILogger _logger;

    public InstallWslModule(ILogger logger)
    {
        _logger = logger;
    }

    public Guid ModuleGuid => new Guid("6869A2A6-2A7D-4616-B59C-2DE31651A58D");
    
    public object Arguments { get; set; } = DistrosEnum.Uninitialized;

    public bool ValidateArguments => CheckArguments();
    
    public Action Execute => InstallWsl;

    private void InstallWsl()
    {
        if (!ValidateArguments)
        {
            var message = $"{Environment.NewLine}ERROR: {Environment.NewLine}" +
                          $"Arguments for InstallWslModule were not able to be verified by " +
                          $"{nameof(CheckArguments)} full arguments are: {Arguments}{Environment.NewLine}" +
                          Environment.NewLine;
            
            Console.WriteLine(message);
            
            throw new ArgumentException(message);
        }

        // Otherwise, if arguments are checked and safe:
        if (Arguments.Equals(DistrosEnum.Ubuntu))
        {
            var installWslProcess = Process.Start("choco upgrade wsl2");
            
            installWslProcess.WaitForExit();
        }
        
        if (Arguments.Equals(DistrosEnum.Debian))
        {
            var installDebianProcess = Process.Start("choco upgrade wsl-debiangnulinux --ignore-checksums --force");

            installDebianProcess.WaitForExit();
            
            
        }
    }
    
    private bool CheckArguments()
    {
        if (Arguments.Equals(DistrosEnum.Uninitialized)) return false;
        
        return true;
    }
}

public enum DistrosEnum
{
    Uninitialized,
    Ubuntu,
    Debian
}