using System.Runtime.Versioning;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Models;

namespace WindowsSetupAssistant.Core.Logic.MainProcessExecutors;

/// <summary>
/// Handles executing any settings the user selected during the main setup process execution
/// </summary>
public class SelectedSettingsExecutor : ObservableObject
{
    private readonly ILogger _logger;
    private readonly SessionPersistentState _sessionPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public SelectedSettingsExecutor(
        ILogger logger,
        SessionPersistentState sessionPersistentState)
    { 
        _logger = logger;
        _sessionPersistentState = sessionPersistentState;
    }
    
    
    /// <summary>
    /// Handles executing any settings the user selected during the main setup process execution
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void ExecuteSelectedSettingsInAllSections()
    {
        foreach (var section in _sessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                if (!setting.IsSelected) continue;
                
                // Otherwise:
                _logger.Debug("Executing setting: {Name}", setting.DisplayName);
                    
                setting.ExecuteSetting?.Invoke();
            }
        }
    }
}