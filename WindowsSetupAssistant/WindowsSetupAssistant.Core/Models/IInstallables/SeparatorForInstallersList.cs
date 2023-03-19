using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.Core.Models.IInstallables;

/// <summary>
/// Contains data for an archive in the \Resources\Portable Applications\ folder
/// Mostly what to show for the display name and where to install it if the user selects it
/// </summary>
public partial class SeparatorForInstallersList : ObservableObject, IInstallable
{
    [ObservableProperty] private string _displayName = "-----------------------";

    [ObservableProperty] private bool _isSelected;

    /// <summary>
    /// Does nothing. This is a separator. For visual organization only 
    /// </summary>
    /// <param name="logger"></param>
    public void ExecuteInstall(ILogger logger) { }
}