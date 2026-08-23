using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

/// <summary>
/// Parent ViewModel for HintInfo control.
/// Manages hint text fields, suggestion building, Kong hint parsing, and child list ViewModels.
/// </summary>
public partial class HintInfoViewModel : ObservableObject
{
    private readonly HintPanelType _hintPanelType;
    private readonly RegionName _regionName;
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

    private string _locationText = "";
    public string LocationText
    {
        get => _locationText;
        set
        {
            if (SetProperty(ref _locationText, value))
            {
                OnLocationTextChanged(value);
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
                OnPotionCountTextChanged(value);
            }
        }
    }

    private HintTypeSettings _hintTypeSettings = null!;
    public HintTypeSettings HintTypeSettings
    {
        get => _hintTypeSettings;
        set => SetProperty(ref _hintTypeSettings, value);
    }

    private SavedHint _savedHint = null!;
    public SavedHint SavedHint
    {
        get => _savedHint;
        set => SetProperty(ref _savedHint, value);
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

    public bool IsHintInfoPath => _hintPanelType == HintPanelType.Paths;

    public bool IsHintInfoFoolish => _hintPanelType == HintPanelType.Foolish;

    /// <summary>
    /// Configuration for hint suggestions (if applicable to this hint type).
    /// Null if HintSuggestion is NONE.
    /// </summary>
    private HintSuggestionConfig? HintSuggestionConfig { get; set; }

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

    /// <summary>
    /// Json key for Kong Hint shorthand lookups.
    /// </summary>
    private const string KONG_HINT_SHORTHAND_KEY = "Kong Hint Shorthand";

    private void OnLocationTextChanged(string newValue)
    {
        SavedHint.LocationText = newValue;
        UpdateSuggestions();
    }

    private void OnPotionCountTextChanged(string newValue)
    {
        SavedHint.PotionCountText = newValue;
    }

    public HintInfoViewModel(
        HintType hintType,
        HintPanelType hintPanelType,
        RegionName regionName,
        SavedHint savedHint,
        IShortcutService shortcutService,
        IHintItemSelectionService hintItemSelectionService,
        ISavedProgressProvider savedProgressProvider,
        IItemTrackingService itemTrackingService,
        IUserSettingsService userSettingsService)
    {
        _hintPanelType = hintPanelType;
        _regionName = regionName;
        _shortcutService = shortcutService ?? throw new ArgumentNullException(nameof(shortcutService));
        _hintItemSelectionService = hintItemSelectionService ?? throw new ArgumentNullException(nameof(hintItemSelectionService));
        _savedProgressProvider = savedProgressProvider ?? throw new ArgumentNullException(nameof(savedProgressProvider));
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));

        // Initialize settings
        HintTypeSettings = HintTypeSettingsList.SETTINGS[hintType];

        // Get suggestion config if applicable
        if (SuggestionToConfigMap.TryGetValue(HintTypeSettings.HintSuggestion, out var config))
        {
            HintSuggestionConfig = config;
        }

        // Initialize SavedHint
        SavedHint = savedHint;
        LocationText = savedHint.LocationText;
        PotionCountText = savedHint.PotionCountText;

        // Build initial suggestions
        UpdateSuggestions();

        OnPropertyChanged(nameof(IsHintInfoPath));
        OnPropertyChanged(nameof(IsHintInfoFoolish));
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

    /// <summary>
    /// Lazy-load FoundHintItemListViewModel.
    /// </summary>
    public FoundHintItemListViewModel FoundHintItemListViewModel
    {
        get
        {
            _foundHintItemListViewModel ??= new FoundHintItemListViewModel(
                _hintPanelType,
                $"Select Found Items - {_hintPanelType}",
                _itemTrackingService,
                _hintItemSelectionService,
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
                "Select Path Items",
                _hintItemSelectionService,
                _savedProgressProvider
            );
            return _pathHintItemListViewModel;
        }
    }
    private PathHintItemListViewModel? _pathHintItemListViewModel;

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
    /// Rebuild suggestions based on current location text.
    /// </summary>
    private void UpdateSuggestions()
    {
        var matches = _shortcutService.FindMatches(
            LocationText,
            _hintPanelType,
            HintSuggestionConfig
        );

        Suggestions = new ObservableCollection<string>(matches);

        // Update dropdown visibility
        SuggestionBoxIsOpen = LocationText != ""
            && Suggestions.Count > 0
            && _hintPanelType != HintPanelType.Kongs;
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

        // Parse the Kong item from first word
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

        // Populate found items with the Kong
        SavedHint.FoundItems = new Dictionary<ItemName, bool> { { kongThatIsFound, false } };

        // TODO: Phase 2 - Call ItemTrackingService to register Kong as user-hinted item
        // _itemTrackingService.SetItemState(kongThatIsFound, new SavedItem(...));

        // Update location text (will trigger UpdateSuggestions via partial OnLocationTextChanged)
        LocationText = newText;

        return true;
    }

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
        }

        SuggestionBoxIsOpen = false;
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
        if (!targetRegion.IsPlayableRegion())
        {
            return;
        }

        // TODO: Phase 2 - Call ItemTrackingService to remove hinted-but-not-found items
        // Find items that were direct-hinted but never found
        // var itemsToRemove = SavedHint.FoundItems
        //     .Where(entry => !entry.Value) // Not found (false = not found)
        //     .Select(entry => entry.Key)
        //     .ToList();
        // foreach (var item in itemsToRemove)
        // {
        //     _itemTrackingService.ClearItemState(item);
        // }
    }

    [ObservableProperty]
    private bool _isFocusRequested = false;

    [ObservableProperty]
    private bool _shouldFocusLocation = false;

    [ObservableProperty]
    private bool _locationIsFocused = false;

    [ObservableProperty]
    private bool _shouldFocusSuggestionsFirst = false;

    [ObservableProperty]
    private string? _selectedSuggestion = null;

    [ObservableProperty]
    private int _selectedSuggestionIndex = -1;

    [ObservableProperty]
    private bool _isSuggestionBoxFocused = false;

    /// <summary>
    /// Tracks whether PotionCount has already been focused for the current location entry.
    /// Prevents re-focusing when user presses Enter in PotionCount field.
    /// Resets when LocationText changes.
    /// </summary>
    private bool _potionCountFocusedForCurrentLocation = false;

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
}
