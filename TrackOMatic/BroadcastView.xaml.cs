using System.ComponentModel;
using System.Windows;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic;

/// <summary>
/// Interaction logic for HintItemSelectionDialog.xaml
/// </summary>
public partial class BroadcastView : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Proxy Properties for User Settings

    private IUserSettingsService UserSettings { get; init; }

    /// <summary>
    /// Proxy property for XAML binding to the broadcast number label setting.
    /// </summary>
    public BroadcastNumberLabel BroadcastNumberLabelSetting
    {
        get => UserSettings.BroadcastNumberLabel;
        set
        {
            if (UserSettings.BroadcastNumberLabel != value)
            {
                UserSettings.BroadcastNumberLabel = value;
                OnPropertyChanged(nameof(BroadcastNumberLabelSetting));
            }
        }
    }

    #endregion

    public BroadcastView(
        IUserSettingsService userSettings,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        ISavedProgressProvider savedProgressProvider,
        IEndGameProgressionService endGameProgressionService
    )
    {

        InitializeComponent();
        DataContext = new MainBroadcastViewModel(
            userSettings,
            parsedSpoilerDataService,
            itemTrackingService,
            levelOrderService,
            savedProgressProvider,
            endGameProgressionService
        );
        UserSettings = userSettings;

        UserSettings.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
            {
                OnPropertyChanged(nameof(BroadcastNumberLabelSetting));
            }
        };
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        // Cancel the close if the setting is still true (user didn't toggle it off)
        // This prevents the cascade: window closing → setting false → handler tries to close again
        if (UserSettings.BroadcastViewActive)
        {
            e.Cancel = true;
            UserSettings.BroadcastViewActive = false;
        }
    }
}
