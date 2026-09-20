using System.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public partial class UiCountableViewModel(
    ItemType itemType,
    ICollectiblesService collectiblesService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    IUserSettingsService userSettingsService
) : CountableViewModel(itemType, collectiblesService, parsedSpoilerDataService), INotifyPropertyChanged
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService
        ?? throw new ArgumentNullException(nameof(userSettingsService));
    private readonly ICollectiblesService _collectiblesService = collectiblesService
        ?? throw new ArgumentNullException(nameof(collectiblesService));
    private readonly ItemType _itemType = itemType;

    #region Commands

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void IncrementCount()
    {
        var count = _collectiblesService.GetCollectibleCount(_itemType);
        _collectiblesService.SetCollectibleCount(_itemType, ++count);
    }

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void DecrementCount()
    {
        var count = _collectiblesService.GetCollectibleCount(_itemType);
        _collectiblesService.SetCollectibleCount(_itemType, Math.Max(count - 1, 0));
    }

    private bool CanChangeCount() => !_userSettingsService.Autotracking;

    #endregion
}
