using System.Windows;
using System.Windows.Controls;

namespace TrackOMatic.HintsUI.Behaviors;

/// <summary>
/// Attached behavior that syncs a control's IsKeyboardFocusWithin property to a ViewModel property.
/// This allows XAML binding to track whether a control has keyboard focus.
///
/// Usage: behaviors:TrackFocusWithinBehavior.IsFocusedWithin="{Binding IsSuggestionBoxFocused, Mode=OneWayToSource}"
/// The behavior will sync the control's IsKeyboardFocusWithin to the ViewModel property.
/// </summary>
public static class TrackFocusWithinBehavior
{
    public static readonly DependencyProperty IsFocusedWithinProperty =
        DependencyProperty.RegisterAttached(
            "IsFocusedWithin",
            typeof(bool),
            typeof(TrackFocusWithinBehavior),
            new PropertyMetadata(false, OnIsFocusedWithinChanged));

    public static bool GetIsFocusedWithin(DependencyObject obj) =>
        (bool)obj.GetValue(IsFocusedWithinProperty);

    public static void SetIsFocusedWithin(DependencyObject obj, bool value) =>
        obj.SetValue(IsFocusedWithinProperty, value);

    private static void OnIsFocusedWithinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement element)
        {
            return;
        }

        // When the attached property is set, start syncing the IsKeyboardFocusWithin state
        if (e.NewValue is true)
        {
            // Set up a binding listener by using a timer to check the focus state
            // WPF doesn't expose IsKeyboardFocusWithin as an event, so we poll it
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // Check every 50ms
            };

            timer.Tick += (sender, args) =>
            {
                SetIsFocusedWithin(element, element.IsKeyboardFocusWithin);
            };

            timer.Start();

            // Store the timer on the element so it doesn't get garbage collected
            element.Tag = timer;
        }
    }
}


