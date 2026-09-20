using TrackOMatic.Logic.Attributes;

namespace TrackOMatic.Logic.Enums;

public enum RegionName
{
    START = 0,

    [SpoilerLogLabel("DK Isles")]
    DK_ISLES,

    [SpoilerLogLabel("Jungle Japes")]
    JUNGLE_JAPES,

    [SpoilerLogLabel("Angry Aztec")]
    ANGRY_AZTEC,

    [SpoilerLogLabel("Frantic Factory")]
    FRANTIC_FACTORY,

    [SpoilerLogLabel("Gloomy Galleon")]
    GLOOMY_GALLEON,

    [SpoilerLogLabel("Fungi Forest")]
    FUNGI_FOREST,

    [SpoilerLogLabel("Crystal Caves")]
    CRYSTAL_CAVES,

    [SpoilerLogLabel("Creepy Castle")]
    CREEPY_CASTLE,

    [SpoilerLogLabel("Hideout Helm")]
    HIDEOUT_HELM,

    [SpoilerLogLabel("Shops")]
    SHOPS,

    UNKNOWN,

    UNHINTABLE_MOVES
}
