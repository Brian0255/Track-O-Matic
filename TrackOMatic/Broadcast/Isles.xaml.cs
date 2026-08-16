using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;

using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;

/// <summary>
/// Interaction logic for Isles.xaml
/// </summary>
public partial class Isles : UserControl
{
    public Isles()
    {
        InitializeComponent();

        // Create ViewModel with injected services
        var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
        var levelOrderService = ServiceLocator.GetService<ILevelOrderService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();
        if (itemTrackingService != null && levelOrderService != null && parsedSpoilerDataService != null && userSettingsService != null)
        {
            DataContext = new IslesViewModel(itemTrackingService, levelOrderService, parsedSpoilerDataService, userSettingsService);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
