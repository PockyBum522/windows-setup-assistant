using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class TaskbarSettingsSectionBuilder
{
    private readonly TaskbarHelper _taskbarHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="taskbarHelper">Injected Taskbar Helper</param>
    public TaskbarSettingsSectionBuilder(TaskbarHelper taskbarHelper)
    {
        _taskbarHelper = taskbarHelper;
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
                _taskbarHelper.CollapseSearchOnTaskbarToHidden();
            }
        };
        
        var taskbarSearchToIcon = new OptionInternalMethod()
        {
            DisplayName = "Collapse Taskbar Search to Just Icon",
            ExecuteSetting = () =>
            {
                _taskbarHelper.CollapseSearchOnTaskbarToIcon();
            }
        };
        
        var turnOffNewsAndInterests = new OptionInternalMethod()
        {
            DisplayName = "Disable news and interests on taskbar",
            ExecuteSetting = () =>
            {
                _taskbarHelper.DisableNewsAndInterestsOnTaskbar();
            }
        };
        
        parentSection.Settings.Add(taskbarSearchToHidden);
        parentSection.Settings.Add(taskbarSearchToIcon);
        parentSection.Settings.Add(turnOffNewsAndInterests);

        return parentSection;
    }
}