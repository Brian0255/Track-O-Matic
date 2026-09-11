using System.ComponentModel;
using System.Windows;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic;

/// <summary>
/// Interaction logic for HintItemSelectionDialog.xaml
/// </summary>
public partial class BroadcastView : Window
{
    private IUserSettingsService UserSettings { get; init; }

    public BroadcastView(
        IUserSettingsService userSettings,
        IParsedSpoilerDataService parsedSpoilerDataService,
        IItemTrackingService itemTrackingService,
        ILevelOrderService levelOrderService,
        ISavedProgressProvider savedProgressProvider,
        IEndGameProgressionService endGameProgressionService,
        IThemeService themeService,
        ICollectiblesService collectiblesService,
        ISongService songService,
        IDataSaverService dataSaverService
    )
    {

        InitializeComponent();
        DataContext = new MainBroadcastViewModel(
            userSettings,
            parsedSpoilerDataService,
            itemTrackingService,
            levelOrderService,
            savedProgressProvider,
            endGameProgressionService,
            themeService,
            collectiblesService,
            songService,
            dataSaverService
        );
        UserSettings = userSettings;
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
