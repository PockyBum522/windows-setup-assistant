using System.Runtime.Versioning;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders.SettingsSectionBuilders;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class DesktopSettingsSectionBuilder
{
    private readonly DesktopHelper _desktopHelper;
    private readonly DisplayHelper _displayHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="desktopHelper">Injected TimeHelper</param>
    /// <param name="displayHelper">Injected DisplayHelper</param>
    public DesktopSettingsSectionBuilder(
        DesktopHelper desktopHelper,
        DisplayHelper displayHelper)
    {
        _desktopHelper = desktopHelper;
        _displayHelper = displayHelper;
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
        
        var allMonitorsFullScaling = new OptionInternalMethod()
        {
            DisplayName = "Set all monitors to 100% scaling",
            ExecuteSetting = () =>
            {
                _displayHelper.SetDpiValueToZeroForAllMonitors();
            }
        };
        
        parentSection.Settings.Add(taskbarSearchToHidden);
        parentSection.Settings.Add(taskbarSearchToIcon);
        parentSection.Settings.Add(wallpaperToDarkImage);
        parentSection.Settings.Add(allMonitorsFullScaling);

        return parentSection;
    }
}