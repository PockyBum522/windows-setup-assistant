using System;
using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Logic;

public class ProcessElevator
{
    private readonly ILogger _logger;

    public ProcessElevator(ILogger logger)
    {
        _logger = logger;
    }
    
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