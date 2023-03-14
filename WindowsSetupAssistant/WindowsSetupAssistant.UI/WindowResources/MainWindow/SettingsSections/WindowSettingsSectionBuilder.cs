using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to window settings
/// </summary>
public class WindowSettingsSectionBuilder
{
    private readonly WindowsUiHelper _uiHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="uiHelper">Injected UiHelper</param>
    public WindowSettingsSectionBuilder(WindowsUiHelper uiHelper)
    {
        _uiHelper = uiHelper;
    }
    
    /// <summary>
    /// Creates the section in MainWindow relating to the time settings
    /// </summary>
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
                
            }
        };
        
        var turnOffTransparency = new OptionInternalMethod()
        {
            DisplayName = "Set windows theme to not use transparency",
            ExecuteSetting = () =>
            {
                
            }
        };
        
        var blackInactiveTitleBars = new OptionInternalMethod()
        {
            DisplayName = "Set inactive title bars to be a dark color also",
            ExecuteSetting = () =>
            {
                
            }
        };
        
        parentSection.Settings.Add(setWindowsThemeToDark);
        parentSection.Settings.Add(turnOffTransparency);
        parentSection.Settings.Add(blackInactiveTitleBars);

        return parentSection;
    }
}