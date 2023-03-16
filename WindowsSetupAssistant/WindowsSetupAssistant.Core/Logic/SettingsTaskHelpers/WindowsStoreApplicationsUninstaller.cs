using System.Diagnostics;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Uninstallers for Windows Store Applications
/// </summary>
public class WindowsStoreApplicationsUninstaller
{
    /// <summary>
    /// Uninstalls the Windows Store version of Spotify 
    /// </summary>
    public string UninstallApplication(string applicationName)
    {
        var process = new Process();
        process.StartInfo.FileName = "powershell.exe";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        // PowerShell command to remove the Spotify app
        process.StartInfo.Arguments = $"Get-AppxPackage -Name *{applicationName}* | Remove-AppxPackage";
        
        // Start the process and wait for it to finish
        process.Start();
        
        var output = process.StandardOutput.ReadToEnd();
        var errors = process.StandardError.ReadToEnd();
        
        process.WaitForExit();

        return errors;
    }
}