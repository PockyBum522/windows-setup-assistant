using System;
using System.IO;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.Application;

/// <summary>
/// Model for storing current state, such as settings and installs that have been selected for the Windows Setup
/// </summary>
public class FinalCleanupHelper
{
    private readonly ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public FinalCleanupHelper(
        ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Used to clean up files once setup assistant is done running
    /// </summary>
    public void DeleteSavedStateFileOnDisk()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        File.Delete(ApplicationPaths.StatePath);

        // Delete script from startup
        var startupFolderPath = Environment.ExpandEnvironmentVariables("%AllUsersProfile%") + @"\Start Menu\Programs\Startup";
		        
        var startupBatPath = Path.Join(startupFolderPath, "Re-RunAdminBatFileAutomatically.bat");
		        
        File.Delete(startupBatPath);
    }
}