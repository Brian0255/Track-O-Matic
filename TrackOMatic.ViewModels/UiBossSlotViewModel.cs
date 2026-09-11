using CommunityToolkit.Mvvm.Input;

using Humanizer;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public partial class UiBossSlotViewModel(
    int slotNumber,
    IEndGameProgressionService endGameProgressionService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    ISavedProgressProvider savedProgressProvider,
    IItemSelectorService itemSelectorService,
    IDataSaverService dataSaverService
) : BossSlotViewModel(slotNumber, endGameProgressionService, parsedSpoilerDataService, savedProgressProvider, dataSaverService)
{
    private readonly IEndGameProgressionService _endGameProgressionService = endGameProgressionService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService = parsedSpoilerDataService;
    private readonly IItemSelectorService _itemSelectorService = itemSelectorService;
    private readonly int _slotNumber = slotNumber;

    #region Commands

    [RelayCommand(CanExecute = nameof(CanSelectItem))]
    private async Task SelectItem((double X, double Y) screenMousePosition)
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

    private bool CanSelectItem((double X, double Y) screenMousePosition) => CanCycle();

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CycleNext()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 1) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CyclePrevious()
    {
        var bossKongs = _endGameProgressionService.GetBossKongs();
        var oldValue = bossKongs[_slotNumber];
        bossKongs[_slotNumber] = (oldValue + 12) % 13; // Allow for the unknown kong.
        _endGameProgressionService.SetBossKongs(bossKongs);
    }

    private bool CanCycle() => !_parsedSpoilerDataService.HasBossOrder();

    #endregion

    #region Object Overrides

    public override string ToString()
    {
        return $"The {(_slotNumber + 1).Ordinalize()} final boss is {GetBossName()}.";
    }

    #endregion
}
