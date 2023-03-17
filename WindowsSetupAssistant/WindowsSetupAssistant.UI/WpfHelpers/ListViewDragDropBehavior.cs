using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WindowsSetupAssistant.Core.Models.IInstallables.Interfaces;

namespace WindowsSetupAssistant.UI.WpfHelpers;

/// <summary>
/// Adds drag and drop reordering of items to a ListView
/// </summary>
public static class ListViewDragDropBehavior
{
    private static ListViewItem? _draggedItem;

    /// <summary>
    /// Gets the value of the IsEnabled attached property.
    /// </summary>
    public static bool GetIsEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Sets the value of the IsEnabled attached property.
    /// </summary>
    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    /// <summary>
    /// The IsEnabled DependencyProperty that enables or disables the drag and drop behavior.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListViewDragDropBehavior), new UIPropertyMetadata(false, OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is ListView listView)) return;

        listView.PreviewMouseLeftButtonDown += ListView_PreviewMouseLeftButtonDown;
        listView.Drop += ListView_Drop;
    }

    private static void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!(sender is ListView listView)) return;

        _draggedItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

        if (_draggedItem == null) return;

        DragDrop.DoDragDrop(listView, _draggedItem.DataContext, DragDropEffects.Move);
    }

    private static void ListView_Drop(object sender, DragEventArgs e)
    {
        if (!(sender is ListView listView)) return;

        var targetItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

        if (_draggedItem == null || targetItem == null) return;

        var oldIndex = listView.Items.IndexOf(_draggedItem.DataContext);
        var newIndex = listView.Items.IndexOf(targetItem.DataContext);

        if (oldIndex == newIndex) return;

        var itemsSource = listView.ItemsSource as ObservableCollection<IInstallable>;

        if (itemsSource == null) return;

        itemsSource.Move(oldIndex, newIndex);
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
        if (current == null) return null;

        do
        {
            if (current is T)
            {
                return (T)current;
            }
            current = VisualTreeHelper.GetParent(current);
        }
        while (current != null);

        return null;
    }
}