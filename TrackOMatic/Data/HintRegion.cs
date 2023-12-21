using System.CodeDom;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace TrackOMatic
{
    public enum HintRegion
    {
        ISLES_SHOPS,
        MAIN_ISLE,
        OUTER_ISLES,
        KREM_ISLE,
        RAREWARE_BANANA_ROOM,
        JAPES_TO_FOREST_LOBBIES,
        CAVES_TO_HELM_LOBBIES,

        JAPES_MEDAL_REWARDS,
        JAPES_COLORED_BANANAS,
        JAPES_SHOPS,
        JAPES_LOWLANDS,
        JAPES_HILLSIDE,
        JAPES_STORMY_TUNNEL,
        JAPES_HIVE_TUNNEL,
        JAPES_CAVES_AND_MINES,

        AZTEC_MEDAL_REWARDS,
        AZTEC_COLORED_BANANAS,
        AZTEC_SHOPS,
        AZTEC_OASIS_AND_TOTEM_AREA,
        AZTEC_TINY_TEMPLE,
        AZTEC_FIVE_DOOR_TEMPLE,
        AZTEC_LLAMA_TEMPLE,
        AZTEC_TUNNELS,

        FACTORY_MEDAL_REWARDS,
        FACTORY_COLORED_BANANAS,
        FACTORY_SHOPS,
        FACTORY_START,
        FACTORY_TESTING_AREA,
        FACTORY_RESEARCH_DEVELOPMENT_AREA,
        FACTORY_STORAGE_AND_ARCADE,
        FACTORY_PRODUCTION_ROOM,

        GALLEON_MEDAL_REWARDS,
        GALLEON_COLORED_BANANAS,
        GALLEON_SHOPS,
        GALLEON_CAVERNS,
        GALLEON_LIGHTHOUSE,
        GALLEON_SHIPYARD_OUTSKIRTS,
        GALLEON_TREASURE_ROOM,
        GALLEON_FIVE_DOOR_SHIP,

        FOREST_MEDAL_REWARDS,
        FOREST_COLORED_BANANAS,
        FOREST_SHOPS,
        FOREST_CENTER_AND_BEANSTALK,
        FOREST_GIANT_MUSH_EXTERIOR,
        FOREST_GIANT_MUSH_INSIDES,
        FOREST_OWL_TREE,
        FOREST_MILLS,

        CAVES_MEDAL_REWARDS,
        CAVES_COLORED_BANANAS,
        CAVES_SHOPS,
        CAVES_MAIN_AREA,
        CAVES_IGLOO,
        CAVES_CABINS,

        CASTLE_MEDAL_REWARDS,
        CASTLE_COLORED_BANANAS,
        CASTLE_SHOPS,
        CASTLE_SURROUNDINGS,
        CASTLE_ROOMS,
        CASTLE_UNDERGROUND,

        HIDEOUT_HELM,
        TROFF_N_SCOFF,
        JETPAC
    }

    public static class HintRegionData
    {
        //for hint regions that are a bit weird to automatically format nicely
        public static readonly Dictionary<HintRegion, string> HINT_REGION_TO_STRING = new()
        {
            {HintRegion.JAPES_TO_FOREST_LOBBIES, "Japes-Forest Lobbies" },
            {HintRegion.CAVES_TO_HELM_LOBBIES, "Caves-Helm Lobbies"},
            {HintRegion.AZTEC_FIVE_DOOR_TEMPLE, "Aztec 5-Door Temple" },
            {HintRegion.FACTORY_RESEARCH_DEVELOPMENT_AREA, "Factory R&D Area" },
            {HintRegion.GALLEON_FIVE_DOOR_SHIP, "Galleon 5-Door Ship"},
            {HintRegion.TROFF_N_SCOFF, "Troff N' Scoff" }
        };
        private static List<string> sortedRegions;
        public static List<string> SortedRegions
        {
            get
            {
                if(sortedRegions == null)
                {
                    sortedRegions = new();
                    var regions = (HintRegion[])Enum.GetValues(typeof(HintRegion));
                    foreach(var hintRegion in regions)
                    {
                        var hintRegionString = hintRegion.ToString();
                        if (HINT_REGION_TO_STRING.ContainsKey(hintRegion))
                        {
                            sortedRegions.Add(HINT_REGION_TO_STRING[hintRegion]);
                            continue;
                        }
                        var textinfo = new CultureInfo("en-US", false).TextInfo;
                        hintRegionString = hintRegionString.Replace("_", " ");
                        hintRegionString = hintRegionString.Replace("AND", "&");
                        hintRegionString = textinfo.ToTitleCase(hintRegionString.ToLower());
                        sortedRegions.Add(hintRegionString);
                    }
                    sortedRegions.Sort();
                }
                return sortedRegions;
            }
        }
    }
}
