using CommunityToolkit.Mvvm.Input;

using Humanizer;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiBossSlotViewModel : BossSlotViewModel
{
    private readonly IEndGameProgressionService _endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
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
        _parsedSpoilerDataService = parsedSpoilerDataService;
        _itemSelectorService = itemSelectorService;

        SelectItemCommand = new AsyncRelayCommand<(double X, double Y)>(ExecuteSelectItem, CanExecuteSelectItem);
        CycleNextCommand = new RelayCommand(ExecuteCycleNext, CanExecuteCycleNext);
        CyclePreviousCommand = new RelayCommand(ExecuteCyclePrevious, CanExecuteCyclePrevious);
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

    private bool CanExecuteSelectItem((double X, double Y) screenMousePosition) => !_parsedSpoilerDataService.HasBossOrder();

    private void ExecuteCycleNext()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 1) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    private bool CanExecuteCycleNext() => !_parsedSpoilerDataService.HasBossOrder();

    private void ExecuteCyclePrevious()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 12) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    private bool CanExecuteCyclePrevious() => !_parsedSpoilerDataService.HasBossOrder();

    #region Object Overrides

    public override string ToString()
    {
        return $"The {(_slotNumber + 1).Ordinalize()} final boss is {GetBossName()}.";
    }

    #endregion
}
