using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// This class contains the mappings needed to parse spoiler log information from DK 64 Rando.
/// </summary>
/// <remarks>These mappings will likely be refactored to use DI in the future.</remarks>
public static class SpoilerParserMappings
{
    public static Dictionary<string, ItemName> ITEM_MAP { get; } = new()
    {
        {"Baboon Blast", ItemName.BABOON_BLAST },
        {"Grape", ItemName.GRAPE_SHOOTER },
        {"Pineapple", ItemName.PINEAPPLE_LAUNCHER},
        {"Diving", ItemName.DIVING},
        {"Chimpy Charge",ItemName.CHIMPY_CHARGE},
        {"Mini Monkey",ItemName.MINI_MONKEY},
        {"Orangstand",ItemName.ORANGSTAND},
        {"Pony Tail Twirl",ItemName.PONYTAIL_TWIRL},
        {"Primate Punch", ItemName.PRIMATE_PUNCH},
        {"Hunky Chunky", ItemName.HUNKY_CHUNKY},
        {"Sniper Sight",ItemName.SNIPER_SCOPE },
        {"Monkeyport", ItemName.MONKEYPORT},
        {"Strong Kong", ItemName.STRONG_KONG},
        {"Saxophone", ItemName.SAXOPHONE_SLAM},
        {"Gorilla Gone", ItemName.GORILLA_GONE},
        {"Guitar", ItemName.GUITAR_GAZUMP},
        {"Peanut", ItemName.PEANUT_POPGUNS},
        {"Bongos", ItemName.BONGO_BLAST},
        {"Progressive Slam 1", ItemName.PROGRESSIVE_SLAM_1},
        {"Progressive Slam 2", ItemName.PROGRESSIVE_SLAM_2},
        {"Progressive Slam 3", ItemName.PROGRESSIVE_SLAM_3},
        {"Coconut",ItemName.COCONUT_GUN},
        {"Simian Spring", ItemName.SIMIAN_SPRING},
        {"Triangle", ItemName.TRIANGLE_TRAMPLE},
        {"Orangstand Sprint",ItemName.ORANGSTAND_SPRINT },
        {"Oranges",ItemName.ORANGE_THROWING},
        {"Homing Ammo",ItemName.HOMING_AMMO},
        {"Vines",ItemName.VINE_SWINGING},
        {"Rocketbarrel Boost",ItemName.ROCKETBARREL_BOOST},
        {"Baboon Balloon",ItemName.BABOON_BALLOON},
        {"Gorilla Grab",ItemName.GORILLA_GRAB },
        {"Feather",ItemName.FEATHER_BOW},
        {"Trombone",ItemName.TROMBONE_TREMOR},
        {"Barrels",ItemName.BARREL_THROWING},

        {"Donkey" ,ItemName.DONKEY },
        {"Diddy",ItemName.DIDDY},
        {"Lanky",ItemName.LANKY},
        {"Tiny",ItemName.TINY },
        {"Chunky",ItemName.CHUNKY},

        {"Key 1" ,ItemName.KEY_1 },
        {"Key 2" ,ItemName.KEY_2 },
        {"Key 3" ,ItemName.KEY_3 },
        {"Key 4" ,ItemName.KEY_4 },
        {"Key 5" ,ItemName.KEY_5 },
        {"Key 6" ,ItemName.KEY_6 },
        {"Key 7" ,ItemName.KEY_7 },
        {"Key 8" ,ItemName.KEY_8 },

        {"Bean" , ItemName.BEAN },

        {"Pearl 1", ItemName.PEARL_1 },
        {"Pearl 2", ItemName.PEARL_2 },
        {"Pearl 3", ItemName.PEARL_3 },
        {"Pearl 4", ItemName.PEARL_4 },
        {"Pearl 5", ItemName.PEARL_5 },

    };

    public static Dictionary<string, VialColor> VIAL_MAP { get; } = new()
    {
        {"Clear Vial", VialColor.CLEAR },
        {"Bean", VialColor.CLEAR },
        {"Yellow Vial", VialColor.YELLOW },
        {"Red Vial", VialColor.RED },
        {"Blue Vial", VialColor.BLUE },
        {"Purple Vial", VialColor.PURPLE },
        {"Green Vial", VialColor.GREEN },
        {"Key", VialColor.KEY },
        {"Kong", VialColor.KONG }
    };

    public static List<ItemName> KONGS { get; } =
    [
        ItemName.DONKEY, ItemName.DIDDY, ItemName.LANKY, ItemName.TINY, ItemName.CHUNKY
    ];

