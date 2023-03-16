using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Model for deserializing JSON representing a portable application folder to copy according to the settings
/// stored in the JSON for that particular installer
/// </summary>
public partial class PortableApplicationInstaller : ObservableObject, IInstallable
{
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
    public string DisplayName { get; set; } = "";
    
    [ObservableProperty] private bool _isSelected;
    
    /// <inheritdoc/>
    public void ExecuteInstall(ILogger logger) 
    {
        var searchInPath =
            Path.Join(
                ApplicationPaths.SetupAssistantRootDir,
                "Resources",
                "Installers",
                "Portable Applications")
                .Replace("/", @"\");

        var folderToInstallPath =
            Path.Join(
                searchInPath,
                FolderName);
        
        folderToInstallPath = Path.GetFullPath(folderToInstallPath);
            
        Directory.CreateDirectory(
            Path.Join(DestinationPath, FolderName));
        
        logger.Debug("Copying portable app: {SourceFolderPath}", folderToInstallPath);
        logger.Debug("To directory: {DestinationFolder}", Path.Join(DestinationPath, FolderName));
        
        if (Directory.Exists(Path.Join(DestinationPath, FolderName)))
        {
            CopyFolderWithContents(folderToInstallPath, Path.Join(DestinationPath, FolderName), logger);
        }
    }
    
    private void CopyFolderWithContents(string sourcePath, string destinationPath, ILogger logger)
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

            try
            {
                File.Copy(sourceFilePath, newPath, true);    
            }
            catch (IOException)
            {logger.Warning("Could not copy file to: {newPath}", newPath);}
            
        }
    }
}