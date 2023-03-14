using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowHelpers.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class DesktopSettingsSectionBuilder
{
    private readonly WindowsUiHelper _uiHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="uiHelper">Injected TimeHelper</param>
    public DesktopSettingsSectionBuilder(WindowsUiHelper uiHelper)
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
            DisplayName = "Desktop"
        };

        var taskbarSearchToHidden = new OptionInternalMethod()
        {
            DisplayName = "Delete All Shortcuts off Desktop When Finished",
            ExecuteSetting = () =>
            {
                _uiHelper.CollapseSearchOnTaskbarToHidden();
            }
        };
        
        var taskbarSearchToIcon = new OptionInternalMethod()
        {
            DisplayName = "Delete All .ini Files off Desktop When Finished",
            ExecuteSetting = () =>
            {
                _uiHelper.CollapseSearchOnTaskbarToIcon();
            }
        };
        
        parentSection.Settings.Add(taskbarSearchToHidden);
        parentSection.Settings.Add(taskbarSearchToIcon);

        return parentSection;
    }
}