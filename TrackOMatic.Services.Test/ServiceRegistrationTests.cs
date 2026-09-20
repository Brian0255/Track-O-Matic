using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.Logic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Sanity-check tests for DI container configuration.
/// These tests verify that interfaces can be registered and resolved, without testing implementation logic.
/// </summary>
public class ServiceRegistrationTests
{
    [Fact]
    public void ServiceCollection_AddTrackOMaticServices_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var ex = Record.Exception(() => services.AddTrackOMaticServices());
        Assert.Null(ex);
    }

    [Fact]
    public void ServiceProvider_AfterAddTrackOMaticServices_CanResolveExistingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act & Assert - these services exist and should resolve without throwing
        Assert.Multiple(() =>
        {
            Assert.NotNull(provider.GetRequiredService<IUserSettingsService>());
            Assert.NotNull(provider.GetRequiredService<IApplicationStateService>());
        });
    }

    #region Factory Pattern Tests

    private static void InitializeHintDataForTesting()
    {
        // Pre-initialize HintData to avoid loading resources during tests
        // This allows HintDataLoader to skip re-initialization
        var testShortcuts = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "Items", new Dictionary<string, string>
                {
                    { "BA", "BATTLE_ARENA" },
                    { "CC", "COLORED_COCONUT" }
                }
            },
            {
                "Moves", new Dictionary<string, string>
                {
                    { "CG", "CHUNKY_GRAB" },
                    { "DP", "DONKEY_POUND" }
                }
            }
        };

        TrackOMatic.Logic.Models.Hints.HintData.InitForTesting(testShortcuts);
    }

    [Fact]
    public void ServiceProvider_CanResolveSavedProgressProvider_WithFactoryPattern()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<ISavedProgressProvider>());

        // Assert
        Assert.Null(result);
        var savedProgressProvider = provider.GetRequiredService<ISavedProgressProvider>();
        Assert.NotNull(savedProgressProvider);
    }

    [Fact]
    public void ServiceProvider_InitializesSavedProgressProvider_WithEmptySavedProgress()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var savedProgressProvider = provider.GetRequiredService<ISavedProgressProvider>();

        // Assert
        Assert.NotNull(savedProgressProvider);
        // Verify it was initialized with empty progress (not null)
        Assert.NotNull(savedProgressProvider.CurrentProgress);
    }

    [Fact]
    public void ServiceProvider_CanResolveHintService_WithFactoryDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IHintService>());

        // Assert
        Assert.Null(result);
        var hintService = provider.GetRequiredService<IHintService>();
        Assert.NotNull(hintService);
    }

    [Fact]
    public void ServiceProvider_HintService_ReceivesRequiredDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var hintService = provider.GetRequiredService<IHintService>();
        var itemTrackingService = provider.GetRequiredService<IItemTrackingService>();
        var savedProgressProvider = provider.GetRequiredService<ISavedProgressProvider>();

        // Assert - verify they're instances (not null)
        Assert.NotNull(hintService);
        Assert.NotNull(itemTrackingService);
        Assert.NotNull(savedProgressProvider);
    }

    [Fact]
    public void ServiceProvider_CanResolveDataSaverService_WithAllDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IDataSaverService>());

        // Assert
        Assert.Null(result);
        var dataSaverService = provider.GetRequiredService<IDataSaverService>();
        Assert.NotNull(dataSaverService);
    }

    [Fact]
    public void ServiceProvider_DataSaverService_ReceivesAllNineDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var dataSaverService = provider.GetRequiredService<IDataSaverService>();

        // Assert - all these services must be registered for DataSaverService to resolve
        Assert.Multiple(() =>
        {
            Assert.NotNull(dataSaverService);
            Assert.NotNull(provider.GetRequiredService<IDataPersistenceService>());
            Assert.NotNull(provider.GetRequiredService<IParsedSpoilerDataService>());
            Assert.NotNull(provider.GetRequiredService<IItemTrackingService>());
            Assert.NotNull(provider.GetRequiredService<IHintService>());
            Assert.NotNull(provider.GetRequiredService<ISavedProgressProvider>());
            Assert.NotNull(provider.GetRequiredService<ISpoilerService>());
            Assert.NotNull(provider.GetRequiredService<IBarrierService>());
            Assert.NotNull(provider.GetRequiredService<IEndGameProgressionService>());
            Assert.NotNull(provider.GetRequiredService<ILevelOrderService>());
        });
    }

    [Fact]
    public void ServiceProvider_CanResolveHintDataProvider_WithFactoryAndHintDataLoader()
    {
        // Arrange
        InitializeHintDataForTesting();
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IHintDataProvider>());

        // Assert
        Assert.Null(result);
        var hintDataProvider = provider.GetRequiredService<IHintDataProvider>();
        Assert.NotNull(hintDataProvider);
    }

    [Fact]
    public void ServiceProvider_HintDataProvider_LoadsDataViaHintDataLoader()
    {
        // Arrange
        InitializeHintDataForTesting();
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var hintDataProvider = provider.GetRequiredService<IHintDataProvider>();
        var hintDataLoader = provider.GetRequiredService<IHintDataLoader>();

        // Assert - both should be instances
        Assert.NotNull(hintDataProvider);
        Assert.NotNull(hintDataLoader);
    }

    [Fact]
    public void ServiceProvider_CanResolveShortcutService_WithUserSettingsAndHintDataProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IShortcutService>());

        // Assert
        Assert.Null(result);
        var shortcutService = provider.GetRequiredService<IShortcutService>();
        Assert.NotNull(shortcutService);
    }

    [Fact]
    public void ServiceProvider_CanResolveRegionSlotProvider_FromRegistry()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IRegionSlotProvider>());

        // Assert
        Assert.Null(result);
        var regionSlotProvider = provider.GetRequiredService<IRegionSlotProvider>();
        Assert.NotNull(regionSlotProvider);
    }

    [Fact]
    public void ServiceProvider_RegionSlotProvider_IsRegistryInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var regionSlotProvider = provider.GetRequiredService<IRegionSlotProvider>();
        var registry = provider.GetRequiredService<RegionSlotProviderRegistry>();

        // Assert - the provider should be the same instance as the registry
        Assert.NotNull(regionSlotProvider);
        Assert.NotNull(registry);
        Assert.Same((object)regionSlotProvider, (object)registry);
    }

    [Fact]
    public void ServiceProvider_AllFactoryRegistrations_Succeed()
    {
        // Arrange
        InitializeHintDataForTesting();
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();

        // Act
        var provider = services.BuildServiceProvider();

        // Assert - verify all factory-pattern services can be resolved
        Assert.Multiple(() =>
        {
            Assert.NotNull(provider.GetRequiredService<ISavedProgressProvider>());
            Assert.NotNull(provider.GetRequiredService<IHintService>());
            Assert.NotNull(provider.GetRequiredService<IDataSaverService>());
            Assert.NotNull(provider.GetRequiredService<IHintDataProvider>());
            Assert.NotNull(provider.GetRequiredService<IShortcutService>());
            Assert.NotNull(provider.GetRequiredService<IRegionSlotProvider>());
        });
    }

    [Fact]
    public void ServiceProvider_Services_AreSingletons()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var userSettings1 = provider.GetRequiredService<IUserSettingsService>();
        var userSettings2 = provider.GetRequiredService<IUserSettingsService>();
        var hintService1 = provider.GetRequiredService<IHintService>();
        var hintService2 = provider.GetRequiredService<IHintService>();

        // Assert
        Assert.Same(userSettings1, userSettings2);
        Assert.Same(hintService1, hintService2);
    }

    [Fact]
    public void ServiceProvider_CanResolveRegionPlacementOrchestrator()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTrackOMaticServices();
        var provider = services.BuildServiceProvider();

        // Act
        var result = Record.Exception(() => provider.GetRequiredService<IRegionPlacementOrchestrator>());

        // Assert
        Assert.Null(result);
        var orchestrator = provider.GetRequiredService<IRegionPlacementOrchestrator>();
        Assert.NotNull(orchestrator);
    }

    #endregion
}
