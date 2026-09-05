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
    private static readonly Dictionary<HintSuggestion, HintSuggestionConfig> _suggestionToConfigMap = new()
    {
        {HintSuggestion.REGION, new HintSuggestionConfig(["Hint Regions"], HintRegion.All)},
        {HintSuggestion.CHECK, new HintSuggestionConfig(["Item Locations"], HintLocation.All)},
        {HintSuggestion.MOVE, new HintSuggestionConfig(["Moves"], HintMove.All)},
        {HintSuggestion.REGION_OR_MOVE, new HintSuggestionConfig(["Hint Regions", "Moves"], [.. HintRegion.All, .. HintMove.All])}
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

    /// <summary>
    /// Tracks whether the ViewModel is initializing from loaded hint data.
    /// Prevents suggestion dropdown from opening during initialization.
    /// </summary>
    private bool _isInitializing = true;

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

    [ObservableProperty]
    public partial HintPanelType HintPanelType { get; set; } = (HintPanelType)(-1);

    partial void OnHintPanelTypeChanged(HintPanelType value)
    {
        // Update dependent properties when HintPanelType changes
        IsHintInfoPath = value == HintPanelType.Paths;
        IsHintInfoFoolish = value == HintPanelType.Foolish;
        IsHintInfoPotionCount = value == HintPanelType.PotionCounts;
        OnPropertyChanged(nameof(IsHintInfoNotFoolish));
    }

    [ObservableProperty]
    public partial string LocationText { get; set; } = "";

    partial void OnLocationTextChanged(string value)
    {
        _hintService.UpdateHintLocation(_hintId, value);
    }

    [ObservableProperty]
    public partial string PotionCountText { get; set; } = "";

    partial void OnPotionCountTextChanged(string value)
    {
        _hintService.UpdateHintPotionCount(_hintId, value);
    }

    [ObservableProperty]
    public partial HintTypeSettings HintTypeSettings { get; set; } = null!;

    [ObservableProperty]
    public partial ObservableCollection<string> Suggestions { get; set; } = [];

    [ObservableProperty]
    public partial bool SuggestionBoxIsOpen { get; set; } = false;


    /// <summary>
    /// Lazy-load FoundHintItemListViewModel.
    /// </summary>
    public FoundHintItemListViewModel FoundHintItemListViewModel
    {
        get
        {
            _foundHintItemListViewModel ??= new FoundHintItemListViewModel(
                HintPanelType,
                _hintId,
                $"Select Found Items - {HintPanelType}",
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
                HintPanelType,
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

    [ObservableProperty]
    public partial bool IsHintInfoPath { get; set; } = false;

    [ObservableProperty]
    public partial bool IsHintInfoFoolish { get; set; } = false;

    public bool IsHintInfoNotFoolish => !IsHintInfoPath;

    [ObservableProperty]
    public partial bool IsHintInfoPotionCount { get; set; } = false;

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

            // Populate initial values from the loaded hint
            LocationText = hint.LocationText;
            PotionCountText = hint.PotionCountText;
        }

        // Get suggestion config if applicable
        if (_suggestionToConfigMap.TryGetValue(HintTypeSettings.HintSuggestion, out var config))
        {
            HintSuggestionConfig = config;
        }

        _hintService.HintLocationUpdated += OnLocationUpdated;
        _hintService.HintPotionCountUpdated += OnPotionCountUpdated;

        // Build initial suggestions
        UpdateSuggestions();

        // Mark initialization as complete; subsequent text changes are user edits
        _isInitializing = false;
    }

    private void OnPotionCountUpdated(object? sender, HintUpdatedEventArgs e)
    {
        if (e.HintId != _hintId)
        {
            return;
        }

        PotionCountText = e.UpdatedHint.PotionCountText;
        OnPropertyChanged(nameof(PotionCountText));
    }

    private void OnLocationUpdated(object? sender, HintUpdatedEventArgs e)
    {
        if (e.HintId != _hintId)
        {
            return;
        }

        LocationText = e.UpdatedHint.LocationText;
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
        if (HintPanelType == HintPanelType.Kongs)
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
        var targetRegion = HintPanelType.ToRegion();
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
            && HintPanelType == HintPanelType.PotionCounts
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
        return LocationText.Length > 0 && PotionCountText.Length > 0 && HintPanelType == HintPanelType.PotionCounts;
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
        if (HintPanelType == HintPanelType.Kongs)
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
            HintPanelType,
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

        // Update dropdown visibility only if not initializing
        // This prevents dropdowns from opening on load; they only open for user-initiated edits
        if (!_isInitializing)
        {
            bool shouldBeOpen = LocationText != ""
                && Suggestions.Count > 0
                && HintPanelType != HintPanelType.Kongs;

            SuggestionBoxIsOpen = shouldBeOpen;
        }
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
        return HintPanelType switch
        {
            HintPanelType.Paths => $"Hint {_hintId}: {LocationText} is on the Path",
            HintPanelType.Foolish => $"Hint {_hintId}: {LocationText} is Foolish",
            HintPanelType.PotionCounts => $"Hint {_hintId}: {LocationText} has {PotionCountText} Potion[s]",
            HintPanelType.Kongs => $"Kong Hint {_hintId}: {LocationText}",
            _ => $"Hint {_hintId}: {LocationText}"
        };
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
