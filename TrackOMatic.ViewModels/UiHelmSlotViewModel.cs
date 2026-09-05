using CommunityToolkit.Mvvm.Input;

using Humanizer;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public partial class UiHelmSlotViewModel(
    int slotNumber,
    IEndGameProgressionService endGameProgressionService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    ISavedProgressProvider savedProgressProvider,
    IItemSelectorService itemSelectorService
) : HelmSlotViewModel(slotNumber, endGameProgressionService, parsedSpoilerDataService, savedProgressProvider)
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

    private bool CanSelectItem((double X, double Y) screenMousePosition) => CanCycle();

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CycleNext()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        var oldValue = helmKongs[_slotNumber];
        helmKongs[_slotNumber] = (oldValue + 1) % 6; // Allow for the unknown kong.
        _endGameProgressionService.SetHelmKongs(helmKongs);
    }

    [RelayCommand(CanExecute = nameof(CanCycle))]
    private void CyclePrevious()
    {
        var helmKongs = _endGameProgressionService.GetHelmKongs();
        var oldValue = helmKongs[_slotNumber];
        helmKongs[_slotNumber] = (oldValue + 5) % 6; // Allow for the unknown kong.
        _endGameProgressionService.SetHelmKongs(helmKongs);
    }

    private bool CanCycle() => !_parsedSpoilerDataService.HasHelmOrder();

    #endregion

    #region Object Overrides

    public override string ToString()
    {
        return $"The {(_slotNumber + 1).Ordinalize()} Kong to destroy the Blast-O-Matic is {GetHelmName()}.";
    }

    #endregion
}
