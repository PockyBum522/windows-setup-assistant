using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;

/// <summary>
/// Methods for changing Windows desktop specific settings
/// </summary>
public class DesktopHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public DesktopHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Changes Windows theme, title bars, and accent color to dark and disables transparency
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void ChangeWindowsThemeToDark()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("AppsUseLightTheme", 0);
        key.SetValue("EnableTransparency", 0);
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
        
        //dwmKey.SetValue("StartColor", 4281546038);
        //dwmKey.SetValue("AccentColor", 4282927692);
    }

    /// <summary>
    /// Sets Windows wallpaper to the dark "Camping under the stars" wallpaper, which is less blinding than the default
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    public void SetWallpaperToDarkDefaultWallpaper()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        var tempFolder = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Temp");
        
        File.Copy(@"C:\Windows\Web\Wallpaper\Theme1\img13.jpg", @$"{tempFolder}\img13.jpg", true);
        
        using var fs = File.Open(@$"{tempFolder}\img13.jpg", FileMode.Open);
        
        var img = System.Drawing.Image.FromStream(fs);
        var tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
        img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

        var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

        if (key is null) throw new NullReferenceException();
        
        key.SetValue(@"WallpaperStyle", 22.ToString());
        key.SetValue(@"TileWallpaper", 0.ToString());
        
        SystemParametersInfo(20, 0, tempPath, 0x01 | 0x02);
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