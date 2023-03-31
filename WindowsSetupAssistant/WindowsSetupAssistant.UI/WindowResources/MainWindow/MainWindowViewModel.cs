using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using WindowsSetupAssistant.Core.Logic.MainProcessExecutors;
using WindowsSetupAssistant.Core.Logic.MainWindowLoaders;
using WindowsSetupAssistant.Core.Models;
using WindowsSetupAssistant.Core.Models.Enums;
using WindowsSetupAssistant.UI.WindowResources.InstallsEditor;
using WindowsSetupAssistant.UI.WpfHelpers;

namespace WindowsSetupAssistant.UI.WindowResources.MainWindow;

/// <summary>
/// The ViewModel for MainWindow
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private string _textRunningAsUsernameMessage = "";
    [ObservableProperty] private SessionPersistentState? _localSessionPersistentState;
    
    private readonly ILogger _logger;
    private readonly InstallsEditorWindow _installsEditorWindow;
    private readonly MainSetupProcessExecutor _mainSetupProcessExecutor;
    private readonly AvailableApplicationsJsonLoader _availableApplicationsJsonLoader;
    private readonly ProfileHandler _profileHandler;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="sessionPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    /// <param name="installsEditorWindow">Injected Installs editor window</param>
    /// <param name="installsEditorWindowViewModel">View model for the installs editor</param>
    /// <param name="mainSetupProcessExecutor">Handles babysitting the main setup process once the user clicks start</param>
    /// <param name="availableApplicationsJsonLoader">Loads "Available Applications.json" file into CurrentState</param>
    /// <param name="profileHandler">Injected to save/load profiles for this window</param>
    public MainWindowViewModel(
        ILogger logger,
        SessionPersistentState sessionPersistentState,
        InstallsEditorWindow installsEditorWindow,
        InstallsEditorWindowViewModel installsEditorWindowViewModel,
        MainSetupProcessExecutor mainSetupProcessExecutor,
        AvailableApplicationsJsonLoader availableApplicationsJsonLoader,
        ProfileHandler profileHandler)
    {
        _logger = logger;
        _mainSetupProcessExecutor = mainSetupProcessExecutor;
        _availableApplicationsJsonLoader = availableApplicationsJsonLoader;
        _profileHandler = profileHandler;
        
        // Installs editor window and view model setup
        _installsEditorWindow = installsEditorWindow;
        _installsEditorWindow.DataContext = installsEditorWindowViewModel;
        
        LocalSessionPersistentState = sessionPersistentState;
        
        //TextRunningAsUsernameMessage = $"Running As DomainUser: {Environment.UserDomainName} | User: {Environment.UserName}";
    }

    [RelayCommand]
    private async Task StartMainSetupProcess()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        LocalSessionPersistentState.ScriptStage = ScriptStageEnum.FirstRun;
        
        await _mainSetupProcessExecutor.ExecuteNextSetupProcessStage();   
    }
    
    /// <summary>
    /// Handles looking at the saved state on the disk, finding out what stage we're at, and if it's not Uninitialized,
    /// handles working that stage's tasks
    /// </summary>
    [RelayCommand]
    public async Task ExecuteNextSetupProcessStage()
    {
        await _mainSetupProcessExecutor.ExecuteNextSetupProcessStage();
    }
    
    [RelayCommand, SupportedOSPlatform("Windows7.0")]
    private void SaveProfile() => _profileHandler.PromptUserToBrowseAndSaveProfile();

    [RelayCommand, SupportedOSPlatform("Windows7.0")]
    private void LoadProfile()
    {
        ClearAll();
        
        _profileHandler.PromptUserForProfileThenLoadIt();
    }
    
    [RelayCommand, SupportedOSPlatform("Windows7.0")]
    private void SelectAllSettingsAndInstallers()
    {
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        foreach (var propertyInfo in LocalSessionPersistentState.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(LocalSessionPersistentState, true);
        }

        foreach (var installer in LocalSessionPersistentState.AvailableInstalls)
        {
            installer.IsSelected = true;
        }
        
        foreach (var section in LocalSessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                setting.IsSelected = true;
            }
        }
    }
    
    [RelayCommand, SupportedOSPlatform("Windows7.0")]
    private void ClearAll()
    {
        if (LocalSessionPersistentState is null) throw new NullReferenceException();
        
        foreach (var propertyInfo in LocalSessionPersistentState.GetType().GetProperties())
        {
            if (propertyInfo.PropertyType != typeof(bool)) continue;

            if (!propertyInfo.Name.Contains("Checked")) continue;

            propertyInfo.SetValue(LocalSessionPersistentState, false);
        }

        foreach (var installer in LocalSessionPersistentState.AvailableInstalls)
        {
            installer.IsSelected = false;
        }

        foreach (var section in LocalSessionPersistentState.SettingsSections)
        {
            foreach (var setting in section.Settings)
            {
                setting.IsSelected = false;
            }
        }
        
        LocalSessionPersistentState.TextHostname = "";


        LocalSessionPersistentState.IsCheckedUpdateWindows = false;
        
        LocalSessionPersistentState.TextHostname = "";

        LocalSessionPersistentState.TextMonitorTimeoutOnAc = "";
        LocalSessionPersistentState.TextMonitorTimeoutOnBattery = "";
        LocalSessionPersistentState.TextStandbyTimeoutOnAc = "";
        LocalSessionPersistentState.TextStandbyTimeoutOnBattery = "";
        LocalSessionPersistentState.TextHibernateTimeoutOnAc = "";
        LocalSessionPersistentState.TextHibernateTimeoutOnBattery = "";
    }

    [RelayCommand] 
    private void ShowInstallerEditorWindow()
    {
        _installsEditorWindow.Show();
        ((InstallsEditorWindowViewModel)_installsEditorWindow.DataContext).DeserializeInstallersJson();
    }

    [RelayCommand]
    private void HandleAvailableInstallsListViewMouseWheel(object mouseWheelEventArgs)
    {
        ListViewMouseWheelScroller.OnPreviewMouseWheelMove((MouseWheelEventArgs)mouseWheelEventArgs);
    }
    
    [RelayCommand]
    private void MainWindowOnClosing()
    {
        // Clean up editor window
        _installsEditorWindow.Close();
    }
        

    [RelayCommand] 
    private void ReloadInstallerList() => _availableApplicationsJsonLoader.LoadAvailableInstallersFromJsonFile();
}