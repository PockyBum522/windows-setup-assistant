using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices.JavaScript;
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

    /// <summary>
    /// Sets the computer's hostname in the registry (Requires a reboot to take effect)
    /// </summary>
    /// <param name="newHostName">The new host name</param>
    /// <exception cref="Exception">If new name cannot be set</exception>
    public void ChangeHostName(string newHostName)
    {
        const string registryComputerNameKey = @"SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName";
    
        var compPath= "Win32_ComputerSystem.Name='" + System.Environment.MachineName + "'";
        
        using (var mo = new ManagementObject(new ManagementPath(compPath)))
        {
            var inputArgs = mo.GetMethodParameters("Rename");
            
            inputArgs["Name"] = newHostName;
            
            var output = mo.InvokeMethod("Rename", inputArgs, new InvokeMethodOptions());
            
            var retValue = (uint)Convert.ChangeType(output.Properties["ReturnValue"].Value, typeof(uint));
            
            if (retValue != 0)
            {
                throw new Exception("Computer could not be changed due to unknown reason.");
            }
        }

        var computerName = Registry.LocalMachine.OpenSubKey(registryComputerNameKey);
        
        if (computerName == null)
        {
            throw new Exception("Registry location '" + registryComputerNameKey + "' is not readable.");
        }

        var computerNameValue = ((string?)computerName.GetValue("ComputerName")) ?? "";
        
        if (computerNameValue != newHostName)
        {
            throw new Exception("The computer name was set by WMI but was not updated in the registry location: '" + registryComputerNameKey + "'");
        }
        
        computerName.Close();
        
        computerName.Dispose();
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