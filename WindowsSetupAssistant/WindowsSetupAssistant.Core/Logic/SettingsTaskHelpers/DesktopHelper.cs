﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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
    /// Sets Windows wallpaper to the dark "Camping under the stars" wallpaper, which is less blinding than the default
    /// </summary>
    /// <exception cref="NullReferenceException">Throws if registry access error</exception>
    [SupportedOSPlatform("Windows7.0")]
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
    public void CleanDesktopOfAllFilesMatching(string[] extensionsToDeleteWithoutWildcards)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var publicDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

        foreach (var pattern in extensionsToDeleteWithoutWildcards)
        {
            DeleteAllFilesWithExtension(desktopPath, pattern);
            DeleteAllFilesWithExtension(publicDesktopPath, pattern);
        }
    }

    private void DeleteAllFilesWithExtension(string pathToDeleteIn, string extensionToMatch)
    {
        var filesOnCommonDesktop = Directory.GetFiles(pathToDeleteIn); 
        
        foreach (var file in filesOnCommonDesktop)
        {
            if (file.EndsWith(extensionToMatch)) File.Delete(file);   
        }
    }
}