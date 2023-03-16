using System;
using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Methods for changing monitor settings
/// </summary>
public class DisplayHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public DisplayHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sets DPI scaling for each monitor on the system to 100%
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws on registry access error</exception>
    [SupportedOSPlatform("windows7.0")]
    public void SetDpiValueToZeroForAllMonitors()
    {
        // This key does not exist until you open display settings and change DPI. Great. Find a way to generate it.
        
        // using var baseKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\PerMonitorSettings", writable: true);
        //     
        // if (baseKey is null)
        // {
        //     throw new InvalidOperationException("The registry key could not be found.");
        // }
        //
        // foreach (var subKeyName in baseKey.GetSubKeyNames())
        // {
        //     using var monitorKey = baseKey.OpenSubKey(subKeyName, writable: true);
        //         
        //     if (monitorKey is null) throw new InvalidOperationException("The registry key could not be found.");
        //         
        //     monitorKey.SetValue("DpiValue", 0, RegistryValueKind.DWord);
        // }
    }
}