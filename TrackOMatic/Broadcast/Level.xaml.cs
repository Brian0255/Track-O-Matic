using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;

/// <summary>
/// Interaction logic for Level.xaml
/// </summary>
public partial class Level : UserControl
{
    public static readonly DependencyProperty LevelNumberProperty = DependencyProperty.Register(
        nameof(LevelNumber),
        typeof(int),
        typeof(Level),
        new PropertyMetadata(-1, OnLevelNumberChanged)
    );

    public int LevelNumber
    {
        get => (int)GetValue(LevelNumberProperty);
        set => SetValue(LevelNumberProperty, value);
    }

    public Level()
    {
        InitializeComponent();
    }

    private static void OnLevelNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Level control && e.NewValue is int levelNumber)
        {
            // Create ViewModel with injected services
            var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
            var levelOrderService = ServiceLocator.GetService<ILevelOrderService>();
            var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
            var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();
            if (itemTrackingService != null && levelOrderService != null && parsedSpoilerDataService != null && userSettingsService != null)
            {
                control.DataContext = new LevelViewModel(levelNumber, itemTrackingService, levelOrderService, parsedSpoilerDataService, userSettingsService);
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
