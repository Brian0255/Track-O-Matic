using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

/// <summary>
/// Parent ViewModel for HintInfo control.
/// Manages hint text fields, suggestion building, Kong hint parsing, and child list ViewModels.
/// </summary>
public partial class HintInfoViewModel : ObservableObject, IEquatable<HintInfoViewModel>, IDisposable
{
    /// <summary>
    /// Json key for Kong Hint shorthand lookups.
    /// </summary>
    private const string KONG_HINT_SHORTHAND_KEY = "Kong Hint Shorthand";

    /// <summary>
    /// Mapping of HintSuggestion types to their configurations.
    /// </summary>
    private static readonly Dictionary<HintSuggestion, HintSuggestionConfig> SuggestionToConfigMap = new()
    {
        {HintSuggestion.REGION, new HintSuggestionConfig(["Hint Regions"], HintRegion.All)},
        {HintSuggestion.CHECK, new HintSuggestionConfig(["Item Locations"], HintLocation.All)},
        {HintSuggestion.MOVE, new HintSuggestionConfig(["Moves"], HintMove.All)},
        {HintSuggestion.REGION_OR_MOVE, new HintSuggestionConfig(["Hint Regions", "Moves"], HintRegion.All.Concat(HintMove.All).ToList())}
    };

    private readonly HintId _hintId;
    public HintId HintId => _hintId;
    private readonly IHintService _hintService;
    private readonly IShortcutService _shortcutService;
    private readonly IHintItemSelectionService _hintItemSelectionService;
    private readonly ISavedProgressProvider _savedProgressProvider;
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IUserSettingsService _userSettingsService;

    /// <summary>
    /// Tracks whether path items selection dialog has been opened.
    /// </summary>
    private bool _pathItemsDialogOpened = false;

    /// <summary>
    /// Tracks whether found items selection dialog has been opened.
    /// </summary>
    private bool _foundItemsDialogOpened = false;

    private HintPanelType _hintPanelType = (HintPanelType)(-1); // Initialize to invalid value to ensure first assignment triggers notification

    public HintPanelType HintPanelType
    {
        get => _hintPanelType;
        private set
        {
            if (SetProperty(ref _hintPanelType, value))
            {
                // Update dependent properties when HintPanelType changes
                IsHintInfoPath = value == HintPanelType.Paths;
                IsHintInfoFoolish = value == HintPanelType.Foolish;
                IsHintInfoPotionCount = value == HintPanelType.PotionCounts;
                OnPropertyChanged(nameof(IsHintInfoNotFoolish));
            }
        }
    }

    public RegionName RegionName { get; private set; }

    /// <summary>
    /// Event invoked when path items selection dialog should open.
    /// Code-behind subscribes to this for actual dialog opening.
    /// </summary>
    public event EventHandler? PathItemsSelectionRequested;

    /// <summary>
    /// Event invoked when found items selection dialog should open.
    /// Code-behind subscribes to this for actual dialog opening.
    /// </summary>
    public event EventHandler? FoundItemsSelectionRequested;

    private string _locationText = "";
    public string LocationText
    {
        get => _locationText;
        set
        {
            if (SetProperty(ref _locationText, value))
            {
                _hintService.UpdateHintLocation(_hintId, value);
            }
        }
    }

    private string _potionCountText = "";
    public string PotionCountText
    {
        get => _potionCountText;
        set
        {
            if (SetProperty(ref _potionCountText, value))
            {
                _hintService.UpdateHintPotionCount(_hintId, value);
            }
        }
    }

    private HintTypeSettings _hintTypeSettings = null!;
    public HintTypeSettings HintTypeSettings
    {
        get => _hintTypeSettings;
        set => SetProperty(ref _hintTypeSettings, value);
    }



    private ObservableCollection<string> _suggestions = new();
    public ObservableCollection<string> Suggestions
    {
        get => _suggestions;
        set => SetProperty(ref _suggestions, value);
    }

