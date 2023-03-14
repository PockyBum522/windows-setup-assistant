using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowHelpers.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class TaskbarSettingsSectionBuilder
{
    private readonly WindowsUiHelper _uiHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="uiHelper">Injected TimeHelper</param>
    public TaskbarSettingsSectionBuilder(WindowsUiHelper uiHelper)
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
            DisplayName = "Taskbar"
        };

        var taskbarSearchToHidden = new OptionInternalMethod()
        {
            DisplayName = "Set Taskbar Search to Hidden",
            ExecuteSetting = () =>
            {
                _uiHelper.CollapseSearchOnTaskbarToHidden();
            }
        };
        
        var taskbarSearchToIcon = new OptionInternalMethod()
        {
            DisplayName = "Collapse Taskbar Search to Just Icon",
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