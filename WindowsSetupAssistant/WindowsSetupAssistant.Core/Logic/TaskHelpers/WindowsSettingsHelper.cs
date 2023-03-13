using System;
using System.Diagnostics;
using System.Management;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

/// <summary>
/// Methods for changing Windows and operating-system-related settings
/// </summary>
public class WindowsSettingsHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
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
    
        var compPath= "Win32_ComputerSystem.Name='" + Environment.MachineName + "'";
        
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

    /// <summary>
    /// Set monitor off, standby, and hibernate timeouts. If no parameters are specified, defaults to a very long
    /// timeout for everything on AC
    /// </summary>
    /// <param name="monitorOffAfterXMinutesOnAc">When on AC power, how long to wait (in minutes)to turn off the screen</param>
    /// <param name="monitorOffAfterXMinutesOnBattery">When on battery power, how long to wait (in minutes)to turn off the screen</param>
    /// <param name="standbyTimeOutMinutesOnAc">When on AC power, how long to wait (in minutes)to sleep the system</param>
    /// <param name="standbyTimeOutMinutesOnBattery">When on battery power, how long to wait (in minutes)to sleep the system</param>
    /// <param name="hibernateTimeOutMinutesOnAc">When on AC power, how long to wait (in minutes)to hibernate</param>
    public void SetPowerSettingsTo(
        int monitorOffAfterXMinutesOnAc = 250, 
        int monitorOffAfterXMinutesOnBattery = 5,
        
        int standbyTimeOutMinutesOnAc = 300,
        int standbyTimeOutMinutesOnBattery = 10,
        
        int hibernateTimeOutMinutesOnAc = 0)
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -monitor-timeout-ac {monitorOffAfterXMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -monitor-timeout-dc {monitorOffAfterXMinutesOnBattery}",
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
            Arguments = $"-x -standby-timeout-ac {standbyTimeOutMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
        
        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -standby-timeout-dc {standbyTimeOutMinutesOnBattery}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -hibernate-timeout-ac {hibernateTimeOutMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
    }
   
    /// <summary>
    /// Makes it so computer doesn't generate thumbnails on network locations, which prevents Thumbs.db files that
    /// annoyingly cannot be deleted or moved
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access problem</exception>
    public void DisableNetworkThumbnails()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Explorer", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("DisableThumbsDBOnNetworkFolders", 1);
    }
}