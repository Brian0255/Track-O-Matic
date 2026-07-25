using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrackOMatic
{
    public static class HintRegion
    {
        public static readonly HintNameEntry ISLES_SHOPS = new("Isles Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry MAIN_ISLE = new("Main Isle", null, HintGroup.REGION_ISLES);
        public static readonly HintNameEntry OUTER_ISLES = new("Outer Isles", null, HintGroup.REGION_ISLES);
        public static readonly HintNameEntry KREM_ISLE = new("Krem Isle", null, HintGroup.REGION_ISLES);
        public static readonly HintNameEntry RAREWARE_BANANA_ROOM = new("Rareware Room", null, HintGroup.REGION_ISLES);
        public static readonly HintNameEntry JAPES_TO_FOREST_LOBBIES = new("Japes - Forest Lobbies", null, HintGroup.REGION_ISLES);
        public static readonly HintNameEntry CAVES_TO_HELM_LOBBIES = new("Caves - Helm Lobbies", null, HintGroup.REGION_ISLES);

        public static readonly HintNameEntry JAPES_MEDAL_REWARDS = new("Japes Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry JAPES_COLORED_BANANAS = new("Japes Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry JAPES_SHOPS = new("Japes Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry JAPES_LOWLANDS = new("Japes Lowlands", null, HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_HILLSIDE = new("Japes Hillside", null, HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_STORMY_TUNNEL = new("Japes Stormy Tunnel Area", null, HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_HIVE_TUNNEL = new("Hive Tunnel Area", null, HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_CAVES_AND_MINES = new("Japes Caves & Mines", null, HintGroup.REGION_JAPES);

        public static readonly HintNameEntry AZTEC_MEDAL_REWARDS = new("Aztec Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_COLORED_BANANAS = new("Aztec Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_SHOPS = new("Aztec Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_OASIS_AND_TOTEM_AREA = new("Aztec Oasis & Totem Area", null, HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_TINY_TEMPLE = new("Tiny Temple", null, HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_FIVE_DOOR_TEMPLE = new("5 Door Temple", null, HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_LLAMA_TEMPLE = new("Llama Temple", null, HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_TUNNELS = new("Various Aztec Tunnels", null, HintGroup.REGION_AZTEC);

        public static readonly HintNameEntry FACTORY_MEDAL_REWARDS = new("Factory Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_COLORED_BANANAS = new("Factory Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_SHOPS = new("Factory Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_START = new("Frantic Factory Foyer", null, HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_TESTING_AREA = new("Testing Area", null, HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_RESEARCH_DEVELOPMENT_AREA = new("R&D Area", null, HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_STORAGE_AND_ARCADE = new("Storage & Arcade", null, HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_PRODUCTION_ROOM = new("Production Room", null, HintGroup.REGION_FACTORY);

        public static readonly HintNameEntry GALLEON_MEDAL_REWARDS = new("Galleon Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_COLORED_BANANAS = new("Galleon Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_SHOPS = new("Galleon Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_CAVERNS = new("Galleon Caverns", null, HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_LIGHTHOUSE = new("Lighthouse Area", null, HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_SHIPYARD_OUTSKIRTS = new("Shipyard Outskirts", null, HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_TREASURE_ROOM = new("Treasure Room", null, HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_FIVE_DOOR_SHIP = new("5 Door Ship", null, HintGroup.REGION_GALLEON);

        public static readonly HintNameEntry FOREST_MEDAL_REWARDS = new("Forest Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry FOREST_COLORED_BANANAS = new("Forest Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry FOREST_SHOPS = new("Forest Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry FOREST_CENTER_AND_BEANSTALK = new("Forest Center & Beanstalk", null, HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_GIANT_MUSH_EXTERIOR = new("Giant Mushroom Exterior", null, HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_GIANT_MUSH_INSIDES = new("Giant Mushroom Insides", null, HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_OWL_TREE = new("Owl Tree Area", null, HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_MILLS_AREA = new("Forest Mills", null, HintGroup.REGION_FOREST);

        public static readonly HintNameEntry CAVES_MEDAL_REWARDS = new("Caves Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry CAVES_COLORED_BANANAS = new("Caves Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry CAVES_SHOPS = new("Caves Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry CAVES_MAIN_AREA = new("Main Caves Area", null, HintGroup.REGION_CAVES);
        public static readonly HintNameEntry CAVES_IGLOO = new("Igloo Area", null, HintGroup.REGION_CAVES);
        public static readonly HintNameEntry CAVES_CABINS = new("Cabins Area", null, HintGroup.REGION_CAVES);

        public static readonly HintNameEntry CASTLE_MEDAL_REWARDS = new("Castle Medal Rewards", null, HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_COLORED_BANANAS = new("Castle Colored Bananas", null, HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_SHOPS = new("Castle Shops", null, HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_SURROUNDINGS = new("Castle Surroundings", null, HintGroup.REGION_CASTLE);
        public static readonly HintNameEntry CASTLE_ROOMS = new("Castle Rooms", null, HintGroup.REGION_CASTLE);
        public static readonly HintNameEntry CASTLE_UNDERGROUND = new("Castle Underground", null, HintGroup.REGION_CASTLE);

        public static readonly HintNameEntry HIDEOUT_HELM = new("Hideout Helm", null, HintGroup.REGION_HELM);
        public static readonly HintNameEntry TROFF_N_SCOFF = new("Troff 'n' Scoff", null, HintGroup.NONE);
        public static readonly HintNameEntry JETPAC = new("Jetpac Game", null, HintGroup.REGION_ISLES);

        public static readonly HintNameEntry FIRST_EIGHTH_BLUEPRINT_REWARDS = new("1st to 8th Blueprint Rewards","1st to 8th BP Rewards",HintGroup.REGION_ISLES);
        public static readonly HintNameEntry NINTH_SIXTEENTH_BLUEPRINT_REWARDS = new("9th to 16th Blueprint Rewards", "9th to 16th BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry SEVENTEENTH_TWENTYFOURTH_BLUEPRINT_REWARDS = new("17th to 24th Blueprint Rewards", "17th to 24th BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry TWENTYFIFTH_THIRTYSECOND_BLUEPRINT_REWARDS = new("25th to 32nd Blueprint Rewards", "25th to 32nd BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry THIRTYTHIRD_FORTIETH_BLUEPRINT_REWARDS = new("33rd to 40th Blueprint Rewards", "33rd to 40th BP Rewards", HintGroup.REGION_ISLES);

        public static readonly IReadOnlyList<HintNameEntry> All = typeof(HintRegion)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(HintNameEntry))
        .Select(f => (HintNameEntry)f.GetValue(null))
        .ToList();

        public static readonly IReadOnlyDictionary<string, HintNameEntry> ByFieldName = typeof(HintRegion)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(HintNameEntry))
        .ToDictionary(f => f.Name, f => (HintNameEntry)f.GetValue(null));
    }
}