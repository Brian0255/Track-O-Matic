using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

public class EndGameMappings
{
    /// <summary>
    /// Map each end game kong/boss to the resource ID to eventually get the correct image.
    /// </summary>
    /// <param name="index">The index of the end game kong/boss.</param>
    /// <returns>The resource ID of the corresponding image.</returns>
    public static string EndGameIndexToImageResource(int index)
    {
        return index switch
        {
            1 => "donkey",
            2 => "diddy",
            3 => "lanky",
            4 => "tiny",
            5 => "chunky",
            6 => "army",
            7 => "doga",
            8 => "madjack",
            9 => "pufftoss",
            10 => "doga2",
            11 => "army2",
            12 => "kutout",
            _ => "unknown_kong_bw",
        };
    }

    public static string EndGameIndexToImageResource(Bosses boss) => EndGameIndexToImageResource((int)boss);

    /// <summary>
    /// Maps each B. Locker and Hideout Helm door to the resource ID to eventually get the correct image.
    /// </summary>
    /// <param name="index">The index of the door.</param>
    /// <returns>The resource ID of the corresponding image.</returns>
    public static string DoorIndexToImageResource(int index)
    {
        return index switch
        {
            1 => "blueprint",
            2 => "pearl",
            3 => "crown",
            4 => "medal",
            5 => "rainbow_coin",
            6 => "fairy",
            7 => "company_coin",
            8 => "bean",
            _ => "golden_banana",
        };
    }

    public static string DoorIndexToImageResource(BarrierItems item) => DoorIndexToImageResource((int)item);

    public static readonly List<RegionName> LOBBY_ORDER =
    [
        RegionName.JUNGLE_JAPES,
        RegionName.ANGRY_AZTEC,
        RegionName.FRANTIC_FACTORY,
        RegionName.GLOOMY_GALLEON,
        RegionName.FUNGI_FOREST,
        RegionName.CRYSTAL_CAVES,
        RegionName.CREEPY_CASTLE,
        RegionName.HIDEOUT_HELM
    ];

    public static readonly HashSet<RegionName> ValidMoveRegions =
    [
        RegionName.JUNGLE_JAPES, RegionName.ANGRY_AZTEC, RegionName.FRANTIC_FACTORY,
        RegionName.GLOOMY_GALLEON, RegionName.FUNGI_FOREST, RegionName.CRYSTAL_CAVES,
        RegionName.CREEPY_CASTLE, RegionName.HIDEOUT_HELM, RegionName.START,
        RegionName.DK_ISLES, RegionName.UNHINTABLE_MOVES
    ];

    public static readonly HashSet<ItemName> BarrelPadItems =
    [
        ItemName.STRONG_KONG,
        ItemName.ROCKETBARREL_BOOST,
        ItemName.ORANGSTAND,
        ItemName.MINI_MONKEY,
        ItemName.HUNKY_CHUNKY,
        ItemName.BABOON_BLAST,
        ItemName.SIMIAN_SPRING,
        ItemName.MONKEYPORT,
        ItemName.GORILLA_GONE
    ];
}
