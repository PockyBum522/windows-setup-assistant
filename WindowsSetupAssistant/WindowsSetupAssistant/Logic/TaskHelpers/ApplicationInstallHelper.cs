using System;
using System.Diagnostics;
using System.IO;
using Serilog;

namespace WindowsSetupAssistant.Logic.TaskHelpers;

public class ApplicationInstallHelper
{
    private readonly ILogger _logger;

    public ApplicationInstallHelper(ILogger logger)
    {
        _logger = logger;
    }
    
    public void InstallTrueLaunchBar()
    {
        var trueLaunchBarPath =
            FileSearcher.ReverseWalkDirectoriesFind(ApplicationPaths.ThisApplicationRunFromDirectoryPath, "truelaunchbar8-free.exe", 8);

        var trueLaunchBarStartInfo = new ProcessStartInfo()
        {
            Arguments = "Arguments",
            CreateNoWindow = true,
            FileName = trueLaunchBarPath
        };

        Process.Start(trueLaunchBarStartInfo);
    }

    public void InstallArchiveFromPortableApplications(string archiveFileName)
    {
        // _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        //
        // Directory.CreateDirectory(ApplicationPaths.PortableAppsDestinationDirectory);
        //
        // var sevenZipExecutablePath =
        //     Path.Join(
        //         ApplicationPaths.PortableAppsSourceDirectory,
        //         "7-Zip",
        //         "7z.exe");
        //
        // var archiveToInstallPath =
        //     Path.Join(
        //         ApplicationPaths.PortableAppsSourceDirectory,
        //         archiveFileName);
        //
        // var arguments = $"e {archiveToInstallPath} -o\"{ApplicationPaths.PortableAppsDestinationDirectory}\" -r";
        //
        // _logger.Debug("Extracting Archive: {ArchiveToInstallPath}", archiveToInstallPath);
        // _logger.Debug("With arguments: {Args}", arguments);
        //
        // if (File.Exists(archiveToInstallPath) &&
        //     File.Exists(sevenZipExecutablePath))
        // {
        //     var processStartInfo = new ProcessStartInfo()
        //     {
        //         Arguments = arguments,
        //         FileName = sevenZipExecutablePath
        //     };
        //
        //     Process.Start(processStartInfo)?.WaitForExit();
        // }
    }
    
    public void InstallDirectoryFromPortableApplications(string folderName)
    {
        // var sourcePath =
        //     Path.Join(
        //         ApplicationPaths.PortableAppsSourceDirectory,
        //         folderName);
        //
        // var destinationPath =
        //     Path.Join(
        //         ApplicationPaths.PortableAppsDestinationDirectory,
        //         folderName);
        //
        // Directory.CreateDirectory(destinationPath);
        //
        // _logger.Debug("Copying portable app: {SourceFolderPath}", sourcePath);
        // _logger.Debug("To directory: {DestinationFolder}", destinationPath);
        //
        // if (Directory.Exists(sourcePath))
        // {
        //     CopyFolderWithContents(sourcePath, destinationPath);
        // }
    }
    
    public void InstallTeamViewer11()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        var assemblyPath = Path.GetDirectoryName(Environment.ProcessPath) ?? "";

        var teamViewerFullPath = FileSearcher.ReverseWalkDirectoriesFind(assemblyPath, "TeamViewer_11.exe", 6);

        var teamviewerProcessIsRunning = (Process.GetProcessesByName("teamviewer").Length > 0);

        if (teamviewerProcessIsRunning) return;
        
        _logger.Information("{Message}", $"Running TeamViewer Install: {teamViewerFullPath}");

        Process.Start(teamViewerFullPath);
    }
    
    private void CopyFolderWithContents(string sourcePath, string destinationPath)
    {
        //Create all the directories.
        foreach (var directoryPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            var newDirectoryPath = directoryPath.Replace(sourcePath, destinationPath);
            
            Directory.CreateDirectory(newDirectoryPath);
        }

        //Copy all the files & Replaces any files with the same name.
        foreach (var sourceFilePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            var newPath = sourceFilePath.Replace(sourcePath, destinationPath);
            
            File.Copy(sourceFilePath, newPath, true);
        }
    }
}