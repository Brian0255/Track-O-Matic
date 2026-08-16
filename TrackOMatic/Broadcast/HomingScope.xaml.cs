using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;

/// <summary>
/// Interaction logic for HomingScope.xaml
/// </summary>
public partial class HomingScope : UserControl
{
    public HomingScope()
    {
        InitializeComponent();

        // Set up the DataContext by obtaining services from the service locator
        // and creating an instance of the view model
        var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();

        if (itemTrackingService != null && parsedSpoilerDataService != null && savedProgressProvider != null)
        {
            DataContext = new HomingScopeViewModel(
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider
            );
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
