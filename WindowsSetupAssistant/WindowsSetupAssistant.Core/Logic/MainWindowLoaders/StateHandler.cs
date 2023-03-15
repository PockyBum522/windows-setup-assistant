using System.IO;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Core.Models;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders;

/// <summary>
/// Handles state loading and saving, and profile loading and saving
/// </summary>
public class StateHandler
{
    private readonly ILogger _logger;
    private MainWindowPersistentState _mainWindowPersistentState;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    /// <param name="mainWindowPersistentState">The main state of the application and user's choices that persists after a reboot</param>
    public StateHandler(
        ILogger logger, 
        MainWindowPersistentState mainWindowPersistentState)
    {
        _logger = logger;
        _mainWindowPersistentState = mainWindowPersistentState;
    }
    
    /// <summary>
    /// Saves the current state to disk at location specified by StatePath
    /// </summary>
    public void SaveCurrentStateAsJson(string profileJsonFilePath)
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };

        if (!profileJsonFilePath.ToLower().EndsWith(".json"))
            profileJsonFilePath += ".json";
        
        using var jsonStateFileWriter = new StreamWriter(profileJsonFilePath);
        
        using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };
        
        serializer.Serialize(jsonStateWriter, _mainWindowPersistentState);
    }
    
    /// <summary>
    /// Saves the current state to disk at location specified by StatePath
    /// </summary>
    public MainWindowPersistentState GetStateFromJson(string profileJsonFilePath)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        if (!File.Exists(profileJsonFilePath)) throw new JsonSerializationException();
        
        // Otherwise:
        var jsonStateRaw = File.ReadAllText(profileJsonFilePath);

        var newMainWindowPartialViewModel =
            JsonConvert.DeserializeObject<MainWindowPersistentState>(jsonStateRaw, settings) ??
            new MainWindowPersistentState();

        _logger.Debug("Loaded current state from disk");
            
        return newMainWindowPartialViewModel;
    }
    
    /// <summary>
    /// Saves the current state to disk at location specified by StatePath
    /// </summary>
    public void LoadStateFromJsonIntoPersistentState(string profileJsonFilePath)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        if (!File.Exists(profileJsonFilePath)) throw new JsonSerializationException();
        
        // Otherwise:
        var jsonStateRaw = File.ReadAllText(profileJsonFilePath);

        _mainWindowPersistentState =
            JsonConvert.DeserializeObject<MainWindowPersistentState>(jsonStateRaw, settings) ??
            new MainWindowPersistentState();

        _logger.Debug("Loaded current state from disk into persistent state instance");
    }
}