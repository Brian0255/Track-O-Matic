using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.AutoTracking.Windows;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.Ui;

namespace TrackOMatic;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private WpfResourceDictionaryProvider? _resourceDictionaryProvider;

    public App()
    {
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        services.AddWindowsAutotracking();

        // Register WPF UI services
        services.AddSingleton<IItemSelectorService, ItemSelectorServiceWpf>();
        services.AddSingleton<IHintItemSelectionService, HintItemSelectionServiceWpf>();

        _serviceProvider = services.BuildServiceProvider();
        ServiceLocator.Initialize(_serviceProvider);
        Dispatcher.UnhandledException += OnDispatcherUnhandledException;
    }

    private void App_Exit(object sender, ExitEventArgs e)
    {
    }

    void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var settingsService = _serviceProvider.GetRequiredService<IUserSettingsService>();
        var appStateService = _serviceProvider.GetRequiredService<IApplicationStateService>();
        var spoilerService = _serviceProvider.GetRequiredService<ISpoilerService>();
        var parsedSpoilerDataService = _serviceProvider.GetRequiredService<IParsedSpoilerDataService>();
        var autotrackingService = _serviceProvider.GetRequiredService<IAutotrackerService>();
        var themeService = _serviceProvider.GetRequiredService<IThemeService>();
        var levelOrderService = _serviceProvider.GetRequiredService<ILevelOrderService>();
        var savedProgressProvider = _serviceProvider.GetRequiredService<ISavedProgressProvider>();
        var dataSaverService = _serviceProvider.GetRequiredService<IDataSaverService>();

        _resourceDictionaryProvider = new WpfResourceDictionaryProvider(themeService);
        _resourceDictionaryProvider.UpdateResourceDictionaries();

        themeService.BarrelPadThemeChanged += (s, e) => _resourceDictionaryProvider.UpdateResourceDictionaries();

        MainWindow mainWindow = new
        (
            settingsService,
            appStateService,
            spoilerService,
            parsedSpoilerDataService,
            autotrackingService,
            themeService,
            savedProgressProvider,
            dataSaverService
        );
        mainWindow.Show();
    }
}
