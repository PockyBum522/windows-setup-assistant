using System;
using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.Application;

/// <summary>
/// Allows for easy restarting of this process with admin privileges
/// </summary>
public class ProcessElevator
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public ProcessElevator(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Restarts current process with admin privileges
    /// </summary>
    public void ElevateThisProcessNow()
    {
        _logger.Information("Restarting new process as admin");
        
        var startInfo = new ProcessStartInfo
        {
            UseShellExecute = true, //<- for elevation
            Verb = "runas",  //<- for elevation
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = "SecondaryCSharpLoader.exe",
            Arguments = @"/RunningAsAdmin"
        };

        Process.Start(startInfo);
    }
}