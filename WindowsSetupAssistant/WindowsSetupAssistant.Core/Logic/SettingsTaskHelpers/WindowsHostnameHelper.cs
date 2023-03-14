using System;
using System.Management;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Methods for changing Windows and operating-system-related settings
/// </summary>
public class WindowsHostnameHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public WindowsHostnameHelper(ILogger logger)
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
        _logger.Information("Changing hostname to: {NewHostName}", newHostName);
        
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
}