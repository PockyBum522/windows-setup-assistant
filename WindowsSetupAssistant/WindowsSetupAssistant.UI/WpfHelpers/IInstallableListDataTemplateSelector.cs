using System.Windows;
using System.Windows.Controls;
using WindowsSetupAssistant.Core.Models.IInstallables;

namespace WindowsSetupAssistant.UI.WpfHelpers;

/// <summary>
/// Selector to template separators differently from other IInstallables when displaying in listViews 
/// </summary>
public class IInstallableListDataTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// DataTemplate to use when displaying selectors
    /// </summary>
    public DataTemplate SeparatorDisplayTemplate { get; set; } = new();
    
    /// <summary>
    /// DataTemplate to use when displaying any other IInstallable
    /// </summary>
    public DataTemplate IInstallableDisplayTemplate { get; set; } = new();

    /// <summary>
    /// Does the actual template selecting
    /// </summary>
    /// <param name="item"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is not SeparatorForInstallersList) return IInstallableDisplayTemplate;
        
        // Otherwise:
        return SeparatorDisplayTemplate;
    }
}