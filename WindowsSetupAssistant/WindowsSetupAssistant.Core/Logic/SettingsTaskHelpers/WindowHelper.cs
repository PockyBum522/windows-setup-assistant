using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Methods for changing Windows UI-specific settings
/// </summary>
public class WindowHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public WindowHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Changes Windows theme, title bars, and accent color to dark
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void ChangeWindowsThemeToDark()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("AppsUseLightTheme", 0);
        key.SetValue("SystemUsesLightTheme", 0);

        using var accentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);

        if (accentKey == null) throw new NullReferenceException();
        
        var paletteData = new byte[] { 0x9B, 0x9A, 0x99, 0x00, 0x84, 0x83, 0x81, 0x00, 0x6D, 0x6B, 0x6A, 0x00, 0x4C, 0x4A, 0x48, 0x00, 0x36, 0x35, 0x33, 0x00, 0x26, 0x25, 0x24, 0x00, 0x19, 0x19, 0x19, 0x00, 0x10, 0x7C, 0x10, 0x00  };
        
        accentKey.SetValue("AccentColorMenu", 4282927692);
        accentKey.SetValue("AccentPalette", paletteData);
        accentKey.SetValue("StartColorMenu", 4281546038);
        
        using var dwmKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\DWM", true);
  
        if (dwmKey == null) throw new NullReferenceException();
        
        dwmKey.SetValue("AccentColor", 4282927692);        
        dwmKey.SetValue("ColorizationAfterglow", 3293334088);
        dwmKey.SetValue("ColorizationColor", 3293334088);
    }
    
    /// <summary>
    /// Changes Windows to disabled transparency
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void DisableWindowTransparency()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("EnableTransparency", 0);
    }

    /// <summary>
    /// Sets active and inactive window title bars to dark colors
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void BlackActiveAndInactiveTitleBars()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var accentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);
        
        if (accentKey == null) throw new NullReferenceException();

        // We have to do some really dumb stuff to get the number into a format SetValue will accept for a DWord
        accentKey.SetValue("StartColorMenu", BitConverter.ToInt32(BitConverter.GetBytes(0xff333536u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentColorMenu", BitConverter.ToInt32(BitConverter.GetBytes(0xff484a4cu), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentPalette", new byte[]{ 0x9B, 0x9A, 0x99, 0x00, 0x84, 0x83, 0x81, 0x00, 0x6D, 0x6B, 0x6A, 0x00, 0x4C, 0x4A, 0x48, 0x00, 0x36, 0x35, 0x33, 0x00, 0x26, 0x25, 0x24, 0x00, 0x19, 0x19, 0x19, 0x00, 0x10, 0x7C, 0x10, 0x00 });

       
        using var dwmKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\DWM", true);
        
        if (dwmKey == null) throw new NullReferenceException();

        accentKey.SetValue("ColorizationAfterglow", BitConverter.ToInt32(BitConverter.GetBytes(0xc44c4a48u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("ColorizationColor", BitConverter.ToInt32(BitConverter.GetBytes(0xc44c4a48u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentColor", BitConverter.ToInt32(BitConverter.GetBytes(0xff484a4cu), 0), RegistryValueKind.DWord);
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
    
    /// <summary>
    /// Deletes files on the desktop that match each pattern in patternsToDeleteWithoutWildcards
    /// </summary>
    public void CleanDesktopOfAllFilesMatching(string[] patternsToDeleteWithoutWildcards)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var publicDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

        var filesOnDesktops = 
            Directory.GetFiles(publicDesktopPath).Concat(
            Directory.GetFiles(desktopPath)); 
        
        foreach (var file in filesOnDesktops)
        {
            foreach (var pattern in patternsToDeleteWithoutWildcards)
            {
                if (file.EndsWith(pattern)) File.Delete(file);    
            }
        }
    }
}