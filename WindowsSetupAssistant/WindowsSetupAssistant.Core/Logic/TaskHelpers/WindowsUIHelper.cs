using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

public class WindowsUiHelper
{
    private readonly ILogger _logger;

    public WindowsUiHelper(ILogger logger)
    {
        _logger = logger;
    }

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

    public void DisableNewsAndInterestsOnTaskbar()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Feeds", true);

        key?.SetValue("ShellFeedsTaskbarViewMode", 2);
    }
    
    [DllImport("user32.dll", SetLastError=true)]
    public static extern bool SetSysColors(int cElements, int [] lpaElements, int [] lpaRgbValues);
    public const int ColorDesktop = 1;

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

    public void SetFolderViewOptions()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("Start_SearchFiles", 2);
        key.SetValue("ServerAdminUI", 0);
        key.SetValue("Hidden", 1);
        key.SetValue("ShowCompColor", 1);
        key.SetValue("HideFileExt", 0);
        key.SetValue("DontPrettyPath", 0);
        key.SetValue("ShowInfoTip", 1);
        key.SetValue("HideIcons", 0);
        key.SetValue("MapNetDrvBtn", 0);
        key.SetValue("WebView", 1);
        key.SetValue("Filter", 0);
        key.SetValue("ShowSuperHidden", 1);
        key.SetValue("SeparateProcess", 1);
        key.SetValue("AutoCheckSelect", 0);
        key.SetValue("IconsOnly", 0);
        key.SetValue("ShowTypeOverlay", 1);
        key.SetValue("ShowStatusBar", 1);
        key.SetValue("StoreAppsOnTaskbar", 1);
        key.SetValue("ListviewAlphaSelect", 1);
        key.SetValue("ListviewShadow", 1);
        key.SetValue("TaskbarAnimations", 1);
        key.SetValue("ShowCortanaButton", 0);
        key.SetValue("ReindexedProfile", 1);
        key.SetValue("StartMenuInit", 13);
        key.SetValue("TaskbarGlomLevel", 1);
        key.SetValue("MMTaskbarGlomLevel", 1);
        key.SetValue("ShowTaskViewButton", 0);
        key.SetValue("AlwaysShowMenus", 1);
        key.SetValue("ShowEncryptCompressedColor", 1);
        key.SetValue("SharingWizardOn", 0);
        key.SetValue("MMTaskbarEnabled", 0);
        key.SetValue("TaskbarSizeMove", 0);
    }
    
    public void CollapseSearchOnTaskbarToIcon()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 1);
    }
    
    public void CollapseSearchOnTaskbarToHidden()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 0);
    }

    public void CleanDesktopOfAllFilesMatching(string[] patternsToDeleteWithoutWildcards)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        foreach (var file in Directory.GetFiles(desktopPath))
        {
            foreach (var pattern in patternsToDeleteWithoutWildcards)
            {
                if (file.EndsWith(pattern)) File.Delete(file);    
            }
        }
    }
}