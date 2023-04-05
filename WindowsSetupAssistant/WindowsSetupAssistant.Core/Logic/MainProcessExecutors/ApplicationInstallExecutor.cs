using System.Runtime.Versioning;
using Serilog;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.IInstallables;

namespace WindowsSetupAssistant.Core.Logic.MainProcessExecutors;

/// <summary>
/// The ViewModel for MainWindow
/// </summary>
public class ApplicationInstallExecutor
{
    private readonly ILogger _logger;
    private readonly SessionPersistentState _sessionPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public ApplicationInstallExecutor(ILogger logger,
        SessionPersistentState sessionPersistentState)
    { 
        _logger = logger;
        _sessionPersistentState = sessionPersistentState;
    }
    
    /// <summary>
    /// Handles installing all user-non-interactive (Everything except ExecutableInstallers) that the user selected as should be installed 
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void WorkAllNonInteractiveApplicationInstalls()
    {
        // Install 7zip no matter what because we need it later for the portable apps
        new ChocolateyInstaller() { ChocolateyId = "7Zip" }.ExecuteInstall(_logger);

        // Run non-executable installers and get them out of the way first since they require no user interaction
        foreach (var installer in _sessionPersistentState.AvailableInstalls)
        {
            _logger.Information("Checking installer: {DisplayName} which IsSelected? {IsSelected}", installer.DisplayName, installer.IsSelected);
            
            if (!installer.IsSelected) continue;

            if (installer is not ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }

    }
    
    /// <summary>
    /// Handles installing all user-interactive (Only ExecutableInstallers) that the user selected as should be installed 
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void WorkAllInteractiveApplicationInstalls()
    {
        // Install 7zip no matter what because we need it later for the portable apps
        new ChocolateyInstaller() { ChocolateyId = "7Zip" }.ExecuteInstall(_logger);

        foreach (var installer in _sessionPersistentState.AvailableInstalls)
        {
            _logger.Information("Checking installer: {DisplayName} which IsSelected? {IsSelected}", installer.DisplayName, installer.IsSelected);
            
            if (!installer.IsSelected) continue;

            if (installer is ExecutableInstaller)
                installer.ExecuteInstall(_logger);
        }
    }
}