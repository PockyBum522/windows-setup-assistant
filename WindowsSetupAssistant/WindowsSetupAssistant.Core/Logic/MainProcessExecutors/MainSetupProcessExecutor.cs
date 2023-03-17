using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders.SettingsSectionBuilders;
using WindowsSetupAssistant.Core.Logic.SettingsTaskHelpers;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.Core.Models.IInstallables;

namespace WindowsSetupAssistant.Core.Logic.MainProcessExecutors;

/// <summary>
/// The ViewModel for MainWindow
/// </summary>
public class MainSetupProcessExecutor
{
    private readonly SessionPersistentState _sessionPersistentState;
    private readonly SelectedSettingsExecutor _selectedSettingsExecutor;
    private readonly ApplicationInstallExecutor _applicationInstallExecutor;
    private readonly StateHandler _stateHandler;
    private readonly SystemRebooter _systemRebooter;
    private readonly StartupScriptWriter _startupScriptWriter;
    private readonly WindowsUpdater _windowsUpdater;
    private readonly PowerHelper _powerHelper;
    private readonly WindowsHostnameHelper _windowsHostnameHelper;
    private readonly AvailableApplicationsJsonLoader _availableApplicationsJsonLoader;
    private readonly SettingsSectionsController _settingsSectionsController;
    private readonly FinalCleanupHelper _finalCleanupHelper;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="systemRebooter">Injected</param>
    /// <param name="startupScriptWriter">Creates the startup script that reruns the program on reboot</param>
    /// <param name="windowsUpdater">Windows update helper</param>
    /// <param name="powerHelper">Windows power settings helper</param>
    /// <param name="windowsHostnameHelper">Windows hostname helper</param>
    /// <param name="availableApplicationsJsonLoader">Loads "Available Applications.json" file into CurrentState</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    /// <param name="selectedSettingsExecutor">Handles actually changing any settings the user selected should be changed during the setup process</param>
    /// <param name="applicationInstallExecutor">Handles installing any applications the user selected should be installed during the setup process</param>
    /// <param name="finalCleanupHelper">Injected to clean up all files on disk when application is finished</param>
    /// <param name="stateHandler">Injected to handle state loading and saving, and profile loading and saving</param>
    /// <param name="settingsSectionsController">Handles using all the SettingsSectionBuilders to set up all settings sections</param>
    public MainSetupProcessExecutor(
        SessionPersistentState sessionPersistentState,
        SelectedSettingsExecutor selectedSettingsExecutor,
        ApplicationInstallExecutor applicationInstallExecutor,
        FinalCleanupHelper finalCleanupHelper,
        StateHandler stateHandler,
        SystemRebooter systemRebooter,
        StartupScriptWriter startupScriptWriter,
        WindowsUpdater windowsUpdater,
        PowerHelper powerHelper,
        WindowsHostnameHelper windowsHostnameHelper,
        AvailableApplicationsJsonLoader availableApplicationsJsonLoader,
        SettingsSectionsController settingsSectionsController)
    { 
        _sessionPersistentState = sessionPersistentState;
        _selectedSettingsExecutor = selectedSettingsExecutor;
        _applicationInstallExecutor = applicationInstallExecutor;
        _finalCleanupHelper = finalCleanupHelper;
        _stateHandler = stateHandler;
        _systemRebooter = systemRebooter;
        _startupScriptWriter = startupScriptWriter;
        _windowsUpdater = windowsUpdater;
        _powerHelper = powerHelper;
        _windowsHostnameHelper = windowsHostnameHelper;
        _availableApplicationsJsonLoader = availableApplicationsJsonLoader;
        _settingsSectionsController = settingsSectionsController;
    }
    
    /// <summary>
    /// Handles executing the tasks in the next stage of the setup process
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    // ReSharper disable once CognitiveComplexity because it's extremely linear and it's more understandable like this
    public async Task ExecuteNextSetupProcessStage()
    {
        DisableSystemSleep();  // So PC doesn't sleep during long installs

        switch (_sessionPersistentState.ScriptStage)
        {
            case ScriptStageEnum.Uninitialized:
                
                // We don't need to do those except for the first time, because the rest of the time they'll come from 
                // the CSharpInstallerScriptState.json file on the disk
                
                _availableApplicationsJsonLoader.LoadAvailableInstallersFromJsonFile();
                _settingsSectionsController.LoadAllSettingsSections();
                
                break;
            
            case ScriptStageEnum.FirstRun:

                OnNextBootResumeAtStage(ScriptStageEnum.WindowsHasBeenUpdatedOnce);
        
                _selectedSettingsExecutor.ExecuteSelectedSettingsInAllSections();

                await UpdateWindowsAndReboot();
                
                // And if we didn't reboot in the line above, it means the user didn't select to update windows, so:
                await JumpToWindowsHasBeenUpdatedFully();

                break;
            
            case ScriptStageEnum.WindowsHasBeenUpdatedOnce:

                OnNextBootResumeAtStage(ScriptStageEnum.WindowsHasBeenUpdatedTwice);
                
                await UpdateWindowsAndReboot();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedTwice:

                OnNextBootResumeAtStage(ScriptStageEnum.WindowsHasBeenUpdatedFully);

                await UpdateWindowsAndReboot();

                break;

            case ScriptStageEnum.WindowsHasBeenUpdatedFully:
                
                OnNextBootResumeAtStage(ScriptStageEnum.RunFinalSettings);
                
                if (_sessionPersistentState.IsCheckedUpdateWindows) _windowsUpdater.UpdateWindows();
                
                _applicationInstallExecutor.WorkAllNonInteractiveApplicationInstalls();

                if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextHostname))
                    _windowsHostnameHelper.ChangeHostName(_sessionPersistentState.TextHostname);
                
