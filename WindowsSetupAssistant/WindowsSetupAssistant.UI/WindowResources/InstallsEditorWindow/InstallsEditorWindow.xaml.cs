using System.ComponentModel;
using System.Windows;

namespace WindowsSetupAssistant.UI.WindowResources.InstallsEditorWindow;

/// <summary>
/// The Installs Editor Window
/// </summary>
public partial class InstallsEditorWindow : Window
{
    /// <summary>
    /// Codebehind for the Installs Editor Window
    /// </summary>
    public InstallsEditorWindow(InstallsEditorViewModel installsEditorViewModel)
    {
        DataContext = installsEditorViewModel;
     
        
        InitializeComponent();
    }

    private void InstallsEditorWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        Hide();
        
        e.Cancel = true;
        
        ((InstallsEditorViewModel)DataContext).DeserializeInstallersJson();
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        Hide();
        
        ((InstallsEditorViewModel)DataContext).DeserializeInstallersJson();
    }

    private void SaveAll_OnClick(object sender, RoutedEventArgs e)
    {
        Hide();
        
        ((InstallsEditorViewModel)DataContext).SaveAllEditedInstallersToJsonFileCommand.Execute(null);
    }
}