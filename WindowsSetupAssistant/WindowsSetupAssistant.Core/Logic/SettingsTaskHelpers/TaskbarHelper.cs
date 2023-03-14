using System;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Methods for changing Windows taskbar specific settings
/// </summary>
public class TaskbarHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public TaskbarHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Turns off news and interests on taskbar
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void DisableNewsAndInterestsOnTaskbar()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Feeds", true);

        key?.SetValue("ShellFeedsTaskbarViewMode", 2);
    }
    
    /// <summary>
    /// Collapses the search in taskbar to just an icon 
    /// </summary>
    /// <exception cref="NullReferenceException">Throws on registry access error</exception>
    public void CollapseSearchOnTaskbarToIcon()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 1);
    }
    
    /// <summary>
    /// Collapses the search in taskbar to completely hidden 
    /// </summary>
    /// <exception cref="NullReferenceException">Throws on registry access error</exception>
    public void CollapseSearchOnTaskbarToHidden()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 0);
    }
}