using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiBossSlotViewModel : BossSlotViewModel
{
    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IItemSelectorService _itemSelectorService;
    private readonly int _slotNumber;

    public IAsyncRelayCommand SelectItemCommand { get; init; }
    public IRelayCommand CycleNextCommand { get; init; }
    public IRelayCommand CyclePreviousCommand { get; init; }


    public UiBossSlotViewModel(
        int slotNumber,
        IEndGameProgressionService endGameProgressionService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService
    ) : base(slotNumber, endGameProgressionService, parsedSpoilerDataService, savedProgressProvider)
    {
        _slotNumber = slotNumber;
        _endGameProgressionService = endGameProgressionService;
        _itemSelectorService = itemSelectorService;

        SelectItemCommand = new AsyncRelayCommand<(double X, double Y)>(ExecuteSelectItem);
        CycleNextCommand = new RelayCommand(ExecuteCycleNext);
        CyclePreviousCommand = new RelayCommand(ExecuteCyclePrevious);
    }

    private async Task ExecuteSelectItem((double X, double Y) screenMousePosition)
    {
        var result = await _itemSelectorService.SelectItemAsync(
            _bossKongMappings.AsReadOnly(),
            "Select Phase/Boss",
            screenMousePosition.X,
            screenMousePosition.Y
        );

        if (result is null)
        {
            return;
        }

        var key = result.ResourceKey;
        var value = _bossKongMappings.FindIndex(k => k.ResourceKey == key);

        var bossKongs = _endGameProgressionService.GetBossKongs();
        bossKongs[_slotNumber] = value;
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    private void ExecuteCycleNext()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 1) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    private void ExecuteCyclePrevious()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 12) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }
}
