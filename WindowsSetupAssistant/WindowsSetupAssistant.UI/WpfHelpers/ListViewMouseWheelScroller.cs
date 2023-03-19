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
    /// <param name="mouseWheelEventArgs">Mouse wheel args</param>
    public static void OnPreviewMouseWheelMove(MouseWheelEventArgs mouseWheelEventArgs)
    {
        var listView = (ListView)mouseWheelEventArgs.Source;
        var scv = (ScrollViewer)listView.Parent;

        var scrollAmount = mouseWheelEventArgs.Delta / 2f;

        scv.ScrollToVerticalOffset(scv.VerticalOffset - scrollAmount);

        mouseWheelEventArgs.Handled = true;
    }
}