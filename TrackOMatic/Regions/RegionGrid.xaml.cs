using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for RegionGrid.xaml
/// </summary>
public partial class RegionGrid : UserControl
{
    public RegionGrid()
    {
        InitializeComponent();

        DataContext = new RegionGridViewModel(
            ServiceLocator.GetService<IItemTrackingService>(),
            ServiceLocator.GetService<IParsedSpoilerDataService>(),
            ServiceLocator.GetService<ISavedProgressProvider>(),
            ServiceLocator.GetService<IRegionPlacementOrchestrator>(),
            ServiceLocator.GetService<IThemeService>(),
            ServiceLocator.GetService<RegionSlotProviderRegistry>()
        );
    }
}