    public static Dictionary<int, Bosses> KROOL_MAP_TO_IMAGE_INDEX { get; } = new()
    {
        {0x53, Bosses.DOGA_2 },
        {0xC5, Bosses.DOGA_1 },
        {0x08, Bosses.ARMY_1 },
        {0xC4, Bosses.ARMY_2 },
        {0x6F, Bosses.PUFFTOSS },
        {0xC7, Bosses.KUTOUT },
        {0x9A, Bosses.MAD_JACK},
        {0xCB, Bosses.DONKEY },
        {0xCC, Bosses.DIDDY },
        {0xCD, Bosses.LANKY },
        {0xCE, Bosses.TINY },
        {0xCF, Bosses.CHUNKY },
    };

    public static Dictionary<string, PointCategory> POINT_NAME_TO_CATEGORY { get; } = new()
    {
        { "kongs", PointCategory.Kong },
        { "keys", PointCategory.Key },
        { "guns", PointCategory.Gun },
        { "instruments",PointCategory.Instrument },
        { "active_moves", PointCategory.PhysicalMove },
        { "pad_moves", PointCategory.PadMove },
        { "barrel_moves", PointCategory.BarrelMove },
        { "training_moves", PointCategory.TrainingMove },
        { "important_shared_moves",PointCategory.SharedMove },
        { "fairy_moves", PointCategory.FairyMove },
        { "shopkeepers", PointCategory.Shopkeeper },
        { "bean", PointCategory.Bean }
    };

    public static Dictionary<string, ItemName> RANDO_NAME_TO_ITEM_NAME { get; } = new()
    {
        {"Progressive Slam 1", ItemName.PROGRESSIVE_SLAM_1 },
        {"Progressive Slam 2", ItemName.PROGRESSIVE_SLAM_2 },
        {"Progressive Slam 3", ItemName.PROGRESSIVE_SLAM_3 },
        {"Coconut",ItemName.COCONUT_GUN },
        {"Bongos", ItemName.BONGO_BLAST },
        {"Baboon Blast", ItemName.BABOON_BLAST },
        {"Strong Kong", ItemName.STRONG_KONG },
        {"Gorilla Grab",ItemName.GORILLA_GRAB },
        {"Peanut",ItemName.PEANUT_POPGUNS },
        {"Guitar",ItemName.GUITAR_GAZUMP },
        {"Chimpy Charge",ItemName.CHIMPY_CHARGE },
        {"Simian Spring", ItemName.SIMIAN_SPRING },
        {"Rocketbarrel Boost",ItemName.ROCKETBARREL_BOOST },
        {"Grape",ItemName.GRAPE_SHOOTER },
        {"Trombone",ItemName.TROMBONE_TREMOR },
        {"Orangstand",ItemName.ORANGSTAND },
        {"Baboon Balloon", ItemName.BABOON_BALLOON },
        {"Orangstand Sprint", ItemName.ORANGSTAND_SPRINT },
        {"Feather",ItemName.FEATHER_BOW },
        {"Saxophone",ItemName.SAXOPHONE_SLAM },
        {"Mini Monkey",ItemName.MINI_MONKEY },
        {"Pony Tail Twirl",ItemName.PONYTAIL_TWIRL },
        {"Monkeyport",ItemName.MONKEYPORT },
        {"Pineapple",ItemName.PINEAPPLE_LAUNCHER },
        {"Triangle",ItemName.TRIANGLE_TRAMPLE },
        {"Hunky Chunky",ItemName.HUNKY_CHUNKY },
        {"Primate Punch",ItemName.PRIMATE_PUNCH },
        {"Gorilla Gone", ItemName.GORILLA_GONE },
        {"Homing Ammo", ItemName.HOMING_AMMO },
        {"Sniper Sight",ItemName.SNIPER_SCOPE },
        {"Oranges",ItemName.ORANGE_THROWING },
        {"Barrels",ItemName.BARREL_THROWING },
        {"Vines", ItemName.VINE_SWINGING },
        {"Diving", ItemName.DIVING },
        {"Cranky", ItemName.CRANKY },
        {"Funky", ItemName.FUNKY },
        {"Candy", ItemName.CANDY },
        {"Snide", ItemName.SNIDE },
        {"Climbing", ItemName.CLIMBING },
        {"Shockwave", ItemName.SHOCKWAVE },
        {"Fairy Camera", ItemName.FAIRY_CAMERA }
    };
}
