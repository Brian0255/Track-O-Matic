using System.Collections.ObjectModel;
using System.Reflection;

namespace TrackOMatic
{
    public static class HintRegion
    {
        public static readonly HintNameEntry ISLES_SHOPS = new("Isles Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry MAIN_ISLE = new("Main Isle", hintGroup: HintGroup.REGION_ISLES);
        public static readonly HintNameEntry OUTER_ISLES = new("Outer Isles", hintGroup: HintGroup.REGION_ISLES);
        public static readonly HintNameEntry KREM_ISLE = new("Krem Isle", hintGroup: HintGroup.REGION_ISLES);
        public static readonly HintNameEntry RAREWARE_BANANA_ROOM = new("Rareware Room", hintGroup: HintGroup.REGION_ISLES);
        public static readonly HintNameEntry JAPES_TO_FOREST_LOBBIES = new("Japes - Forest Lobbies", hintGroup: HintGroup.REGION_ISLES);
        public static readonly HintNameEntry CAVES_TO_HELM_LOBBIES = new("Caves - Helm Lobbies", hintGroup: HintGroup.REGION_ISLES);

        public static readonly HintNameEntry JAPES_MEDAL_REWARDS = new("Japes Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry JAPES_COLORED_BANANAS = new("Japes Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry JAPES_SHOPS = new("Japes Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry JAPES_LOWLANDS = new("Japes Lowlands", hintGroup: HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_HILLSIDE = new("Japes Hillside", hintGroup: HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_STORMY_TUNNEL = new("Japes Stormy Tunnel Area", hintGroup: HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_HIVE_TUNNEL = new("Hive Tunnel Area", hintGroup: HintGroup.REGION_JAPES);
        public static readonly HintNameEntry JAPES_CAVES_AND_MINES = new("Japes Caves & Mines", hintGroup: HintGroup.REGION_JAPES);

        public static readonly HintNameEntry AZTEC_MEDAL_REWARDS = new("Aztec Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_COLORED_BANANAS = new("Aztec Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_SHOPS = new("Aztec Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry AZTEC_OASIS_AND_TOTEM_AREA = new("Aztec Oasis & Totem Area", hintGroup: HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_TINY_TEMPLE = new("Tiny Temple", hintGroup: HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_FIVE_DOOR_TEMPLE = new("5 Door Temple", hintGroup: HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_LLAMA_TEMPLE = new("Llama Temple", hintGroup: HintGroup.REGION_AZTEC);
        public static readonly HintNameEntry AZTEC_TUNNELS = new("Various Aztec Tunnels", hintGroup: HintGroup.REGION_AZTEC);

        public static readonly HintNameEntry FACTORY_MEDAL_REWARDS = new("Factory Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_COLORED_BANANAS = new("Factory Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_SHOPS = new("Factory Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FACTORY_START = new("Frantic Factory Foyer", hintGroup: HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_TESTING_AREA = new("Testing Area", hintGroup: HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_RESEARCH_DEVELOPMENT_AREA = new("R&D Area", hintGroup: HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_STORAGE_AND_ARCADE = new("Storage & Arcade", hintGroup: HintGroup.REGION_FACTORY);
        public static readonly HintNameEntry FACTORY_PRODUCTION_ROOM = new("Production Room", hintGroup: HintGroup.REGION_FACTORY);

        public static readonly HintNameEntry GALLEON_MEDAL_REWARDS = new("Galleon Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_COLORED_BANANAS = new("Galleon Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_SHOPS = new("Galleon Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry GALLEON_CAVERNS = new("Galleon Caverns", hintGroup: HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_LIGHTHOUSE = new("Lighthouse Area", hintGroup: HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_SHIPYARD_OUTSKIRTS = new("Shipyard Outskirts", hintGroup: HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_TREASURE_ROOM = new("Treasure Room", hintGroup: HintGroup.REGION_GALLEON);
        public static readonly HintNameEntry GALLEON_FIVE_DOOR_SHIP = new("5 Door Ship", hintGroup: HintGroup.REGION_GALLEON);

        public static readonly HintNameEntry FOREST_MEDAL_REWARDS = new("Forest Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FOREST_COLORED_BANANAS = new("Forest Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FOREST_SHOPS = new("Forest Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry FOREST_CENTER_AND_BEANSTALK = new("Forest Center & Beanstalk", hintGroup: HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_GIANT_MUSH_EXTERIOR = new("Giant Mushroom Exterior", hintGroup: HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_GIANT_MUSH_INSIDES = new("Giant Mushroom Insides", hintGroup: HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_OWL_TREE = new("Owl Tree Area", hintGroup: HintGroup.REGION_FOREST);
        public static readonly HintNameEntry FOREST_MILLS_AREA = new("Forest Mills", hintGroup: HintGroup.REGION_FOREST);

        public static readonly HintNameEntry CAVES_MEDAL_REWARDS = new("Caves Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CAVES_COLORED_BANANAS = new("Caves Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CAVES_SHOPS = new("Caves Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CAVES_MAIN_AREA = new("Main Caves Area", hintGroup: HintGroup.REGION_CAVES);
        public static readonly HintNameEntry CAVES_IGLOO = new("Igloo Area", hintGroup: HintGroup.REGION_CAVES);
        public static readonly HintNameEntry CAVES_CABINS = new("Cabins Area", hintGroup: HintGroup.REGION_CAVES);

        public static readonly HintNameEntry CASTLE_MEDAL_REWARDS = new("Castle Medal Rewards", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_COLORED_BANANAS = new("Castle Colored Bananas", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_SHOPS = new("Castle Shops", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry CASTLE_SURROUNDINGS = new("Castle Surroundings", hintGroup: HintGroup.REGION_CASTLE);
        public static readonly HintNameEntry CASTLE_ROOMS = new("Castle Rooms", hintGroup: HintGroup.REGION_CASTLE);
        public static readonly HintNameEntry CASTLE_UNDERGROUND = new("Castle Underground", hintGroup: HintGroup.REGION_CASTLE);

        public static readonly HintNameEntry HIDEOUT_HELM = new("Hideout Helm", hintGroup: HintGroup.REGION_HELM);
        public static readonly HintNameEntry TROFF_N_SCOFF = new("Troff 'n' Scoff", hintGroup: HintGroup.NONE);
        public static readonly HintNameEntry JETPAC = new("Jetpac Game", hintGroup: HintGroup.REGION_ISLES);

        public static readonly HintNameEntry FIRST_EIGHTH_BLUEPRINT_REWARDS = new("1st to 8th Blueprint Rewards", "1st to 8th BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry NINTH_SIXTEENTH_BLUEPRINT_REWARDS = new("9th to 16th Blueprint Rewards", "9th to 16th BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry SEVENTEENTH_TWENTYFOURTH_BLUEPRINT_REWARDS = new("17th to 24th Blueprint Rewards", "17th to 24th BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry TWENTYFIFTH_THIRTYSECOND_BLUEPRINT_REWARDS = new("25th to 32nd Blueprint Rewards", "25th to 32nd BP Rewards", HintGroup.REGION_ISLES);
        public static readonly HintNameEntry THIRTYTHIRD_FORTIETH_BLUEPRINT_REWARDS = new("33rd to 40th Blueprint Rewards", "33rd to 40th BP Rewards", HintGroup.REGION_ISLES);

        private static ReadOnlyCollection<HintNameEntry> InitializeAll() =>
            typeof(HintRegion)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(HintNameEntry))
                .Select(f => (HintNameEntry)f.GetValue(null)!)
                .ToList()
                .AsReadOnly();

        private static ReadOnlyDictionary<string, HintNameEntry> InitializeByFieldName() =>
            typeof(HintRegion)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(HintNameEntry))
                .ToDictionary(f => f.Name, f => (HintNameEntry)f.GetValue(null)!)
                .AsReadOnly();

        public static readonly IReadOnlyList<HintNameEntry> All = InitializeAll();

        public static readonly IReadOnlyDictionary<string, HintNameEntry> ByFieldName = InitializeByFieldName();
    }
}
