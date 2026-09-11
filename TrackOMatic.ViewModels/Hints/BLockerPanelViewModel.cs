using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class BLockerPanelViewModel(
    string lineColor,
    IBarrierService barrierService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    ISavedProgressProvider savedProgressProvider,
    IItemSelectorService itemSelectorService,
    IDataSaverService dataSaverService
) : ObservableObject
{
    [ObservableProperty]
    public partial string LineColor { get; set; } = lineColor;

    public ObservableCollection<BLockerHintViewModel> Hints { get; init; } =
    [
        new(RegionName.JUNGLE_JAPES, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.FUNGI_FOREST, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.ANGRY_AZTEC, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.CRYSTAL_CAVES, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.FRANTIC_FACTORY, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.CREEPY_CASTLE, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.GLOOMY_GALLEON, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
        new(RegionName.HIDEOUT_HELM, barrierService, parsedSpoilerDataService, savedProgressProvider, itemSelectorService, dataSaverService),
    ];
}
