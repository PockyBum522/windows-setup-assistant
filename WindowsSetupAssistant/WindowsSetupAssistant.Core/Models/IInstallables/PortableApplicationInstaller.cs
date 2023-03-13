﻿using System.IO;
using Serilog;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Model for deserializing JSON representing a portable application folder to copy according to the settings
/// stored in the JSON for that particular installer
/// </summary>
public class PortableApplicationInstaller : BaseInstaller
{
    private ILogger _logger;
    
    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public PortableApplicationInstaller(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Name of the folder containing the portable application, must be located in \Resources\Portable Applications\
    /// </summary>
    public string FolderName { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";
    
    /// <summary>
    /// If user wants to create a desktop shortcut, what exe should that shortcut point to? Blank or whitespace
    /// if no shortcut desired
    /// </summary>
    public string DesktopShortcutExePath { get; set; } = "";
    
    /// <summary>
    /// If user wants to create a Start Menu shortcut, what exe should that shortcut point to? Blank or whitespace
    /// if no shortcut desired
    /// </summary>
    public string StartMenuShortcutExePath { get; set; } = "";
    
    /// <inheritdoc/>
    public override void ExecuteInstall()
    {
        var searchInPath =
            Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "WindowsSetupAssistant",
                "Resources",
                "Portable Applications");
        
        var folderToInstallPath =
            Path.Join(
                searchInPath,
                FolderName);
        
        Directory.CreateDirectory(DestinationPath);
        
        _logger.Debug("Copying portable app: {SourceFolderPath}", folderToInstallPath);
        _logger.Debug("To directory: {DestinationFolder}", DestinationPath);
        
        if (Directory.Exists(folderToInstallPath))
        {
            CopyFolderWithContents(folderToInstallPath, DestinationPath);
        }
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