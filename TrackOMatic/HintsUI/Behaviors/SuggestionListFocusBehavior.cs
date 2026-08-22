using System.Windows;
using System.Windows.Controls;

namespace TrackOMatic.HintsUI.Behaviors;

/// <summary>
/// Attached behavior that focuses the first item in a ListBox when a ViewModel property changes.
/// This replaces ItemContainerGenerator access in code-behind.
///
/// Usage: behaviors:SuggestionListFocusBehavior.FocusFirstItem="{Binding ShouldFocusSuggestionsFirst}"
/// When ShouldFocusSuggestionsFirst becomes true, the first ListBoxItem will receive focus.
/// The behavior then resets the property to false.
/// </summary>
public static class SuggestionListFocusBehavior
{
    public static readonly DependencyProperty FocusFirstItemProperty =
        DependencyProperty.RegisterAttached(
            "FocusFirstItem",
            typeof(bool),
            typeof(SuggestionListFocusBehavior),
            new PropertyMetadata(false, OnFocusFirstItemChanged));

    public static bool GetFocusFirstItem(DependencyObject obj) =>
        (bool)obj.GetValue(FocusFirstItemProperty);

    public static void SetFocusFirstItem(DependencyObject obj, bool value) =>
        obj.SetValue(FocusFirstItemProperty, value);

    private static void OnFocusFirstItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ListBox listBox)
        {
            return;
        }

        // Only act when the property becomes true
        if (e.NewValue is true)
        {
            // Ensure layout is updated so ItemContainerGenerator can find items
            listBox.UpdateLayout();

            // Get the first item container
            var firstItem = listBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            if (firstItem != null)
            {
                firstItem.Focus();
            }
            else
            {
                // Fall back to focusing the ListBox itself if item container isn't ready
                listBox.Focus();
            }

            // Reset the flag to false after focusing
            // This allows the command to trigger focus again later
            listBox.Dispatcher.BeginInvoke(() =>
            {
                SetFocusFirstItem(listBox, false);
            });
        }
    }
}
