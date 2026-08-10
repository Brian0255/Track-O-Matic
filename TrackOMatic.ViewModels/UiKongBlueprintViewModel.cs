using System.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiKongBlueprintViewModel: KongBlueprintViewModel, INotifyPropertyChanged
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
}
