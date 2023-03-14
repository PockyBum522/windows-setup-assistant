using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Core.Logic.Application;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.Core.Logic.MainWindowLoaders;

/// <summary>
/// Helper to load in the "Available Installs.json" and update CurrentState with entries
/// </summary>
public class AvailableApplicationsJsonLoader
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    public AvailableApplicationsJsonLoader(
        ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Load in the "Available Installs.json" and updates currentState with entries
    /// </summary>
    /// <param name="currentState">CurrentState object to update</param>
    /// <exception cref="NullReferenceException">Thrown if deserialized available installs object is null</exception>
    public void LoadAvailableInstallersFromJsonFile(CurrentState currentState)
    {
        var availableInstallsJsonRaw = File.ReadAllText(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        _logger.Debug("About to deserialize Available Installs.json");
        
        var availableInstalls =
            JsonConvert.DeserializeObject<List<IInstallable>>(availableInstallsJsonRaw, jsonSerializerSettings);

        if (availableInstalls is null) throw new NullReferenceException();

        _logger.Information("Clearing MainWindowPartialViewModel.AvailableInstalls");
        currentState.MainWindowPartialViewModel.AvailableInstalls.Clear();

        foreach (var availableInstall in availableInstalls)
        {
            _logger.Information("Got {InstallName} in deserialization, adding install to MainWindowPartialViewModel.AvailableInstalls", availableInstall.DisplayName);
            
            currentState.MainWindowPartialViewModel.AvailableInstalls.Add(availableInstall);
        }
    }
}