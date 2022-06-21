using System.Diagnostics;
using WindowsPostSetupAssistant.Core.Interfaces;

namespace WindowsPostSetupAssistant.Core.Logic.Modules;

public class InstallChocolateyApplication : IModule
{
    private readonly ILogger _logger;

    public InstallChocolateyApplication(ILogger logger)
    {
        _logger = logger;
    }

    public Guid ModuleGuid => new Guid("62369EAC-8C58-43A0-82FF-7468B875F0D8");
    
    public object Arguments { get; set; } = "";

    public bool ValidateArguments => CheckArguments();
    
    public Action Execute => UpgradeChocolateyApplication;

    private void UpgradeChocolateyApplication()
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

        // Otherwise, if arguments are checked:
        var installChocolateyAppProcess = new Process();

        installChocolateyAppProcess.StartInfo.FileName = "choco";
        installChocolateyAppProcess.StartInfo.Arguments = $"upgrade {Arguments}";
        installChocolateyAppProcess.StartInfo.Verb = "runas";
        installChocolateyAppProcess.StartInfo.UseShellExecute = true;

        _logger.Information("In module: {ThisType}", nameof(GetType));
        _logger.Information("About to run: choco upgrade {Arguments}", (string)Arguments);
        
        installChocolateyAppProcess.Start();
        
        installChocolateyAppProcess.WaitForExit();
    }
    
    private bool CheckArguments()
    {
        if (string.IsNullOrWhiteSpace((string)Arguments)) return false;
        
        return true;
    }
}