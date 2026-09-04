using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HelmDoorPanel : UserControl
{
    public HelmDoorPanel()
    {
        InitializeComponent();

        var barrierService = ServiceLocator.GetService<IBarrierService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var itemSelectorService = ServiceLocator.GetService<IItemSelectorService>();
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();

        DataContext = new HelmDoorPanelViewModel(
            "#7d3cbd",
            userSettingsService,
            barrierService,
            savedProgressProvider,
            itemSelectorService
        );
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
    }
}
