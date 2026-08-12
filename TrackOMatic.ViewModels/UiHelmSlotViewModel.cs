using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiHelmSlotViewModel : HelmSlotViewModel
{
    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly int _slotNumber;

    public IAsyncRelayCommand SelectItemCommand { get; init; }
    public IRelayCommand CycleNextCommand { get; init; }
    public IRelayCommand CyclePreviousCommand { get; init; }


    public UiHelmSlotViewModel(
        int slotNumber,
        IEndGameProgressionService endGameProgressionService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService
    ) : base(slotNumber, endGameProgressionService, parsedSpoilerDataService, savedProgressProvider)
    {
        _slotNumber = slotNumber;
        _endGameProgressionService = endGameProgressionService;
        _parsedSpoilerDataService = parsedSpoilerDataService;
        _itemSelectorService = itemSelectorService;

        SelectItemCommand = new AsyncRelayCommand<(double X, double Y)>(ExecuteSelectItem, CanExecuteSelectItem);
        CycleNextCommand = new RelayCommand(ExecuteCycleNext, CanExecuteCycleNext);
        CyclePreviousCommand = new RelayCommand(ExecuteCyclePrevious, CanExecuteCyclePrevious);
    }

    private async Task ExecuteSelectItem((double X, double Y) screenMousePosition)
    {
        var result = await _itemSelectorService.SelectItemAsync(
            _helmKongMappings.AsReadOnly(),
            "Select Kong",
            screenMousePosition.X,
            screenMousePosition.Y
        );

        if (result is null)
        {
            return;
        }

        var key = result.ResourceKey;
        var value = _helmKongMappings.FindIndex(k => k.ResourceKey == key);

        var helmKongs = _endGameProgressionService.GetHelmKongs();
        helmKongs[_slotNumber] = value;
        _endGameProgressionService.SetHelmKongs(helmKongs);
    }

    private bool CanExecuteSelectItem((double X, double Y) screenMousePosition) => !_parsedSpoilerDataService.HasHelmOrder();

    private void ExecuteCycleNext()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        var oldValue = helmKongs[_slotNumber];
        helmKongs[_slotNumber] = (oldValue + 1) % 6; // Allow for the unknown kong.
        _endGameProgressionService.SetHelmKongs(helmKongs);
    }

    private bool CanExecuteCycleNext() => !_parsedSpoilerDataService.HasHelmOrder();

    private void ExecuteCyclePrevious()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        var oldValue = helmKongs[_slotNumber];
        helmKongs[_slotNumber] = (oldValue + 5) % 6; // Allow for the unknown kong.
        _endGameProgressionService.SetHelmKongs(helmKongs);
    }

    private bool CanExecuteCyclePrevious() => !_parsedSpoilerDataService.HasHelmOrder();
}
