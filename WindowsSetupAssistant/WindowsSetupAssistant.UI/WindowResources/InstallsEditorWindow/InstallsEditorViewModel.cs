using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WindowsSetupAssistant.UI.WindowResources.InstallsEditorWindow;

/// <summary>
/// The ViewModel for the InstallsEditorWindow
/// </summary>
public partial class InstallsEditorViewModel : ObservableObject
{
    [ObservableProperty] private Visibility _isChocolateyInstallerVisible = Visibility.Hidden;
    [ObservableProperty] private Visibility _isExecutableInstallerVisible = Visibility.Hidden;
    [ObservableProperty] private Visibility _isPortableApplicationInstallerVisible = Visibility.Hidden;
    [ObservableProperty] private Visibility _isArchiveInstallerVisible = Visibility.Hidden;
}