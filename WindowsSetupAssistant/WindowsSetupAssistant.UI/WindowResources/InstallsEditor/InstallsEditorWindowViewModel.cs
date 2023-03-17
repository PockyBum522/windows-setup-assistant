using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Core;
using WindowsSetupAssistant.Core.Models.IInstallables;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.UI.WindowResources.InstallsEditor;

/// <summary>
/// The ViewModel for the InstallsEditorWindow
/// </summary>
public partial class InstallsEditorWindowViewModel : ObservableObject
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    public InstallsEditorWindowViewModel(ILogger logger)
    {
        _logger = logger;

        DeserializeInstallersJson();
    }
    
    [ObservableProperty] private ObservableCollection<IInstallable> _availableInstallersInJson = new();

    [ObservableProperty] private string _textDisplayName = "";
    [ObservableProperty] private string _textChocolateyId = "";
    [ObservableProperty] private string _textChocolateyArguments = "";
    [ObservableProperty] private string _textChocolateyParameters = "";

    [ObservableProperty] private string _textExecutableInstallerFileName = "";
    [ObservableProperty] private string _textExecutableInstallerArguments = "";

    [ObservableProperty] private string _textPortableInstallerFolderName = "";
    [ObservableProperty] private string _textPortableInstallerDestinationPath = "";

    [ObservableProperty] private string _textArchiveInstallerFileName = "";
    [ObservableProperty] private string _textArchiveInstallerDestinationPath = "";
    
    [RelayCommand]
    private void SaveCurrentlySelectedInstallerData()
    {
        if (SelectedInstaller is null) return;
        
        SelectedInstaller.DisplayName = TextDisplayName;

        if (SelectedInstaller is ChocolateyInstaller)
        {
            var convertedInstaller = (ChocolateyInstaller)SelectedInstaller;

            convertedInstaller.ChocolateyId = TextChocolateyId;
            convertedInstaller.Arguments = TextChocolateyArguments;
            convertedInstaller.Parameters = TextChocolateyParameters;
        }
        
        if (SelectedInstaller is ExecutableInstaller)
        {
            var convertedInstaller = (ExecutableInstaller)SelectedInstaller;

            convertedInstaller.FileName = TextExecutableInstallerFileName;
            convertedInstaller.Arguments = TextExecutableInstallerArguments;
        }
        
        if (SelectedInstaller is PortableApplicationInstaller)
        {
            var convertedInstaller = (PortableApplicationInstaller)SelectedInstaller;

            convertedInstaller.FolderName = TextPortableInstallerFolderName;
            convertedInstaller.DestinationPath = TextPortableInstallerDestinationPath;
        }
        
        if (SelectedInstaller is ArchiveInstaller)
        {
            var convertedInstaller = (ArchiveInstaller)SelectedInstaller;

            convertedInstaller.ArchiveFilename = TextArchiveInstallerFileName;
            convertedInstaller.DestinationPath = TextArchiveInstallerDestinationPath;
        }
        
        SaveAllEditedInstallersToJsonFile();

        IsChocolateyInstallerChecked = false;
        IsExecutableInstallerChecked = false;
        IsArchiveInstallerChecked = false;
        IsPortableApplicationInstallerChecked = false;
        IsAddSeparatorChecked = false;
    }
    
    [RelayCommand]
    private void CancelEditingInstallerData()
    {
        TextDisplayName = "";

        TextChocolateyId = "";
        TextChocolateyArguments = "";
        TextChocolateyParameters = "";
        
        TextExecutableInstallerFileName = "";
        TextExecutableInstallerArguments = "";
        
        TextPortableInstallerFolderName = "";
        TextPortableInstallerDestinationPath = "";
        
        TextArchiveInstallerFileName = "";
        TextArchiveInstallerDestinationPath = "";
        
        IsChocolateyInstallerChecked = false;
        IsExecutableInstallerChecked = false;
        IsArchiveInstallerChecked = false;
        IsPortableApplicationInstallerChecked = false;
        IsAddSeparatorChecked = false;
    }
    
    [RelayCommand]
    private void SaveAllEditedInstallersToJsonFile()
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
        
        using var jsonStateFileWriter = new StreamWriter(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);
        
        using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };
        
        serializer.Serialize(jsonStateWriter, AvailableInstallersInJson);
    }

    [RelayCommand]
    private void AddInstaller()
    {
        if (string.IsNullOrWhiteSpace(TextDisplayName))
        {
            MessageBox.Show("Display name textbox cannot be blank when adding an installer");
        }
        else if (ChocolateyInstallerRequirementsMet())
        {
            AvailableInstallersInJson.Add(
                new ChocolateyInstaller()
                {
                    DisplayName = TextDisplayName,
                    ChocolateyId = TextChocolateyId,
                    Arguments = TextChocolateyArguments,
                    Parameters = TextChocolateyParameters
                });
        } 
        else if (ExecutableInstallerRequirementsMet())
        {
            AvailableInstallersInJson.Add(
                new ExecutableInstaller()
                {
                    DisplayName = TextDisplayName,
                    Arguments = TextExecutableInstallerArguments,
                    FileName = TextExecutableInstallerFileName
                });
        }
        else if (PortableApplicationInstallerRequirementsMet())
        {
            AvailableInstallersInJson.Add(
                new PortableApplicationInstaller()
                {
                    DisplayName = TextDisplayName,
                    DestinationPath = TextPortableInstallerDestinationPath,
                    FolderName = TextPortableInstallerFolderName
                });
        }
        else if (ArchiveInstallerRequirementsMet())
        {
            AvailableInstallersInJson.Add(
                new ArchiveInstaller()
                {
                    DisplayName = TextDisplayName,
                    DestinationPath = TextPortableInstallerDestinationPath,
                    ArchiveFilename = TextArchiveInstallerFileName
                });
        }
        else if (IsAddSeparatorChecked)
        {           
            if (SelectedInstaller is null) return;
            
            AvailableInstallersInJson.Insert(
                AvailableInstallersInJson.IndexOf(SelectedInstaller),
                new SeparatorForInstallersList()
                {
                    DisplayName = TextDisplayName
                });
        }
        else
        {
            MessageBox.Show("One or more required textboxes (Textboxes marked with a *) are blank when trying to add this installer");
        }
    }

    [RelayCommand]
    private void RemoveSelectedInstaller()
    {
        for (var i = 0; i < AvailableInstallersInJson.Count; i++)
        {
            if (SelectedInstaller?.DisplayName != AvailableInstallersInJson[i].DisplayName) continue;
            
            // Otherwise:
            AvailableInstallersInJson.RemoveAt(i);
            break;
        }
    }
    
    /// <summary>
    /// Bound to the ChocolateyInstaller radio button
    /// </summary>
    public bool IsChocolateyInstallerChecked
    {
        get => IsChocolateyInstallerVisible == Visibility.Visible;
        set
        {
            OnPropertyChanging();
            IsChocolateyInstallerVisible = value ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Bound to the ExecutableInstaller radio button
    /// </summary>
    public bool IsExecutableInstallerChecked
    {
        get => IsExecutableInstallerVisible == Visibility.Visible;
        set
        {
            OnPropertyChanging();
            IsExecutableInstallerVisible = value ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Bound to the PortableApplicationInstaller radio button
    /// </summary>
    public bool IsPortableApplicationInstallerChecked
    {
        get => IsPortableApplicationInstallerVisible == Visibility.Visible;
        set
        {
            OnPropertyChanging();
            IsPortableApplicationInstallerVisible = value ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Bound to the ArchiveInstaller radio button
    /// </summary>
    public bool IsArchiveInstallerChecked
    {
        get => IsArchiveInstallerVisible == Visibility.Visible;
        set
        {
            OnPropertyChanging();
            IsArchiveInstallerVisible = value ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Bound to the Separator radio button
    /// </summary>
    public bool IsAddSeparatorChecked
    {
        get => IsAddSeparatorVisible == Visibility.Visible;
        set
        {
            OnPropertyChanging();
            IsAddSeparatorVisible = value ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Bound to the ArchiveInstaller radio button
    /// </summary>
    public IInstallable? SelectedInstaller
    {
        get => _selectedInstaller;
        set
        {
            OnPropertyChanging();
            
            if (value is null) return;
            
            _selectedInstaller = value;
            SetTextBoxesToInstallerProperties(value);
            OnPropertyChanged();
        }
    }

    private void SetTextBoxesToInstallerProperties(IInstallable? installable)
    {
        if (installable is null || installable is SeparatorForInstallersList)
        {
            TextDisplayName = "";
            return;
        }

        if (installable is ChocolateyInstaller)
        {
            IsChocolateyInstallerChecked = true;
            
            var convertedInstaller = (ChocolateyInstaller)installable;

            TextDisplayName = installable.DisplayName;
            TextChocolateyId = convertedInstaller.ChocolateyId;
            TextChocolateyArguments = convertedInstaller.Arguments;
            TextChocolateyParameters = convertedInstaller.Parameters;
        }
        
        if (installable is ExecutableInstaller)
        {
            IsExecutableInstallerChecked = true;
            
            var convertedInstaller = (ExecutableInstaller)installable;
            
            TextDisplayName = installable.DisplayName;
            TextExecutableInstallerFileName = convertedInstaller.FileName;
            TextExecutableInstallerArguments = convertedInstaller.Arguments;
        }
        
        if (installable is PortableApplicationInstaller)
        {
            IsPortableApplicationInstallerChecked = true;
            
            var convertedInstaller = (PortableApplicationInstaller)installable;

            TextDisplayName = installable.DisplayName;
            TextPortableInstallerFolderName = convertedInstaller.FolderName;
            TextPortableInstallerDestinationPath = convertedInstaller.DestinationPath;
        }
        
        if (installable is ArchiveInstaller)
        {
            IsArchiveInstallerChecked = true;
            
            var convertedInstaller = (ArchiveInstaller)installable;

            TextDisplayName = installable.DisplayName;
            TextArchiveInstallerFileName = convertedInstaller.ArchiveFilename;
            TextArchiveInstallerDestinationPath = convertedInstaller.DestinationPath;
        }
    }

    [ObservableProperty] private Visibility _isChocolateyInstallerVisible = Visibility.Collapsed;
    [ObservableProperty] private Visibility _isExecutableInstallerVisible = Visibility.Collapsed;
    [ObservableProperty] private Visibility _isPortableApplicationInstallerVisible = Visibility.Collapsed;
    [ObservableProperty] private Visibility _isArchiveInstallerVisible = Visibility.Collapsed;
    [ObservableProperty] private Visibility _isAddSeparatorVisible = Visibility.Collapsed;

    private IInstallable _selectedInstaller = new ChocolateyInstaller(); // New, just so it's not null
    
    /// <summary>
    /// Deserializes the installers in Available Installs.json and puts them in the AvailableInstallersInJson property
    /// </summary>
    /// <exception cref="JsonSerializationException">Throws if deserialization fails</exception>
    [SupportedOSPlatform("Windows7.0")]
    public void DeserializeInstallersJson()
    {
        var availableInstallsJsonRaw = File.ReadAllText(ApplicationPaths.ResourcePaths.InstallsFileJsonPath);

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        _logger.Debug("About to deserialize Available Installs.json");
        
        var availableInstalls =
            JsonConvert.DeserializeObject<List<IInstallable>>(availableInstallsJsonRaw, jsonSerializerSettings);

        if (availableInstalls is null) throw new JsonSerializationException();

        _logger.Information("Clearing AvailableInstallersInJson");
        AvailableInstallersInJson.Clear();

        foreach (var availableInstall in availableInstalls)
        {
            _logger.Debug("Got {InstallName} in deserialization, adding install to AvailableInstallersInJson", availableInstall.DisplayName);
            
            AvailableInstallersInJson.Add(availableInstall);
        }
    }
    
    [RelayCommand]
    private void CloseEditorWindow(object parameter)
    {
        if (parameter is Window window)
        {
            window.Close();
        }
    }
    
    private bool ArchiveInstallerRequirementsMet()
    {
        return IsArchiveInstallerChecked &&
               !string.IsNullOrWhiteSpace(TextArchiveInstallerFileName) &&
               !string.IsNullOrWhiteSpace(TextArchiveInstallerDestinationPath);
    }

    private bool PortableApplicationInstallerRequirementsMet()
    {
        return IsPortableApplicationInstallerChecked &&
               string.IsNullOrWhiteSpace(TextPortableInstallerFolderName) &&
               string.IsNullOrWhiteSpace(TextPortableInstallerDestinationPath);
    }

    private bool ExecutableInstallerRequirementsMet()
    {
        return IsExecutableInstallerChecked &&
               !string.IsNullOrWhiteSpace(TextExecutableInstallerFileName);
    }

    private bool ChocolateyInstallerRequirementsMet()
    {
        return IsChocolateyInstallerChecked &&
               !string.IsNullOrWhiteSpace(TextChocolateyId);
    }
}