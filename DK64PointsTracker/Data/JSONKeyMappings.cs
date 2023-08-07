using System.Collections.Generic;

namespace DK64PointsTracker
{
    public static class JSONKeyMappings
    {
        public static readonly Dictionary<string, ItemName> ITEM_MAP = new()
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

    public static readonly Dictionary<string, RegionName> REGION_MAP = new()
    {
      {"Training", RegionName.DK_ISLES },
      {"DK Isles", RegionName.DK_ISLES},
      {"Jungle Japes", RegionName.JUNGLE_JAPES},
      {"Angry Aztec", RegionName.ANGRY_AZTEC},
      {"Frantic Factory", RegionName.FRANTIC_FACTORY},
      {"Gloomy Galleon", RegionName.GLOOMY_GALLEON},
      {"Fungi Forest", RegionName.FUNGI_FOREST},
      {"Crystal Caves", RegionName.CRYSTAL_CAVES},
      {"Creepy Castle", RegionName.CREEPY_CASTLE},
      {"Hideout Helm", RegionName.HIDEOUT_HELM},
      {"The End of Helm", RegionName.HIDEOUT_HELM }
    };

    public static readonly Dictionary<string, VialColor> VIAL_MAP = new()
    {
        {"Clear Vial", VialColor.CLEAR },
        {"Yellow Vial", VialColor.YELLOW },
        {"Red Vial", VialColor.RED },
        {"Blue Vial", VialColor.BLUE },
        {"Purple Vial", VialColor.PURPLE },
        {"Green Vial", VialColor.GREEN },
        {"Key", VialColor.KEY },
        {"Kong", VialColor.KONG }
    };

    public static readonly List<ItemName> KONGS = new()
    {
        ItemName.DONKEY, ItemName.DIDDY, ItemName.LANKY, ItemName.TINY, ItemName.CHUNKY
    };


    }

}
