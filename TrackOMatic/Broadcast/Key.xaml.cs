using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;

/// <summary>
/// Interaction logic for Key.xaml
/// </summary>
public partial class Key : UserControl
{
    public static readonly DependencyProperty LevelNumberProperty = DependencyProperty.Register(
        nameof(LevelNumber),
        typeof(int),
        typeof(Key),
        new PropertyMetadata(-1, OnLevelNumberChanged)
    );

    public int LevelNumber
    {
        get => (int)GetValue(LevelNumberProperty);
        set => SetValue(LevelNumberProperty, value);
    }

    public Key()
    {
        InitializeComponent();
    }

    private static void OnLevelNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Key control && e.NewValue is int levelNumber)
        {
            // Create ViewModel with injected services
            var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
            var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
            var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
            if (itemTrackingService != null && parsedSpoilerDataService != null && savedProgressProvider != null)
            {
                control.DataContext = new KeyViewModel(levelNumber, itemTrackingService, parsedSpoilerDataService, savedProgressProvider);
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
