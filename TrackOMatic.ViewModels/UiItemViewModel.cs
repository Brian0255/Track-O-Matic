using System.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.ViewModels;

public class UiItemViewModel : BroadcastItemViewModel, INotifyPropertyChanged
{
    private readonly IItemTrackingService _itemTrackingService;
    private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
    private readonly ItemName _itemName;

    public MouseDragType MouseDragType { get; private set; }

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

    #region Proxy Properties

    private bool _isDragging = false;
    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            if (_isDragging != value)
            {
                _isDragging = value;
                OnPropertyChanged();
            }
        }
    }

    private double _dragX = 0;
    public double DragX
    {
        get => _dragX;
        set
        {
            if (_dragX != value)
            {
                _dragX = value;
                OnPropertyChanged();
            }
        }
    }

    private double _dragY = 0;
    public double DragY
    {
        get => _dragY;
        set
        {
            if (_dragY != value)
            {
                _dragY = value;
                OnPropertyChanged();
            }
        }
    }

    private double _dragWidth = 0;
    public double DragWidth
    {
        get => _dragWidth;
        set
        {
            if (_dragWidth != value)
            {
                _dragWidth = value;
                OnPropertyChanged();
            }
        }
    }

    private double _dragHeight = 0;
    public double DragHeight
    {
        get => _dragHeight;
        set
        {
            if (_dragHeight != value)
            {
                _dragHeight = value;
                OnPropertyChanged();
            }
        }
    }

    private double _opacity = 1.0;
    public double Opacity
    {
        get => _opacity;
        protected set
        {
            if (_opacity != value)
            {
                _opacity = value;
                OnPropertyChanged();
            }
        }
    }

    private RegionName _regionName = RegionName.UNKNOWN;
    public RegionName RegionName
    {
        get => _regionName;
        protected set
        {
            if (_regionName != value)
            {
                _regionName = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    public void BeginDrag(MouseDragType dragType, double width, double height)
    {
        Opacity = dragType == MouseDragType.Right ? 0.375 : 1.0;
        MouseDragType = dragType;
        DragWidth = width;
        DragHeight = height;
        IsDragging = true;
        UpdateImageResourceKey(null);
    }

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

    public void ToggleStar()
    {
        _itemTrackingService.ToggleStar(_itemName);
    }

    public void RemoveFromRegion()
    {
        var item = _itemTrackingService.GetItemState(_itemName);
        if (item is not null)
        {
            _itemTrackingService.ClearItemState(_itemName);
        }
    }

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

    private void UpdateHoverText()
    {
        // TODO: Query _spoilerService for detailed hint/point information when available
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
