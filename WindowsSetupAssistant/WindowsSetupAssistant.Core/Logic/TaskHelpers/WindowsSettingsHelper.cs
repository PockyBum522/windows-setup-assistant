using System;
using System.Diagnostics;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

public class WindowsSettingsHelper
{
    private readonly ILogger _logger;

    public WindowsSettingsHelper(ILogger logger)
    {
        _logger = logger;
    }
    
    public void DisableSleepWhenPoweredByAc()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -monitor-timeout-ac 20",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -monitor-timeout-dc 5",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -disk-timeout-ac 0",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -standby-timeout-ac 0",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -hibernate-timeout-ac 0",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
    }
   
    public void DisableNetworkThumbnails()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Explorer", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("DisableThumbsDBOnNetworkFolders", 1);
    }
}