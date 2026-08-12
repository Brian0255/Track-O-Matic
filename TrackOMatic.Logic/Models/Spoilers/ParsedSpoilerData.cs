using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Spoilers;
public class ParsedSpoilerData
{
    /// <summary>
    /// Gets the spoiler data for each region in the parsed spoiler data.
    /// </summary>
    public Dictionary<RegionName, RegionSpoilerData> RegionData { get; set; } = [];

    /// <summary>
    /// Gets the starting items for the player in the parsed spoiler data.
    /// </summary>
    /// <remarks>This will primarily use the <see cref="RegionName.START"/> and <see cref="RegionName.UNHINTABLE_MOVES"/> regions.</remarks>
    public Dictionary<ItemName, RegionName> StartingItems { get; init; } = [];

    /// <summary>
    /// Gets the order of kongs needed to destroy the Blast-O-Matic in the parsed spoiler data.
    /// </summary>
    /// <remarks>
    ///1) The values within the list will (likely) be one more than the original spoiler log values
    ///   due to the presence of the Unknown Kong value in our <see cref="Bosses"/> enum.
    ///2) The max number of entries allowed is 5. If there are fewer, the UI should hide those slots.
    /// </remarks>
    public List<int> HelmOrder { get; init; } = [];

    /// <summary>
    /// Gets the order of bosses players have to defeat this seed in the parsed spoiler data.
    /// </summary>
    /// <remarks>
    /// 1) The values within the list will (likely) be one more than the original spoiler log values
    ///    due to the presence of the Unknown Kong value in our <see cref="Bosses"/> enum.
    /// 2) The max number of entries allowed is 5. If there are fewer, the UI should hide those slots.
    /// </remarks>
    public List<int> FinalBossOrder { get; init; } = [];

    /// <summary>
    /// Gets the indexed order of regions the player must traverse this seed.
    /// </summary>
    /// <remarks>
    /// 1) This does not guarantee that each level must be completed in a particular order
    ///    (or even visited), especially if the Complex Level Order setting is activated.
    /// 2) The numbers are converted to better match with the <see cref="RegionName"/> enum.
    /// 3) The max number of entries is allowed is 8. If there are 7 entries, then Helm
    ///    is guaranteed to be level 8.
    /// </remarks>
    public required Dictionary<RegionName, int> LevelOrder { get; init; } = [];

    public bool HasLevelOrder => LevelOrder.Count > 0;

    /// <summary>
    /// Gets the point spread for different categories of items in the parsed spoiler data.
    /// </summary>
    public Dictionary<PointCategory, int> PointSpread { get; init; } = [];

    /// <summary>
    /// Gets the toggleable spoiler settings used to generate the seed in the parsed spoiler data.
    /// </summary>
    public SpoilerSettings SpoilerSettings { get; init; } = new();

    /// <summary>
    /// Gets the B. Locker barrier information for each region in the parsed spoiler data.
    /// </summary>
    public List<BlockerInfoRaw> RegionBarrierInfo { get; init; } = [];
}
