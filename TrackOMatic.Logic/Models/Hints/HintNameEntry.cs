using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models.Hints;

/// <summary>
/// A record representing an explicit hint as defined by Donkey Kong 64 Randomizer.
/// Each hint belongs to a <see cref="HintGroup"/> and can optionally be given a
/// <see cref="ShortName"/> for UI purposes.
/// </summary>
public record HintNameEntry
{
    public string FullName { get; init; }
    public string ShortName { get; init; }
    public HintGroup HintGroup { get; init; }
    public HintNameEntry(string fullName, string shortName = "", HintGroup hintGroup = HintGroup.NONE)
    {
        FullName = fullName;
        ShortName = string.IsNullOrEmpty(shortName) ? fullName : shortName;
        HintGroup = hintGroup;
        //should be replaced but this works ok for now
        if (FullName.Contains("Enemy"))
        {
            HintGroup = HintGroup.ENEMY;
        }
    }
}
