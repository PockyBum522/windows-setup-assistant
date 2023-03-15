using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

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
    /// Updates windows through Windows update then reboots 
    /// </summary>
    public void UpdateWindowsAndReboot()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        // This set of commands installs anything that isn't the Windows 11 upgrade
        var updateCommand = """
            Install-Module PSWindowsUpdate -Repository PSGallery -Force
            
            Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted

            Import-Module PSWindowsUpdate

            Add-WUServiceManager -ServiceID "7971f918-a847-4430-9279-4a52d1efe18d" -AddServiceFlag 7 -Confirm:$false
                        
            Get-WindowsUpdate -Criteria 'IsInstalled=0 and DeploymentAction=*' -MicrosoftUpdate -NotTitle 'Windows 11' -Verbose -Install -AcceptAll -RecurseCycle 3 -AutoReboot
            
            Start-Sleep -Seconds 120

            Write-Host "Updated windows"

            shutdown /r /t 60

            Start-Sleep -Seconds 90

            shutdown /r /t 5
            """;
        
        Process.Start("pwsh.exe", $"-c {updateCommand}").WaitForExit();
    }
    
    /// <summary>
    /// Updates windows through Windows update
    /// </summary>
    public void UpdateWindows()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        // This set of commands installs anything that isn't the Windows 11 upgrade
        var updateCommand = """
            Install-Module PSWindowsUpdate -Repository PSGallery -Force
            
            Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted

            Import-Module PSWindowsUpdate

            Add-WUServiceManager -ServiceID "7971f918-a847-4430-9279-4a52d1efe18d" -AddServiceFlag 7 -Confirm:$false
                        
            Get-WindowsUpdate -Criteria 'IsInstalled=0 and DeploymentAction=*' -MicrosoftUpdate -NotTitle 'Windows 11' -Verbose -Install -AcceptAll -RecurseCycle 3 -AutoReboot
            
            Start-Sleep -Seconds 120

            Write-Host "Updated windows"
            """;
        
        Process.Start("pwsh.exe", $"-c {updateCommand}").WaitForExit();
    }
}


