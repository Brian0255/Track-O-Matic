using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HelmDoorPanelViewModel(
    IBarrierService barrierService,
    ISavedProgressProvider savedProgressProvider,
    IItemSelectorService itemSelectorService
    ) : ObservableObject
{
    public HelmDoorHintViewModel FirstDoor { get; } = new HelmDoorHintViewModel(HelmDoor.First, barrierService, savedProgressProvider, itemSelectorService);
    public HelmDoorHintViewModel SecondDoor { get; } = new HelmDoorHintViewModel(HelmDoor.Second, barrierService, savedProgressProvider, itemSelectorService);
}
