using System.Runtime.Versioning;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to window settings
/// </summary>
public class WindowSettingsSectionBuilder
{
    private readonly WindowHelper _windowHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="windowHelper">Injected UiHelper</param>
    public WindowSettingsSectionBuilder(WindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
    }
    
    /// <summary>
    /// Creates the section in MainWindow relating to the time settings
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public SettingsSection MakeSection()
    {
        var parentSection = new SettingsSection()
        {
            DisplayName = "Window Settings"
        };

        var setWindowsThemeToDark = new OptionInternalMethod()
        {
            DisplayName = "Set windows theme to dark theme",
            ExecuteSetting = () =>
            {
                _windowHelper.ChangeWindowsThemeToDark();
            }
        };
        
        var turnOffTransparency = new OptionInternalMethod()
        {
            DisplayName = "Set windows theme to not use transparency",
            ExecuteSetting = () =>
            {
                _windowHelper.DisableWindowTransparency();
            }
        };
        
        var blackInactiveTitleBars = new OptionInternalMethod()
        {
            DisplayName = "Set inactive title bars to be a dark color also",
            ExecuteSetting = () =>
            {
                _windowHelper.BlackActiveAndInactiveTitleBars();
            }
        };
        
        parentSection.Settings.Add(setWindowsThemeToDark);
        parentSection.Settings.Add(turnOffTransparency);
        parentSection.Settings.Add(blackInactiveTitleBars);

        return parentSection;
    }
}