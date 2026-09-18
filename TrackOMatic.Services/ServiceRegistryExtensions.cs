using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services;

/// <summary>
/// Extension methods for registering Track-O-Matic services in the dependency injection container.
/// </summary>
public static class ServiceRegistryExtensions
{
    /// <summary>
    /// Registers all application services (singletons and dependencies).
    /// </summary>
    public static IServiceCollection AddTrackOMaticServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataPersistenceService, DataPersistenceService>();
        services.AddSingleton<IUserSettingsService, UserSettingsService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();
        services.AddSingleton<IThemeService, BarrelPadThemeService>();
        services.AddSingleton<IBarrierService, BarrierService>();
        services.AddSingleton<IEndGameProgressionService, EndGameProgressionService>();
        services.AddSingleton<ILevelOrderService, LevelOrderService>();

        // Unified spoiler deserialization and parsing service
        services.AddSingleton<ISpoilerService, SpoilerParserService>();

        // ISavedProgressProvider: Uses factory pattern because SavedProgress is mutable state
        // that changes when tracker is reset or new save data is loaded. The singleton provider
        // broadcasts state changes via ProgressChanged event, allowing subscribed services to
        // reinitialize their caches.
        services.AddSingleton<ISavedProgressProvider>(sp => new SavedProgressProvider(new SavedProgress()));
        services.AddSingleton<IItemTrackingService, ItemTrackingService>();
        services.AddSingleton<IHintService>(sp => new HintService(
            sp.GetRequiredService<IItemTrackingService>(),
            sp.GetRequiredService<ISavedProgressProvider>()
        ));
        services.AddSingleton<IDataSaverService>(sp => new DataSaverService(
            sp.GetRequiredService<IDataPersistenceService>(),
            sp.GetRequiredService<IParsedSpoilerDataService>(),
            sp.GetRequiredService<IItemTrackingService>(),
            sp.GetRequiredService<IHintService>(),
            sp.GetRequiredService<ISavedProgressProvider>(),
            sp.GetRequiredService<ISpoilerService>(),
            sp.GetRequiredService<IBarrierService>(),
            sp.GetRequiredService<IEndGameProgressionService>(),
            sp.GetRequiredService<ILevelOrderService>()
        ));
        services.AddSingleton<ICollectiblesService, CollectiblesService>();
        services.AddSingleton<IParsedSpoilerDataService, ParsedSpoilerDataService>();

        // Hint data loader: loads hint data (shortcuts, regions, etc.) from sources
        services.AddSingleton<IHintDataLoader, HintDataLoader>();

        // Hint data provider: container for loaded hint data
        services.AddSingleton<IHintDataProvider>(sp =>
        {
            var loader = sp.GetRequiredService<IHintDataLoader>();
            var result = loader.LoadHintData();
            return new HintDataProvider(
                result.UserShortcuts,
                result.RegionsWithoutLevelName,
                result.SortedRegions,
                result.SortedMoves,
                result.SortedChecks
            );
        });

        services.AddSingleton<IShortcutService>(sp => new ShortcutService(
            sp.GetRequiredService<IUserSettingsService>(),
            sp.GetRequiredService<IHintDataProvider>()
        ));

        // Autotracking service: requires platform-specific implementations (IProcessMemoryReader, IEmulatorAttacher)
        // to be registered via platform registration (e.g., AddWindowsAutotracking())
        services.AddSingleton<ISongService, SongService>();
        services.AddSingleton<ITimerFactory, SystemTimerFactory>();
        services.AddSingleton<IAutotrackerService, AutotrackerService>();

        // Region slot provider registry: maintains mappings of regions to their vial slots.
        // RegionViewModels register themselves with this registry when created.
        services.AddSingleton<RegionSlotProviderRegistry>();
        services.AddSingleton<IRegionSlotProvider>(sp => sp.GetRequiredService<RegionSlotProviderRegistry>());

        services.AddSingleton<IRegionPlacementOrchestrator, RegionPlacementOrchestrator>();

        return services;
    }
}
