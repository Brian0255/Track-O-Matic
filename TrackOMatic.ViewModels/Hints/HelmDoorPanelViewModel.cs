using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class HelmDoorPanelViewModel : ObservableObject
{
    private readonly IUserSettingsService _userSettingsService;

    private string _lineColor = string.Empty;
    public string LineColor
    {
        get => _lineColor;
        private set => SetProperty(ref _lineColor, value);
    }

    public HelmDoorHintViewModel FirstDoor { get; }
    public HelmDoorHintViewModel SecondDoor { get; }

    public bool IsEnabled => _userSettingsService.HelmDoors;

    public HelmDoorPanelViewModel(
        string lineColor,
        IUserSettingsService userSettingsService,
        IBarrierService barrierService,
        ISavedProgressProvider savedProgressProvider,
        IItemSelectorService itemSelectorService
    )
    {
        LineColor = lineColor;
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        FirstDoor = new HelmDoorHintViewModel(HelmDoor.First, barrierService, savedProgressProvider, itemSelectorService);
        SecondDoor = new HelmDoorHintViewModel(HelmDoor.Second, barrierService, savedProgressProvider, itemSelectorService);

        _userSettingsService.PropertyChanged += OnSettingChanged;
    }

    private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.HelmDoors))
        {
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}
