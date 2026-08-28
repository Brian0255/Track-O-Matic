using System.ComponentModel;
using System.Windows;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;

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
    private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

    /// <summary>
    /// Proxy property for XAML binding to TopMost setting.
    /// </summary>
    public bool TopMostSetting
    {
        get => UserSettings.TopMost;
        set
        {
            if (UserSettings.TopMost != value)
            {
                UserSettings.TopMost = value;
                OnPropertyChanged(nameof(TopMostSetting));
            }
        }
    }

    /// <summary>
    /// Proxy property for XAML binding to Helm & K. Rool broadcast setting.
    /// </summary>
    public bool BroadcastHelmKRoolSetting
    {
        get => UserSettings.BroadcastHelmKRool;
        set
        {
            if (UserSettings.BroadcastHelmKRool != value)
            {
                UserSettings.BroadcastHelmKRool = value;
                OnPropertyChanged(nameof(BroadcastHelmKRoolSetting));
            }
        }
    }

    /// <summary>
    /// Proxy property for XAML binding to shopkeepers broadcast setting.
    /// </summary>
    public bool BroadcastShopkeepersSetting
    {
        get => UserSettings.BroadcastShopkeepers;
        set
        {
            if (UserSettings.BroadcastShopkeepers != value)
            {
                UserSettings.BroadcastShopkeepers = value;
                OnPropertyChanged(nameof(BroadcastShopkeepersSetting));
            }
        }
    }

    /// <summary>
    /// Proxy property for XAML binding to song display broadcast setting.
    /// </summary>
    public bool BroadcastSongDisplaySetting
    {
        get => UserSettings.BroadcastSongDisplay;
        set
        {
            if (UserSettings.BroadcastSongDisplay != value)
            {
                UserSettings.BroadcastSongDisplay = value;
                OnPropertyChanged(nameof(BroadcastSongDisplaySetting));
            }
        }
    }

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

    public BroadcastView(IUserSettingsService userSettings, IParsedSpoilerDataService parsedSpoilerDataService)
    {
        UserSettings = userSettings;
        ParsedSpoilerDataService = parsedSpoilerDataService;
        UserSettings.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.TopMost))
            {
                OnPropertyChanged(nameof(TopMostSetting));
            }
            else if (e.PropertyName == nameof(IUserSettingsService.BroadcastHelmKRool))
            {
                OnPropertyChanged(nameof(BroadcastHelmKRoolSetting));
            }
            else if (e.PropertyName == nameof(IUserSettingsService.BroadcastShopkeepers))
            {
                OnPropertyChanged(nameof(BroadcastShopkeepersSetting));
            }
            else if (e.PropertyName == nameof(IUserSettingsService.BroadcastSongDisplay))
            {
                OnPropertyChanged(nameof(BroadcastSongDisplaySetting));
            }
            else if (e.PropertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
            {
                OnPropertyChanged(nameof(BroadcastNumberLabelSetting));
                AdjustLayout();
            }
        };
        ParsedSpoilerDataService.ParsedSpoilerDataChanged += (s, e) => AdjustLayout();

        InitializeComponent();
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

    public void AdjustLayout()
    {
        var pointsEnabled = ParsedSpoilerDataService.HasItemPoints();
        MovesWidth.Width = new GridLength(pointsEnabled ? 315 : 345, GridUnitType.Pixel);
    }
}
