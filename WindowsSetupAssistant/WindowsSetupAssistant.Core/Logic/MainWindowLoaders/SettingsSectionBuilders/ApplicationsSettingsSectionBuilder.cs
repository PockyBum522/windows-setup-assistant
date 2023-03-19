using System.Runtime.Versioning;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models.ISelectableSettings;
using WindowsSetupAssistant.Core.Models.ISelectableSettings.ISelectableSettings;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders.SettingsSectionBuilders;

/// <summary>
/// Creates the section in MainWindow relating to the time settings
/// </summary>
public class ApplicationsSettingsSectionBuilder
{
    private readonly WindowsStoreApplicationsUninstaller _windowsStoreApplicationsUninstaller;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="windowsStoreApplicationsUninstaller">Injected WindowsStoreApplicationsUninstaller</param>
    public ApplicationsSettingsSectionBuilder(
        WindowsStoreApplicationsUninstaller windowsStoreApplicationsUninstaller)
    {
        _windowsStoreApplicationsUninstaller = windowsStoreApplicationsUninstaller;
    }
    
    /// <summary>
    /// Creates the section in MainWindow relating to the time settings
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public SettingsSection MakeSection()
    {
        var parentSection = new SettingsSection()
        {
            DisplayName = "Applications"
        };

        var uninstallWindowsStoreSpotify = new OptionInternalMethod()
        {
            DisplayName = "Uninstall Windows Store version of spotify",
            ExecuteSetting = () =>
            {
                _windowsStoreApplicationsUninstaller.UninstallApplication("Spotify");
            }
        };
        
        parentSection.Settings.Add(uninstallWindowsStoreSpotify);

        return parentSection;
    }
}