    private bool _suggestionBoxIsOpen = false;
    public bool SuggestionBoxIsOpen
    {
        get => _suggestionBoxIsOpen;
        set => SetProperty(ref _suggestionBoxIsOpen, value);
    }


    /// <summary>
    /// Lazy-load FoundHintItemListViewModel.
    /// </summary>
    public FoundHintItemListViewModel FoundHintItemListViewModel
    {
        get
        {
            _foundHintItemListViewModel ??= new FoundHintItemListViewModel(
                _hintPanelType,
                _hintId,
                $"Select Found Items - {_hintPanelType}",
                _itemTrackingService,
                _hintItemSelectionService,
                _hintService,
                _savedProgressProvider
                );
            return _foundHintItemListViewModel;
        }
    }
    private FoundHintItemListViewModel? _foundHintItemListViewModel;

    /// <summary>
    /// Lazy-load PathHintItemListViewModel.
    /// </summary>
    public PathHintItemListViewModel PathHintItemListViewModel
    {
        get
        {
            _pathHintItemListViewModel ??= new PathHintItemListViewModel(
                _hintPanelType,
                _hintId,
                "Select Path Items",
                _hintService,
                _hintItemSelectionService,
                _savedProgressProvider
            );
            return _pathHintItemListViewModel;
        }
    }
    private PathHintItemListViewModel? _pathHintItemListViewModel;


    private bool _isHintInfoPath = false;
    public bool IsHintInfoPath
    {
        get => _isHintInfoPath;
        private set => SetProperty(ref _isHintInfoPath, value);
    }

    private bool _isHintInfoFoolish = false;
    public bool IsHintInfoFoolish
    {
        get => _isHintInfoFoolish;
        private set => SetProperty(ref _isHintInfoFoolish, value);
    }

    public bool IsHintInfoNotFoolish => !IsHintInfoPath;

    private bool _isHintInfoPotionCount = false;
    public bool IsHintInfoPotionCount
    {
        get => _isHintInfoPotionCount;
        private set => SetProperty(ref _isHintInfoPotionCount, value);
    }

    /// <summary>
    /// Configuration for hint suggestions (if applicable to this hint type).
    /// Null if HintSuggestion is NONE.
    /// </summary>
    private HintSuggestionConfig? HintSuggestionConfig { get; set; }

    [ObservableProperty]
    public partial bool IsFocusRequested { get; set; } = false;

    [ObservableProperty]
    public partial bool ShouldFocusLocation { get; set; } = false;

    [ObservableProperty]
    public partial bool LocationIsFocused { get; set; } = false;

    [ObservableProperty]
    public partial bool ShouldFocusSuggestionsFirst { get; set; } = false;

    [ObservableProperty]
    public partial string? SelectedSuggestion { get; set; } = null;

    [ObservableProperty]
    public partial int SelectedSuggestionIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool IsSuggestionBoxFocused { get; set; } = false;

    /// <summary>
    /// Tracks whether PotionCount has already been focused for the current location entry.
    /// Prevents re-focusing when user presses Enter in PotionCount field.
    /// Resets when LocationText changes.
    /// </summary>
    private bool _potionCountFocusedForCurrentLocation = false;

