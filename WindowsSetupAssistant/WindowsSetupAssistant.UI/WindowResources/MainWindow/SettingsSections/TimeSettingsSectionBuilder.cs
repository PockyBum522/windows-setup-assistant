using WindowsSetupAssistant.Core.Logic.TaskHelpers;
using WindowsSetupAssistant.Core.Models.Settings;
using WindowsSetupAssistant.Core.Models.Settings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowHelpers.SettingsSections;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class TimeSettingsSectionBuilder
{
    private readonly TimeHelper _timeHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="timeHelper">Injected TimeHelper</param>
    public TimeSettingsSectionBuilder(TimeHelper timeHelper)
    {
        _timeHelper = timeHelper;
    }
    
    /// <summary>
    /// Creates the section in MainWindow relating to the time settings
    /// </summary>
    public SettingsSection MakeSection()
    {
        var parentSection = new SettingsSection()
        {
            DisplayName = "System Time"
        };

        var timeSyncOption = new OptionInternalMethod()
        {
            DisplayName = "Synchronize System Time with NTP",
            ExecuteSetting = () =>
            {
                _timeHelper.SyncSystemTime();
            }
        };
        
        var timeZoneSetToEasternOption = new OptionInternalMethod()
        {
            DisplayName = "Set Timezone to (-5) EST",
            ExecuteSetting = () =>
            {
                _timeHelper.SetSystemTimeZone("Eastern Standard Time");
            }
        };
        
        parentSection.Settings.Add(timeSyncOption);
        parentSection.Settings.Add(timeZoneSetToEasternOption);

        return parentSection;
    }
}