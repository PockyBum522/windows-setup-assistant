﻿using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Represents an application that will be installed through Chocolatey windows package manager
/// </summary>
public class ChocolateyInstaller : BaseInstaller
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public ChocolateyInstaller(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The ID of the package at https://community.chocolatey.org/packages (basically whatever would come after choco install)
    /// </summary>
    public string ChocolateyId { get; set; } = "";
    
    /// <summary>
    /// Any arguments chocolatey needs to install the package. These are optional and generally not needed
    /// </summary>
    public string Arguments { get; set; } = "";

    /// <summary>
    /// Any parameters chocolatey needs to install the package. These are optional and generally not needed
    /// </summary>
    public string Parameters { get; set; } = "";
    
    /// <inheritdoc/>
    public override void ExecuteInstall()
    {
        _logger.Information("Installing {PackageName} with Chocolatey", ChocolateyId);

        var procInfo = new ProcessStartInfo();

        var argsString = $"upgrade {ChocolateyId}";

        if (!string.IsNullOrWhiteSpace(Arguments))
        {
            argsString += $" --install-arguments='{Arguments}'";
        }

        if (!string.IsNullOrWhiteSpace(Parameters))
        {
            argsString += $" --params '{Parameters}'";
        }

        procInfo.Arguments = argsString;

        procInfo.FileName = "choco";

        var proc = Process.Start(procInfo);

        proc?.WaitForExit();   
    }
}