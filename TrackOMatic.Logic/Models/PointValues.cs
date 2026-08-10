using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// This class contains the point values for different item types and specific items in DK 64 Randomizer.
/// </summary>
/// <remarks>This class is a DI refactoring candidate due to its cross cutting concerns.</remarks>
public class PointValues
{
    public static Dictionary<ItemType, int> GroupedValues { get; set; } = [];
    public static Dictionary<ItemName, int> SpecificValues { get; set; } = [];
}
