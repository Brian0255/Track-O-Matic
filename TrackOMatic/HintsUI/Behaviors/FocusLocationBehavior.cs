using System.Windows;
using System.Windows.Controls;

namespace TrackOMatic.HintsUI.Behaviors;

/// <summary>
/// Attached behavior that focuses a TextBox when a ViewModel property changes.
/// Used specifically for the Location field to replace x:Name dependency.
///
/// Usage: behaviors:FocusLocationBehavior.FocusRequest="{Binding ShouldFocusLocation}"
/// When ShouldFocusLocation becomes true, the TextBox will automatically focus.
/// The behavior then resets the property to false.
/// </summary>
public static class FocusLocationBehavior
{
    public static readonly DependencyProperty FocusRequestProperty =
        DependencyProperty.RegisterAttached(
            "FocusRequest",
            typeof(bool),
            typeof(FocusLocationBehavior),
            new PropertyMetadata(false, OnFocusRequestChanged));

    public static bool GetFocusRequest(DependencyObject obj) =>
        (bool)obj.GetValue(FocusRequestProperty);

    public static void SetFocusRequest(DependencyObject obj, bool value) =>
        obj.SetValue(FocusRequestProperty, value);

    private static void OnFocusRequestChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        // Only act when the property becomes true
        if (e.NewValue is true)
        {
            textBox.Focus();

            // Reset the flag to false after focusing
            // This allows the command to trigger focus again later
            textBox.Dispatcher.BeginInvoke(() =>
            {
                SetFocusRequest(textBox, false);
            });
        }
    }
}