                _systemRebooter.RebootComputerAndExit();

                break;
            
            case ScriptStageEnum.RunFinalSettings:
                
                _finalCleanupHelper.DeleteSavedStateFileOnDisk();
                
                // Some settings here need to be re-run at the end to take effect.
                // Not to mention some have to be run at the end by their nature, such as desktop clean up
                _selectedSettingsExecutor.ExecuteSelectedSettingsInAllSections();
                
                _applicationInstallExecutor.WorkInteractiveApplicationInstalls();
                
                await HandleFinalRebootByInteractiveInstallsCount();

                break;
        }
    }

    private void OnNextBootResumeAtStage(ScriptStageEnum stageToResumeOn)
    {
        _sessionPersistentState.ScriptStage = stageToResumeOn;
        _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);
        _startupScriptWriter.CreateRebootScriptInStartup();
    }

    private void DisableSystemSleep()
    {
        // Set long sleep and monitor off times so it doesn't sleep during install
        _powerHelper.SetPowerSettingsTo();
    }

    private async Task JumpToWindowsHasBeenUpdatedFully()
    {
        _sessionPersistentState.ScriptStage = ScriptStageEnum.WindowsHasBeenUpdatedFully;
        _stateHandler.SaveCurrentStateAsJson(ApplicationPaths.StatePath);

        await ExecuteNextSetupProcessStage();
    }

    private async Task UpdateWindowsAndReboot()
    {
        DisableSystemSleep();
        
        if (!_sessionPersistentState.IsCheckedUpdateWindows) return;
        
        // Otherwise:
        _windowsUpdater.UpdateWindowsAndReboot();
        
        _systemRebooter.RebootComputerAndExit();

        // Make sure any code after calling this method doesn't execute
        await Task.Delay(9999999);
    }

    private async Task HandleFinalRebootByInteractiveInstallsCount()
    {
        var interactiveInstallersToRunCount =
            _sessionPersistentState.AvailableInstalls.Count(
                install => install is ExecutableInstaller && install.IsSelected);

        if (interactiveInstallersToRunCount > 0)
        {
            SetPowerSettingsToUserChoicesAtStart();
            await _systemRebooter.PromptToRebootComputerAndExit();
        }
        else
        {
            SetPowerSettingsToUserChoicesAtStart();
            _systemRebooter.RebootComputerAndExit();
        }
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetPowerSettingsToUserChoicesAtStart()
    {
        var monitorTimeoutOnAc = 20; // Start with defaults
        var monitorTimeoutOnBattery = 5;
        var standbyTimeoutOnAc = 60;
        var standbyTimeoutOnBattery = 10;
        var hibernateTimeoutOnAc = 0;
        
        if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextMonitorTimeoutOnAc))
            monitorTimeoutOnAc = int.Parse(_sessionPersistentState.TextMonitorTimeoutOnAc);
        
        if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextMonitorTimeoutOnBattery))
            monitorTimeoutOnBattery = int.Parse(_sessionPersistentState.TextMonitorTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextStandbyTimeoutOnAc))
            standbyTimeoutOnAc = int.Parse(_sessionPersistentState.TextStandbyTimeoutOnAc);

        if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextStandbyTimeoutOnBattery))
            standbyTimeoutOnBattery = int.Parse(_sessionPersistentState.TextStandbyTimeoutOnBattery);
        
        if (!string.IsNullOrWhiteSpace(_sessionPersistentState.TextHibernateTimeoutOnAc))
            hibernateTimeoutOnAc = int.Parse(_sessionPersistentState.TextHibernateTimeoutOnAc);
        
        _powerHelper.SetPowerSettingsTo(monitorTimeoutOnAc, monitorTimeoutOnBattery, standbyTimeoutOnAc, standbyTimeoutOnBattery, hibernateTimeoutOnAc);
    }

}