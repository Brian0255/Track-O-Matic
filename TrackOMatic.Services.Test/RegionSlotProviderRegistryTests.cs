using Moq;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services.Test;

/// <summary>
/// Tests for RegionSlotProviderRegistry, which manages region providers for vial slot queries.
/// </summary>
public class RegionSlotProviderRegistryTests
{
    [Fact]
    public void Constructor_CreatesEmptyRegistry()
    {
        // Act
        var registry = new RegionSlotProviderRegistry();

        // Assert - should not throw
        Assert.NotNull(registry);
    }

    [Fact]
    public void RegisterRegionProvider_StoresProvider_ForRegionName()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var mockProvider = new Mock<IRegionSlotProvider>();
        var region = RegionName.JUNGLE_JAPES;

        // Act
        registry.RegisterRegionProvider(region, mockProvider.Object);

        // Assert - provider should be retrievable
        var vials = registry.GetVialSlotsForRegion(region);
        Assert.NotNull(vials);
    }

    [Fact]
    public void RegisterRegionProvider_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => registry.RegisterRegionProvider(region, null!));
        Assert.NotNull(ex);
    }

    [Fact]
    public void RegisterRegionProvider_MultipleTimes_OverwritesPreviousProvider()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;

        var mockProvider1 = new Mock<IRegionSlotProvider>();
        var mockProvider2 = new Mock<IRegionSlotProvider>();

        var vial1 = new Mock<IVialSlot>();
        var vial2 = new Mock<IVialSlot>();

        mockProvider1.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { vial1.Object });
        mockProvider2.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { vial2.Object });

        // Act
        registry.RegisterRegionProvider(region, mockProvider1.Object);
        var result1 = registry.GetVialSlotsForRegion(region);

        registry.RegisterRegionProvider(region, mockProvider2.Object);
        var result2 = registry.GetVialSlotsForRegion(region);

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        // The second registration should override
        Assert.Same(vial2.Object, result2.First());
    }

    [Fact]
    public void GetVialSlotsForRegion_WithRegisteredProvider_ReturnsVials()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;
        var mockProvider = new Mock<IRegionSlotProvider>();
        var mockVial = new Mock<IVialSlot>();

        mockProvider.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { mockVial.Object });

        registry.RegisterRegionProvider(region, mockProvider.Object);

        // Act
        var result = registry.GetVialSlotsForRegion(region);

        // Assert
        Assert.Single(result);
        Assert.Same(mockVial.Object, result.First());
    }

    [Fact]
    public void GetVialSlotsForRegion_WithUnregisteredRegion_ReturnsEmpty()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var unregisteredRegion = RegionName.JUNGLE_JAPES;

        // Act
        var result = registry.GetVialSlotsForRegion(unregisteredRegion);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void UnregisterRegionProvider_WithRegisteredRegion_RemovesProvider()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;
        var mockProvider = new Mock<IRegionSlotProvider>();

        mockProvider.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns([]);

        registry.RegisterRegionProvider(region, mockProvider.Object);

        // Act
        var removed = registry.UnregisterRegionProvider(region);
        var resultAfterRemoval = registry.GetVialSlotsForRegion(region);

        // Assert
        Assert.True(removed);
        Assert.Empty(resultAfterRemoval);
    }

    [Fact]
    public void UnregisterRegionProvider_WithUnregisteredRegion_ReturnsFalse()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var unregisteredRegion = RegionName.JUNGLE_JAPES;

        // Act
        var removed = registry.UnregisterRegionProvider(unregisteredRegion);

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public void MultipleRegions_EachMaintainSeparateProviders()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region1 = RegionName.JUNGLE_JAPES;
        var region2 = RegionName.ANGRY_AZTEC;

        var mockProvider1 = new Mock<IRegionSlotProvider>();
        var mockProvider2 = new Mock<IRegionSlotProvider>();

        var vial1 = new Mock<IVialSlot>();
        var vial2 = new Mock<IVialSlot>();

        mockProvider1.Setup(x => x.GetVialSlotsForRegion(region1))
            .Returns(new[] { vial1.Object });
        mockProvider2.Setup(x => x.GetVialSlotsForRegion(region2))
            .Returns(new[] { vial2.Object });

        // Act
        registry.RegisterRegionProvider(region1, mockProvider1.Object);
        registry.RegisterRegionProvider(region2, mockProvider2.Object);

        var result1 = registry.GetVialSlotsForRegion(region1);
        var result2 = registry.GetVialSlotsForRegion(region2);

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Same(vial1.Object, result1.First());
        Assert.Same(vial2.Object, result2.First());
    }

    [Fact]
    public void RegistryIsSingleton_CanBeSharedAcrossComponents()
    {
        // Arrange - simulate usage pattern where registry is injected to multiple components
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;
        var mockProvider = new Mock<IRegionSlotProvider>();
        var mockVial = new Mock<IVialSlot>();

        mockProvider.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { mockVial.Object });

        // Act - first component registers
        registry.RegisterRegionProvider(region, mockProvider.Object);

        // Second component queries (same registry instance)
        var result = registry.GetVialSlotsForRegion(region);

        // Assert
        Assert.Single(result);
        Assert.Same(mockVial.Object, result.First());
    }

    [Fact]
    public void GetVialSlotsForRegion_CallsProviderWithCorrectRegion()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;
        var mockProvider = new Mock<IRegionSlotProvider>();
        var mockVial = new Mock<IVialSlot>();

        mockProvider.Setup(x => x.GetVialSlotsForRegion(It.IsAny<RegionName>()))
            .Returns(new[] { mockVial.Object });

        registry.RegisterRegionProvider(region, mockProvider.Object);

        // Act
        registry.GetVialSlotsForRegion(region);

        // Assert - verify the provider was called with the correct region
        mockProvider.Verify(x => x.GetVialSlotsForRegion(region), Times.Once);
    }

    [Fact]
    public void UnregisterAndReregister_WorksCorrectly()
    {
        // Arrange
        var registry = new RegionSlotProviderRegistry();
        var region = RegionName.JUNGLE_JAPES;

        var mockProvider1 = new Mock<IRegionSlotProvider>();
        var mockProvider2 = new Mock<IRegionSlotProvider>();

        var vial1 = new Mock<IVialSlot>();
        var vial2 = new Mock<IVialSlot>();

        mockProvider1.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { vial1.Object });
        mockProvider2.Setup(x => x.GetVialSlotsForRegion(region))
            .Returns(new[] { vial2.Object });

        // Act - register first provider
        registry.RegisterRegionProvider(region, mockProvider1.Object);
        var result1 = registry.GetVialSlotsForRegion(region);

        // Unregister
        registry.UnregisterRegionProvider(region);
        var resultAfterUnregister = registry.GetVialSlotsForRegion(region);

        // Register different provider
        registry.RegisterRegionProvider(region, mockProvider2.Object);
        var result2 = registry.GetVialSlotsForRegion(region);

        // Assert
        Assert.Single(result1);
        Assert.Empty(resultAfterUnregister);
        Assert.Single(result2);
        Assert.Same(vial1.Object, result1.First());
        Assert.Same(vial2.Object, result2.First());
    }
}
