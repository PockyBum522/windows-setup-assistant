using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

/// <summary>
/// Helper for updating windows through windows update
/// </summary>
public class WindowsUpdater
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public WindowsUpdater(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Updates windows through Windows update
    /// </summary>
    public void UpdateWindows()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        // This set of commands installs anything that isn't the Windows 11 upgrade
        var updateCommand = @"
            Install-Module PSWindowsUpdate -Repository PSGallery -Force
            
            Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted

	        Import-Module PSWindowsUpdate

            Add-WUServiceManager -ServiceID ""7971f918-a847-4430-9279-4a52d1efe18d"" -AddServiceFlag 7 -Confirm:$false
            
            Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force
            
            Get-WindowsUpdate -Criteria 'IsInstalled=0 and DeploymentAction=*' -MicrosoftUpdate -NotTitle 'Windows 11' -Verbose -Install -AcceptAll -RecurseCycle 3 -AutoReboot
            
	        Start-Sleep -Seconds 60

            Write-Host ""Updated windows""
            ";
        
        Process.Start("pwsh.exe", $"-c {updateCommand}").WaitForExit();
    		    
        // TODO: Figure out how to make this not prompt when a reboot is needed
    }
}


