using System.Collections.Concurrent;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services;

/// <summary>
/// Registry that maintains a mapping of region names to their vial slot providers (RegionViewModels).
/// Implemented as a stateful singleton service that collects region references as they are created.
///
/// This registry enables the orchestrator to query regions for available vial slots at placement time,
/// without requiring pre-registration or tight coupling to UI components like MainWindow.
/// </summary>
public class RegionSlotProviderRegistry : IRegionSlotProvider
{
    private readonly ConcurrentDictionary<RegionName, IRegionSlotProvider> _regionProviders;

    public RegionSlotProviderRegistry()
    {
        _regionProviders = new ConcurrentDictionary<RegionName, IRegionSlotProvider>();
    }

    /// <summary>
    /// Registers a region provider (typically a RegionViewModel) for the given region name.
    /// Called when a RegionViewModel is created or when a region becomes available.
    /// </summary>
    /// <param name="regionName">The region this provider serves</param>
    /// <param name="provider">The provider (typically RegionViewModel implementing GetVialSlots)</param>
    public void RegisterRegionProvider(RegionName regionName, IRegionSlotProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _regionProviders.AddOrUpdate(regionName, provider, (_, __) => provider);
    }

    /// <summary>
    /// Unregisters a region provider when a region is unloaded or destroyed.
    /// </summary>
    /// <param name="regionName">The region to unregister</param>
    /// <returns>True if a provider was removed; false if the region was not registered</returns>
    public bool UnregisterRegionProvider(RegionName regionName)
    {
        return _regionProviders.TryRemove(regionName, out _);
    }

    /// <summary>
    /// Gets all vial slots in a specific region by querying the registered provider.
    /// </summary>
    /// <param name="regionName">The region to query</param>
    /// <returns>Collection of vial slots from that region, or empty if region not found</returns>
    public IEnumerable<IVialSlot> GetVialSlotsForRegion(RegionName regionName)
    {
        if (_regionProviders.TryGetValue(regionName, out var provider))
        {
            return provider.GetVialSlotsForRegion(regionName);
        }

        return [];
    }
}
