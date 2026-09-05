using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public partial class UiItemViewModel : BroadcastItemViewModel, INotifyPropertyChanged
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ItemName _itemName;

    public MouseDragType MouseDragType { get; private set; }

    #region Proxy Properties

    [ObservableProperty]
    public partial bool IsDragging { get; set; } = false;

    [ObservableProperty]
    public partial double DragX { get; set; } = 0;

    [ObservableProperty]
    public partial double DragY { get; set; } = 0;

    [ObservableProperty]
    public partial double DragWidth { get; set; } = 0;

    [ObservableProperty]
    public partial double DragHeight { get; set; } = 0;

    [ObservableProperty]
    public partial double Opacity { get; set; } = 1.0;

    [ObservableProperty]
    public partial RegionName RegionName { get; set; } = RegionName.UNKNOWN;

    #endregion

    public UiItemViewModel(
        ItemName itemName,
        IItemTrackingService itemTrackingService,
        IParsedSpoilerDataService parsedSpoilerDataService,
        ISavedProgressProvider savedProgressProvider,
        IThemeService themeService
    ) : base(itemName, itemTrackingService, parsedSpoilerDataService, savedProgressProvider, themeService)
    {
        _itemName = itemName;
        _itemTrackingService = itemTrackingService ?? throw new ArgumentNullException(nameof(itemTrackingService));
        _parsedSpoilerDataService = parsedSpoilerDataService ?? throw new ArgumentNullException(nameof(parsedSpoilerDataService));
        _itemTrackingService.ItemStateChanged += OnItemStateChanged;
        _parsedSpoilerDataService.ParsedSpoilerDataChanged += OnParsedSpoilerDataChanged;
        // Initialize state from service (if item exists)
        InitializeState();
    }

    /// <summary>
    /// Initialize properties from current service state.
    /// Called on construction and when state needs to be refreshed.
    /// </summary>
    private void InitializeState()
    {
        var itemState = _itemTrackingService.GetItemState(_itemName);

        if (itemState != null)
        {
            UpdateImageResourceKey(itemState);
            IsStarred = itemState.IsStarred;
            Opacity = itemState.Opacity;
        }
        else
        {
            // No saved state yet - show default B&W variant
            UpdateImageResourceKey(null);
            IsStarred = false;
            Opacity = 1;
        }

        UpdateHoverText();
    }

    /// <summary>
    /// Initiates a drag operation with the specified drag type and dimensions.
    /// This is called by drag event handlers in the view code-behind.
    /// </summary>
    public void BeginDrag(MouseDragType dragType, double width, double height)
    {
        Opacity = dragType == MouseDragType.Right ? 0.375 : 1.0;
        MouseDragType = dragType;
        DragWidth = width;
        DragHeight = height;
        IsDragging = true;
        UpdateImageResourceKey(null);
    }

    /// <summary>
    /// Ends the current drag operation and restores opacity.
    /// This is called by drag event handlers in the view code-behind.
    /// </summary>
    public void EndDrag()
    {
        IsDragging = false;
        MouseDragType = MouseDragType.None;
        // Restore opacity from the saved item state
        var savedItem = _itemTrackingService.GetItemState(_itemName);
        if (savedItem != null)
        {
            Opacity = savedItem.Opacity;
        }
        else
        {
            Opacity = 1.0;
        }
        UpdateImageResourceKey(savedItem);
    }

    /// <summary>
    /// Completes a drag operation by placing the item in the found region.
    /// This is called by drag event handlers in the view code-behind.
    /// </summary>
    public void CompleteDrag(RegionName foundRegion)
    {
        IsDragging = false;

        Opacity = MouseDragType == MouseDragType.Right ? 0.375 : 1.0;
        var previousItem = _itemTrackingService.GetItemState(_itemName) ?? SavedItem.CreateEmpty(_itemName);
        _itemTrackingService.SetItemState(_itemName, previousItem with
        {
            Region = foundRegion,
            Opacity = Opacity,
            Starred = previousItem.Starred,
            Autotracked = previousItem.Autotracked,
        });
        MouseDragType = MouseDragType.None;
    }

    #region Commands

    [RelayCommand]
    private void ToggleStar()
    {
        _itemTrackingService.ToggleStar(_itemName);
    }

    [RelayCommand(CanExecute = nameof(CanRemoveFromRegion))]
    private void RemoveFromRegion()
    {
        var item = _itemTrackingService.GetItemState(_itemName);
        if (item is not null)
        {
            _itemTrackingService.ClearItemState(_itemName);
        }
    }

    private bool CanRemoveFromRegion()
    {
        var item = _itemTrackingService.GetItemState(_itemName);
        return item is not null && !item.Autotracked;
    }

    #endregion

    private void OnItemStateChanged(object? sender, ItemStateChangedEventArgs e)
    {
        // Handle the item state change event here.
        if (e.UpdatedItem.ItemName == _itemName)
        {
            // Update the ImageResourceKey based on the new state of the item.
            UpdateImageResourceKey(e.UpdatedItem);
            IsStarred = e.UpdatedItem.IsStarred;
            Opacity = e.UpdatedItem.Opacity;

            // Update hover text from spoiler service if available
            UpdateHoverText();
        }
    }

    private void UpdateImageResourceKey(SavedItem? item)
    {
        string baseKey = _itemName.ToString().ToLower();
        RegionName spoiledRegion = RegionName.UNKNOWN;
        if (_parsedSpoilerDataService.CurrentData?.StartingItems.ContainsKey(_itemName) == true)
        {
            spoiledRegion = _parsedSpoilerDataService.CurrentData.StartingItems[_itemName];
        }

        // If dragging and dropping, always use the full color variant.
        if (IsDragging)
        {
            ImageResourceKey = baseKey;
            return;
        }

        // If no item state, show default B&W variant
        if (item == null && spoiledRegion == RegionName.UNKNOWN)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If the item is empty (not a part of any real region), show default B&W variant.
        if (item?.IsEmpty == true)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }


        // If item is hinted, always show B&W
        // This logic may need to be revised.
        if (item?.Hinted == true)
        {
            ImageResourceKey = $"{baseKey}_bw";
            return;
        }

        // If item is in a spoiled region, show full-color
        if (spoiledRegion != RegionName.UNKNOWN)
        {
            ImageResourceKey = baseKey;
            return;
        }

        // If item is in a valid region (but not ItemGrid/UNKNOWN), show full-color
        var itemRegion = item?.Region ?? RegionName.UNKNOWN;
        if (itemRegion != RegionName.UNKNOWN && EndGameMappings.ValidMoveRegions.Contains(itemRegion))
        {
            ImageResourceKey = baseKey;
            return;
        }

        // Otherwise, item is in ItemGrid (UNKNOWN) or not placed, show B&W
        ImageResourceKey = $"{baseKey}_bw";
    }
}
