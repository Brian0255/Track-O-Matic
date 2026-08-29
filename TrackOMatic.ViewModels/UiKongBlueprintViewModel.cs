using System.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiKongBlueprintViewModel: KongBlueprintViewModel, IEquatable<UiKongBlueprintViewModel>, INotifyPropertyChanged
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly ICollectiblesService _collectibles;
    private readonly ItemType _collectedType;

    public UiKongBlueprintViewModel(
        ItemType collectedType,
        ItemType turnedInType,
        ICollectiblesService collectiblesService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IUserSettingsService userSettingsService
    ) : base(collectedType, turnedInType, collectiblesService, parsedSpoilerDataService, savedProgressProvider)
    {
        _collectedType = collectedType;
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _collectibles = collectiblesService ?? throw new ArgumentNullException(nameof(collectiblesService));
    }

    public void IncrementCount()
    {
        if (!_userSettingsService.Autotracking)
        {
            var old = _collectibles.GetCollectibleCount(_collectedType);
            _collectibles.SetCollectibleCount(_collectedType, int.Min(old + 1, 8));
        }
    }

    public void DecrementCount()
    {
        if (!_userSettingsService.Autotracking)
        {
            var old = _collectibles.GetCollectibleCount(_collectedType);
            _collectibles.SetCollectibleCount(_collectedType, int.Max(old - 1, 0));
        }
    }

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
