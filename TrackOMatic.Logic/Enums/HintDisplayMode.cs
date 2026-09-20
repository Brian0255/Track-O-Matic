using TrackOMatic.Logic.Attributes;

namespace TrackOMatic.Logic.Enums;

public enum HintDisplayMode
{
    [LegacyName("Off")]
    Off = 0,

    [LegacyName("Multipath Hints")]
    MultipathHints = 1,

    [LegacyName("Direct Item Hints")]
    DirectItemHints = 2,
}
