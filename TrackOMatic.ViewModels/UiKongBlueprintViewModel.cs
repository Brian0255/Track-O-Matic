using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public partial class UiKongBlueprintViewModel(
    ItemType collectedType,
    ItemType turnedInType,
    ICollectiblesService collectiblesService,
    IParsedSpoilerDataService parsedSpoilerDataService,
    ISavedProgressProvider savedProgressProvider,
    IUserSettingsService userSettingsService
) : KongBlueprintViewModel(collectedType, turnedInType, collectiblesService, parsedSpoilerDataService, savedProgressProvider), IEquatable<UiKongBlueprintViewModel>
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService
        ?? throw new ArgumentNullException(nameof(userSettingsService));
    private readonly ICollectiblesService _collectibles = collectiblesService
        ?? throw new ArgumentNullException(nameof(collectiblesService));
    private readonly ItemType _collectedType = collectedType;

    #region Commands

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void IncrementCount()
    {
        var old = _collectibles.GetCollectibleCount(_collectedType);
        _collectibles.SetCollectibleCount(_collectedType, int.Min(old + 1, 8));
    }

    [RelayCommand(CanExecute = nameof(CanChangeCount))]
    private void DecrementCount()
    {
        var old = _collectibles.GetCollectibleCount(_collectedType);
        _collectibles.SetCollectibleCount(_collectedType, int.Max(old - 1, 0));
    }

    private bool CanChangeCount()
    {
        return !_userSettingsService.Autotracking;
    }

    #endregion

    #region Object Overrides

    public bool Equals(UiKongBlueprintViewModel? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Equals(_collectedType, other._collectedType);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as UiKongBlueprintViewModel);
    }

    public override int GetHashCode()
    {
        return _collectedType.GetHashCode();
    }

    public override string ToString()
    {
        return $"UiKongBlueprintViewModel: {Count} of the {_collectedType} blueprints on hand.";
    }

    #endregion
}
