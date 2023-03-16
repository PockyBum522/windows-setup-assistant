using System.Runtime.Versioning;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class DesktopSettingsSectionBuilder
{
    private readonly DesktopHelper _desktopHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="desktopHelper">Injected TimeHelper</param>
    public DesktopSettingsSectionBuilder(DesktopHelper desktopHelper)
    {
        _desktopHelper = desktopHelper;
    }
    
    /// <summary>
    /// Creates the section in MainWindow relating to the time settings
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public SettingsSection MakeSection()
    {
        var parentSection = new SettingsSection()
        {
            DisplayName = "Desktop"
        };

        var taskbarSearchToHidden = new OptionInternalMethod()
        {
            DisplayName = "Delete all shortcuts off desktop when finished",
            ExecuteSetting = () =>
            {
                _desktopHelper.CleanDesktopOfAllFilesMatching(new []{".lnk"});
            }
        };
        
        var taskbarSearchToIcon = new OptionInternalMethod()
        {
            DisplayName = "Delete all .ini files off desktop when finished",
            ExecuteSetting = () =>
            {
                _desktopHelper.CleanDesktopOfAllFilesMatching(new []{".ini"});
            }
        };
        
        var wallpaperToDarkImage = new OptionInternalMethod()
        {
            DisplayName = "Set wallpaper to dark image (Windows camping)",
            ExecuteSetting = () =>
            {
                _desktopHelper.SetWallpaperToDarkDefaultWallpaper();
            }
        };
        
        parentSection.Settings.Add(taskbarSearchToHidden);
        parentSection.Settings.Add(taskbarSearchToIcon);
        parentSection.Settings.Add(wallpaperToDarkImage);

        return parentSection;
    }
}