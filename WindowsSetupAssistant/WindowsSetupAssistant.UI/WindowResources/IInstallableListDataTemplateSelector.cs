using System.Windows;
using System.Windows.Controls;
using WindowsSetupAssistant.Core.Models.IInstallables;

namespace WindowsSetupAssistant.UI.WindowResources;

public class IInstallableListDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate SeparatorDisplayTemplate { get; set; }
    public DataTemplate IInstallableDisplayTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is SeparatorForInstallersList)
        {
            return SeparatorDisplayTemplate;
        }
        else 
        {
            return IInstallableDisplayTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}