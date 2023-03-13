using System;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains data for an archive in the \Resources\Portable Applications\ folder
/// Mostly what to show for the display name and where to install it if the user selects it
/// </summary>
public partial class ArchiveInstaller : ObservableObject, IInstallable
{
    /// <summary>
    /// Path to the folder containing the portable application
    /// </summary>
    public string ArchiveFilename { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";

    /// <inheritdoc/>
    public string DisplayName { get; set; } = "";

    [ObservableProperty] private bool _isSelected;

    /// <inheritdoc/>
    public void ExecuteInstall(ILogger logger)
    {        
        logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        Directory.CreateDirectory(Path.GetDirectoryName(DestinationPath) ?? throw new DirectoryNotFoundException());
        
        var searchInPath =
            Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Installers",
                "Installer Archives")
            .Replace("/", @"\");
        
        var archiveToInstallPath =
            Path.Join(
                searchInPath,
                ArchiveFilename);
        
        archiveToInstallPath = Path.GetFullPath(archiveToInstallPath);
        
        var arguments = $"""e "{archiveToInstallPath}" -o"{DestinationPath}" -r -y""";
        
        logger.Debug("Extracting Archive: {ArchiveToInstallPath}", archiveToInstallPath);
        logger.Debug("With arguments: {Args}", arguments);
        
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