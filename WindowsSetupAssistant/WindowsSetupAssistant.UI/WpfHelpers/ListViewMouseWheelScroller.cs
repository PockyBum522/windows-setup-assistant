using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsSetupAssistant.UI.WpfHelpers;

/// <summary>
/// Helpers for WPF controls
/// </summary>
public static class ListViewMouseWheelScroller
{
    /// <summary>
    /// Handles the MainWindow scrolling with mouse wheel for any scrollviewers with listviews inside them
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