    public HintInfoViewModel(
        HintId hintId,
        IHintService hintService,
        IShortcutService shortcutService,
        IHintItemSelectionService hintItemSelectionService,
        ISavedProgressProvider savedProgressProvider,
        IItemTrackingService itemTrackingService,
        IUserSettingsService userSettingsService)
    {
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));

        _hintId = hintId;

        var hint = _hintService.GetHint(hintId);
        if (hint is not null)
        {
            HintPanelType = hint.HintPanelType;
            RegionName = HintPanelType.ToRegion();
            HintTypeSettings = HintTypeSettingsList.SETTINGS[HintPanelType.ToHintType()];
        }

        // Get suggestion config if applicable
        if (SuggestionToConfigMap.TryGetValue(HintTypeSettings.HintSuggestion, out var config))
        {
            HintSuggestionConfig = config;
        }

        _hintService.HintLocationUpdated += OnLocationUpdated;
        _hintService.HintPotionCountUpdated += OnPotionCountUpdated;

        // Build initial suggestions
        UpdateSuggestions();
    }

    private void OnPotionCountUpdated(object? sender, HintUpdatedEventArgs e)
    {
        if (e.HintId != _hintId)
        {
            return;
        }

        _potionCountText = e.UpdatedHint.PotionCountText;
        OnPropertyChanged(nameof(PotionCountText));
    }

    private void OnLocationUpdated(object? sender, HintUpdatedEventArgs e)
    {
        if (e.HintId != _hintId)
        {
            return;
        }

        _locationText = e.UpdatedHint.LocationText;
        OnPropertyChanged(nameof(LocationText));
        UpdateSuggestions();
    }

    /// <summary>
    /// Handle property changes to reset focus guard when LocationText changes.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // Reset the potion count focus guard when user enters a new location
        // This allows the focus command to trigger again for the next location
        if (e.PropertyName == nameof(LocationText))
        {
            _potionCountFocusedForCurrentLocation = false;
            ShouldFocusLocation = false; // Reset location focus request
            // Refresh the command's CanExecute state
            FocusAndSelectPotionCountCommand.NotifyCanExecuteChanged();
        }
    }

    #region Commands

    /// <summary>
    /// Signal that path items selection dialog should open.
    /// Code-behind subscribes to PathItemsSelectionRequested and handles actual dialog opening.
    /// Only enabled if this hint type shows path items.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanOpenPathItemsSelection))]
    private void OpenPathItemsSelection()
    {
        _pathItemsDialogOpened = true;
        PathItemsSelectionRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns true if path items selection can be opened for this hint type.
    /// </summary>
    private bool CanOpenPathItemsSelection() => IsHintInfoPath && !_pathItemsDialogOpened;

    /// <summary>
    /// Signal that found items selection dialog should open.
    /// Code-behind subscribes to FoundItemsSelectionRequested and handles actual dialog opening.
    /// Only enabled if this hint type shows found items.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanOpenFoundItemsSelection))]
    private void OpenFoundItemsSelection()
    {
        _foundItemsDialogOpened = true;
        FoundItemsSelectionRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns true if found items selection can be opened for this hint type.
    /// </summary>
    private bool CanOpenFoundItemsSelection() => !IsHintInfoFoolish && !_foundItemsDialogOpened;


    /// <summary>
    /// Handle user selecting a suggestion or manually confirming location (Enter key).
    /// Public command that accepts screen mouse coordinates for dialog positioning.
    /// Attempts Kong hint parsing and invokes dialog opening commands as appropriate.
    /// </summary>
    [RelayCommand]
    public void ConfirmLocationAndItems((string locationText, double X, double Y) screenMousePosition)
    {
        // Try Kong hint parsing if applicable
        if (_hintPanelType == HintPanelType.Kongs)
        {
            if (TryProcessKongHint(LocationText))
            {
                // Kong hint was processed successfully
                // Reset dialog flags since Kong parsing populates FoundItems
                _pathItemsDialogOpened = false;
                _foundItemsDialogOpened = true; // Kong items go directly to FoundItems
                return;
            }
        }

        if (OpenPathItemsSelectionCommand.CanExecute(null))
        {
            OpenPathItemsSelectionCommand.Execute(null);
        }
        else if (OpenFoundItemsSelectionCommand.CanExecute(null))
        {
            OpenFoundItemsSelectionCommand.Execute(null);
        }
    }

    /// <summary>
    /// Handle suggestion selection from the dropdown.
    /// Private method called when a suggestion is selected.
    /// </summary>
    [RelayCommand]
    private void SelectSuggestion(string? selectedSuggestion)
    {
        // If a suggestion was selected, update the location text
        if (!string.IsNullOrEmpty(selectedSuggestion))
        {
            LocationText = selectedSuggestion;
            SuggestionBoxIsOpen = false;

            // Trigger the appropriate action based on hint panel type
            TriggerLocationConfirmationAction();
        }
    }

    /// <summary>
    /// Remove this hint from tracking.
    /// Cleans up any user-hinted items that were never found via the normal system.
    /// </summary>
    [RelayCommand]
    public void RemoveHint()
    {
        // Only process direct item hints that tie themselves to a region.
        var targetRegion = _hintPanelType.ToRegion();
        if (targetRegion.IsPlayableRegion())
        {
            foreach (var hint in FoundHintItemListViewModel.FoundItems)
            {
                var itemState = _itemTrackingService.GetItemState(hint.ItemName);
                if (itemState is not null && itemState.Hinted && itemState.Region == targetRegion)
                {
                    // Preserve any stars if they were set this way.
                    _itemTrackingService.SetItemState(hint.ItemName, itemState with { Hinted = false, Region = RegionName.UNKNOWN });
                }
            }
        }

        _hintService.RemoveHint(_hintId);
    }

    /// <summary>
    /// Command to focus and select the PotionCount TextBox when user confirms location.
    /// Triggered by FocusOnCommandBehavior attached behavior in XAML.
    /// This eliminates the need for x:Name reference to the PotionCount control.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanFocusAndSelectPotionCount))]
    private void FocusAndSelectPotionCount()
    {
        // Signal to the behavior that focus is requested
        IsFocusRequested = true;
        _potionCountFocusedForCurrentLocation = true;
    }

    private bool CanFocusAndSelectPotionCount()
    {
        return LocationText.Length > 0
            && _hintPanelType == HintPanelType.PotionCounts
            && !_potionCountFocusedForCurrentLocation;
    }

    /// <summary>
    /// Command to confirm potion count entry and prepare to save the hint.
    /// Triggered when user presses Enter/Return in the PotionCount TextBox.
    /// TODO: Phase 2 - Implement actual hint persistence when service layer is ready.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanConfirmPotionCount))]
    private void ConfirmPotionCount()
    {
        // TODO: Phase 2 - Call mainWindow.DataSaver.Save() or similar persistence logic
        // For now, just clear the focus (which is handled by the code-behind)
    }

    private bool CanConfirmPotionCount()
    {
        // Allow confirm if we have both location and potion count text
        return LocationText.Length > 0 && PotionCountText.Length > 0 && _hintPanelType == HintPanelType.PotionCounts;
    }

    /// <summary>
    /// Trigger the appropriate action when location text is confirmed.
    /// Implements "first come, first serve" behavior:
    /// 1. PotionCounts panel: Focus potion count text box
    /// 2. Paths panel: Open path items selection dialog
    /// 3. Kongs panel: Try Kong hint parsing, then open found items dialog if parsing succeeds
    /// 4. Everything else (except Foolish): Open found items selection dialog
    /// </summary>
    public void TriggerLocationConfirmationAction()
    {
        // Priority 1: Potion Counts panel
        if (IsHintInfoPotionCount)
        {
            if (FocusAndSelectPotionCountCommand.CanExecute(null))
            {
                FocusAndSelectPotionCountCommand.Execute(null);
            }
            return;
        }

        // Priority 2: Paths panel
        if (IsHintInfoPath)
        {
            if (OpenPathItemsSelectionCommand.CanExecute(null))
            {
                OpenPathItemsSelectionCommand.Execute(null);
            }
            return;
        }

        // Priority 3: Kongs panel (special Kong hint parsing logic)
        if (_hintPanelType == HintPanelType.Kongs)
        {
            if (TryProcessKongHint(LocationText))
            {
                // Kong hint was processed successfully
                _pathItemsDialogOpened = false;
                _foundItemsDialogOpened = true; // Kong items go directly to FoundItems
                return;
            }
        }

        // Priority 4: Everything else except Foolish (they don't have item selections)
        if (!IsHintInfoFoolish)
        {
            if (OpenFoundItemsSelectionCommand.CanExecute(null))
            {
                OpenFoundItemsSelectionCommand.Execute(null);
            }
        }
    }

    #endregion

    /// <summary>
    /// Rebuild suggestions based on current location text.
    /// </summary>
    private void UpdateSuggestions()
    {
        var matches = _shortcutService.FindMatches(
            LocationText,
            _hintPanelType,
            HintSuggestionConfig
        ).ToList();

        // Update collection in-place to avoid redundant popup re-rendering
        if (Suggestions.Count != matches.Count || !Suggestions.SequenceEqual(matches))
        {

            Suggestions.Clear();
            foreach (var match in matches)
            {
                Suggestions.Add(match);
            }
        }

        // Update dropdown visibility
        bool shouldBeOpen = LocationText != ""
            && Suggestions.Count > 0
            && _hintPanelType != HintPanelType.Kongs;


        SuggestionBoxIsOpen = shouldBeOpen;
    }

    /// <summary>
    /// Try to parse location text as a Kong hint (3-word abbreviation format).
    /// If successful, registers the Kong in FoundItems and returns true.
    /// </summary>
    private bool TryProcessKongHint(string locationText)
    {
        var hintWordList = locationText.Split(' ');
        if (hintWordList.Length != 3)
        {
            return false;
        }

        if (!HintData.UserShortcuts.TryGetValue(KONG_HINT_SHORTHAND_KEY, out var kongShorthands))
        {
            return false;
        }

        // Validate all three words are valid Kong shorthand keys
        for (int i = 0; i < hintWordList.Length; i++)
        {
            hintWordList[i] = hintWordList[i].ToLower();
            if (!kongShorthands.ContainsKey(hintWordList[i]))
            {
                return false;
            }
        }

        // The kong to find is in the first word. The region is the third. The second is who or what guards the kong.
        var foundKongString = kongShorthands[hintWordList[0]];
        if (!Enum.TryParse(foundKongString, out ItemName kongThatIsFound))
        {
            return false;
        }

        // Build human-readable location text from the three parts
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        var region = kongShorthands[hintWordList[2]];
        var thingThatFinds = kongShorthands[hintWordList[1]];
        thingThatFinds = textInfo.ToTitleCase(thingThatFinds.ToLower());

        var newText = thingThatFinds + " in " + region;
        if (thingThatFinds == "Boss")
        {
            newText = region + " Boss Defeated";
        }

        _hintService.SetFoundItems(_hintId, new Dictionary<ItemName, bool> { { kongThatIsFound, false } }, HintPanelType);
        _hintService.UpdateHintLocation(_hintId, newText);

        return true;
    }


    #region Object Overrides

    public bool Equals(HintInfoViewModel? other)
    {
        return other is not null && _hintId == other._hintId;
    }

    public override bool Equals(object? obj) => Equals(obj as HintInfoViewModel);

    public override int GetHashCode() => _hintId.GetHashCode();

    public override string ToString()
    {
        switch (_hintPanelType)
        {
            case HintPanelType.Paths:
                return $"Hint {_hintId}: {LocationText} is on the Path";
            case HintPanelType.Foolish:
                return $"Hint {_hintId}: {LocationText} is Foolish";
            case HintPanelType.PotionCounts:
                return $"Hint {_hintId}: {LocationText} has {PotionCountText} Potion[s]";
            case HintPanelType.Kongs:
                return $"Kong Hint {_hintId}: {LocationText}";
            default:
                return $"Hint {_hintId}: {LocationText}";
        }
    }

    #endregion

    #region Operator Overloads

    public static bool operator ==(HintInfoViewModel? left, HintInfoViewModel? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(HintInfoViewModel? left, HintInfoViewModel? right) => !(left == right);

    #endregion

    #region IDisposable

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Unsubscribe from events
                _hintService.HintLocationUpdated -= OnLocationUpdated;
                _hintService.HintPotionCountUpdated -= OnPotionCountUpdated;
            }
            _disposed = true;
        }
    }

    ~HintInfoViewModel()
    {
        Dispose(false);
    }

    #endregion
}
