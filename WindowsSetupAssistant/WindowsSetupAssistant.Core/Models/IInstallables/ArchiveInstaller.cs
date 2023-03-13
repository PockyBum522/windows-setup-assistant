using System;
using System.Diagnostics;
using System.IO;
using Serilog;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains data for an archive in the \Resources\Portable Applications\ folder
/// Mostly what to show for the display name and where to install it if the user selects it
/// </summary>
public class ArchiveInstaller : IInstallable
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public ArchiveInstaller(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Path to the folder containing the portable application
    /// </summary>
    public string ArchiveFilename { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";

    /// <inheritdoc/>
    public void ExecuteInstall()
    {        
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        Directory.CreateDirectory(Path.GetDirectoryName(DestinationPath) ?? throw new DirectoryNotFoundException());
        
        var searchInPath =
            Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "WindowsSetupAssistant",
                "Resources",
                "Installer Archives");
        
        var archiveToInstallPath =
            Path.Join(
                searchInPath,
                ArchiveFilename);
        
        var arguments = $"""e {archiveToInstallPath} -o"{DestinationPath}" -r""";
        
        _logger.Debug("Extracting Archive: {ArchiveToInstallPath}", archiveToInstallPath);
        _logger.Debug("With arguments: {Args}", arguments);
        
        var sevenZipExecutablePath =
            Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "7-Zip",
                "7z.exe");

        if (!File.Exists(sevenZipExecutablePath))
            throw new FileNotFoundException(
                $"Could not find 7-Zip exe. Make sure it exists at: {sevenZipExecutablePath}");

        if (!File.Exists(archiveToInstallPath))
            throw new FileNotFoundException($"Could not find archive specified: {archiveToInstallPath}");
        
        var processStartInfo = new ProcessStartInfo()
        {
            Arguments = arguments,
            FileName = sevenZipExecutablePath
        };
    
        Process.Start(processStartInfo)?.WaitForExit();
    }
}