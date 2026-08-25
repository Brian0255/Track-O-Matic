using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.Logic.Models;
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
        // Existing application services
        services.AddSingleton<IUserSettingsService, UserSettingsService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();
        services.AddSingleton<IThemeService, BarrelPadThemeService>();

        // Unified spoiler deserialization and parsing service
        services.AddSingleton<ISpoilerService, SpoilerParserService>();

        // ISavedProgressProvider: Uses factory pattern because SavedProgress is mutable state
        // that changes when tracker is reset or new save data is loaded. The singleton provider
        // broadcasts state changes via ProgressChanged event, allowing subscribed services to
        // reinitialize their caches.
        services.AddSingleton<ISavedProgressProvider>(sp => new SavedProgressProvider(new SavedProgress()));
        services.AddSingleton<IDataPersistenceService, DataPersistenceService>();
        services.AddSingleton<IItemTrackingService, ItemTrackingService>();
        services.AddSingleton<IHintService>(sp => new HintService(sp.GetRequiredService<IItemTrackingService>(), sp.GetRequiredService<ISavedProgressProvider>()));
        services.AddSingleton<ICollectiblesService, CollectiblesService>();
        services.AddSingleton<IBarrierService, BarrierService>();
        services.AddSingleton<ITrackerStateService, TrackerStateService>();
        services.AddSingleton<IEndGameProgressionService, EndGameProgressionService>();
        services.AddSingleton<ILevelOrderService, LevelOrderService>();
        services.AddSingleton<ISpoilerLogService, SpoilerLogService>();
        services.AddSingleton<IParsedSpoilerDataService, ParsedSpoilerDataService>();

        services.AddSingleton<IShortcutService, ShortcutService>();

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
