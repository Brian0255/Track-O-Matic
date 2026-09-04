using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class BLockerPanel : UserControl
{
    public BLockerPanel()
    {
        InitializeComponent();

        var barrierService = ServiceLocator.GetService<IBarrierService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var itemSelectorService = ServiceLocator.GetService<IItemSelectorService>();

        DataContext = new BLockerPanelViewModel(
            "#f0f000",
            barrierService,
            parsedSpoilerDataService,
            savedProgressProvider,
            itemSelectorService
        );
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
    }
}
