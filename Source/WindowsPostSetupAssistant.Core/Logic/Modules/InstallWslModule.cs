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
                          $"Arguments for {nameof(GetType)} were not able to be verified by " +
                          $"{nameof(CheckArguments)} full arguments are: {Arguments}{Environment.NewLine}" +
                          Environment.NewLine;
            
            _logger.Error("Arguments for {ThisType} were not able to be verified, full arguments " +
                          "are: {EnvironmentArguments}", 
                nameof(GetType),
                          (string)Arguments);
            
            throw new ArgumentException(message);
        }

        if (Arguments.Equals(DistrosEnum.Ubuntu.ToString()))
        {
            // Otherwise, if arguments are checked:
            var installChocolateyAppProcess = new Process();

            installChocolateyAppProcess.StartInfo.FileName = "choco";
            installChocolateyAppProcess.StartInfo.Arguments = $"upgrade wsl2";
            installChocolateyAppProcess.StartInfo.Verb = "runas";
            installChocolateyAppProcess.StartInfo.UseShellExecute = true;

            _logger.Information("In module: {ThisType}", nameof(GetType));
            _logger.Information("About to run: choco upgrade wsl2");
        
            installChocolateyAppProcess.Start();
            installChocolateyAppProcess.WaitForExit();
        }
        
        if (Arguments.Equals(DistrosEnum.Debian.ToString()))
        {
            // Otherwise, if arguments are checked:
            var installChocolateyAppProcess = new Process();

            installChocolateyAppProcess.StartInfo.FileName = "choco";
            installChocolateyAppProcess.StartInfo.Arguments = $"upgrade wsl-debiangnulinux --ignore-checksums --force";
            installChocolateyAppProcess.StartInfo.Verb = "runas";
            installChocolateyAppProcess.StartInfo.UseShellExecute = true;

            _logger.Information("In module: {ThisType}", nameof(GetType));
            _logger.Information(
                "About to run: choco upgrade wsl-debiangnulinux --ignore-checksums --force");
        
            installChocolateyAppProcess.Start();
            installChocolateyAppProcess.WaitForExit();
        }
    }
    
    private bool CheckArguments()
    {
        if (Arguments.Equals(DistrosEnum.Uninitialized.ToString())) return false;
        
        return true;
    }
}

public enum DistrosEnum
{
    Uninitialized,
    Ubuntu,
    Debian
}