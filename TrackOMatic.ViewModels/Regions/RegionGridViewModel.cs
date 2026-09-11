using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Regions;

public partial class RegionGridViewModel : ObservableObject
{
    private static readonly RegionName[] _displayRegions =
    [
        RegionName.START,
        RegionName.DK_ISLES,
        RegionName.JUNGLE_JAPES,
        RegionName.FUNGI_FOREST,
        RegionName.ANGRY_AZTEC,
        RegionName.CRYSTAL_CAVES,
        RegionName.FRANTIC_FACTORY,
        RegionName.CREEPY_CASTLE,
        RegionName.GLOOMY_GALLEON,
        RegionName.HIDEOUT_HELM
    ];

    public ObservableCollection<RegionViewModel> Regions { get; } = [];

    public RegionGridViewModel(
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IRegionPlacementOrchestrator orchestrator,
        IThemeService themeService,
        RegionSlotProviderRegistry regionSlotProviderRegistry,
        IAutotrackerService autotrackerService,
        IDataSaverService dataSaverService
    )
    {
        foreach (var regionName in _displayRegions)
        {
            Regions.Add(new(
                regionName,
                itemTrackingService,
                parsedSpoilerDataService,
                savedProgressProvider,
                orchestrator,
                themeService,
                regionSlotProviderRegistry,
                autotrackerService,
                dataSaverService
            ));
        }
    }
}
