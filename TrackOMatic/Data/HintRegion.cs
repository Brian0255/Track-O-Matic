using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrackOMatic
{
    public static class HintRegion
    {
        public static readonly HintNameEntry ISLES_SHOPS = new("Isles Shops");
        public static readonly HintNameEntry MAIN_ISLE = new("Main Isle");
        public static readonly HintNameEntry OUTER_ISLES = new("Outer Isles");
        public static readonly HintNameEntry KREM_ISLE = new("Krem Isle");
        public static readonly HintNameEntry RAREWARE_BANANA_ROOM = new("Rareware Room");
        public static readonly HintNameEntry JAPES_TO_FOREST_LOBBIES = new("Japes - Forest Lobbies");
        public static readonly HintNameEntry CAVES_TO_HELM_LOBBIES = new("Caves - Helm Lobbies");

        public static readonly HintNameEntry JAPES_MEDAL_REWARDS = new("Japes Medal Rewards");
        public static readonly HintNameEntry JAPES_COLORED_BANANAS = new("Japes Colored Bananas");
        public static readonly HintNameEntry JAPES_SHOPS = new("Japes Shops");
        public static readonly HintNameEntry JAPES_LOWLANDS = new("Japes Lowlands");
        public static readonly HintNameEntry JAPES_HILLSIDE = new("Japes Hillside");
        public static readonly HintNameEntry JAPES_STORMY_TUNNEL = new("Japes Stormy Tunnel Area");
        public static readonly HintNameEntry JAPES_HIVE_TUNNEL = new("Hive Tunnel Area");
        public static readonly HintNameEntry JAPES_CAVES_AND_MINES = new("Japes Caves & Mines");

        public static readonly HintNameEntry AZTEC_MEDAL_REWARDS = new("Aztec Medal Rewards");
        public static readonly HintNameEntry AZTEC_COLORED_BANANAS = new("Aztec Colored Bananas");
        public static readonly HintNameEntry AZTEC_SHOPS = new("Aztec Shops");
        public static readonly HintNameEntry AZTEC_OASIS_AND_TOTEM_AREA = new("Aztec Oasis & Totem Area");
        public static readonly HintNameEntry AZTEC_TINY_TEMPLE = new("Tiny Temple");
        public static readonly HintNameEntry AZTEC_FIVE_DOOR_TEMPLE = new("5 Door Temple");
        public static readonly HintNameEntry AZTEC_LLAMA_TEMPLE = new("Llama Temple");
        public static readonly HintNameEntry AZTEC_TUNNELS = new("Various Aztec Tunnels");

        public static readonly HintNameEntry FACTORY_MEDAL_REWARDS = new("Factory Medal Rewards");
        public static readonly HintNameEntry FACTORY_COLORED_BANANAS = new("Factory Colored Bananas");
        public static readonly HintNameEntry FACTORY_SHOPS = new("Factory Shops");
        public static readonly HintNameEntry FACTORY_START = new("Frantic Factory Foyer");
        public static readonly HintNameEntry FACTORY_TESTING_AREA = new("Testing Area");
        public static readonly HintNameEntry FACTORY_RESEARCH_DEVELOPMENT_AREA = new("R&D Area");
        public static readonly HintNameEntry FACTORY_STORAGE_AND_ARCADE = new("Storage & Arcade");
        public static readonly HintNameEntry FACTORY_PRODUCTION_ROOM = new("Production Room");

        public static readonly HintNameEntry GALLEON_MEDAL_REWARDS = new("Galleon Medal Rewards");
        public static readonly HintNameEntry GALLEON_COLORED_BANANAS = new("Galleon Colored Bananas");
        public static readonly HintNameEntry GALLEON_SHOPS = new("Galleon Shops");
        public static readonly HintNameEntry GALLEON_CAVERNS = new("Galleon Caverns");
        public static readonly HintNameEntry GALLEON_LIGHTHOUSE = new("Lighthouse Area");
        public static readonly HintNameEntry GALLEON_SHIPYARD_OUTSKIRTS = new("Shipyard Outskirts");
        public static readonly HintNameEntry GALLEON_TREASURE_ROOM = new("Treasure Room");
        public static readonly HintNameEntry GALLEON_FIVE_DOOR_SHIP = new("5 Door Ship");

        public static readonly HintNameEntry FOREST_MEDAL_REWARDS = new("Forest Medal Rewards");
        public static readonly HintNameEntry FOREST_COLORED_BANANAS = new("Forest Colored Bananas");
        public static readonly HintNameEntry FOREST_SHOPS = new("Forest Shops");
        public static readonly HintNameEntry FOREST_CENTER_AND_BEANSTALK = new("Forest Center & Beanstalk");
        public static readonly HintNameEntry FOREST_GIANT_MUSH_EXTERIOR = new("Giant Mushroom Exterior");
        public static readonly HintNameEntry FOREST_GIANT_MUSH_INSIDES = new("Giant Mushroom Insides");
        public static readonly HintNameEntry FOREST_OWL_TREE = new("Owl Tree Area");
        public static readonly HintNameEntry FOREST_MILLS_AREA = new("Forest Mills");

        public static readonly HintNameEntry CAVES_MEDAL_REWARDS = new("Caves Medal Rewards");
        public static readonly HintNameEntry CAVES_COLORED_BANANAS = new("Caves Colored Bananas");
        public static readonly HintNameEntry CAVES_SHOPS = new("Caves Shops");
        public static readonly HintNameEntry CAVES_MAIN_AREA = new("Main Caves Area");
        public static readonly HintNameEntry CAVES_IGLOO = new("Igloo Area");
        public static readonly HintNameEntry CAVES_CABINS = new("Cabins Area");

        public static readonly HintNameEntry CASTLE_MEDAL_REWARDS = new("Castle Medal Rewards");
        public static readonly HintNameEntry CASTLE_COLORED_BANANAS = new("Castle Colored Bananas");
        public static readonly HintNameEntry CASTLE_SHOPS = new("Castle Shops");
        public static readonly HintNameEntry CASTLE_SURROUNDINGS = new("Castle Surroundings");
        public static readonly HintNameEntry CASTLE_ROOMS = new("Castle Rooms");
        public static readonly HintNameEntry CASTLE_UNDERGROUND = new("Castle Underground");

        public static readonly HintNameEntry HIDEOUT_HELM = new("Hideout Helm");
        public static readonly HintNameEntry TROFF_N_SCOFF = new("Troff 'n' Scoff");
        public static readonly HintNameEntry JETPAC = new("Jetpac Game");

        public static readonly HintNameEntry FIRST_EIGHTH_BLUEPRINT_REWARDS = new("1st to 8th Blueprint Rewards","1st to 8th BP Rewards");
        public static readonly HintNameEntry NINTH_SIXTEENTH_BLUEPRINT_REWARDS = new("9th to 16th Blueprint Rewards", "9th to 16th BP Rewards");
        public static readonly HintNameEntry SEVENTEENTH_TWENTYFOURTH_BLUEPRINT_REWARDS = new("17th to 24th Blueprint Rewards", "17th to 24th BP Rewards");
        public static readonly HintNameEntry TWENTYFIFTH_THIRTYSECOND_BLUEPRINT_REWARDS = new("25th to 32nd Blueprint Rewards", "25th to 32nd BP Rewards");
        public static readonly HintNameEntry THIRTYTHIRD_FORTIETH_BLUEPRINT_REWARDS = new("33rd to 40th Blueprint Rewards", "33rd to 40th BP Rewards");

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