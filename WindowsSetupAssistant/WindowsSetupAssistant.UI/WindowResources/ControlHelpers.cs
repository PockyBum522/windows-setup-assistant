using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsSetupAssistant.UI.WindowResources;

/// <summary>
/// Helpers for WPF controls
/// </summary>
public static class ControlHelpers
{
    /// <summary>
    /// Handles the MainWindow scrolling with mouse wheel for Available Installs list
    /// </summary>
    /// <param name="sender">ListView to be scrolled</param>
    /// <param name="e">Mouse wheel args</param>
    public static void OnPreviewMouseWheelMove(object sender, MouseWheelEventArgs e)
    {
        var listView = (ListView)sender;
        var scv = (ScrollViewer)listView.Parent;

        var scrollAmount = e.Delta / 2f;

        scv.ScrollToVerticalOffset(scv.VerticalOffset - scrollAmount);

        e.Handled = true;
    }
}