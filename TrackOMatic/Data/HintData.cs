using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrackOMatic
{
    public static class HintData
    {
        public static Dictionary<HintRegion, string> HINT_REGION_TO_STRING = new()
        {
            {HintRegion.JAPES_TO_FOREST_LOBBIES, "Japes-Forest Lobbies" },
            {HintRegion.CAVES_TO_HELM_LOBBIES, "Caves-Helm Lobbies"},
            {HintRegion.AZTEC_FIVE_DOOR_TEMPLE, "Aztec 5-Door Temple" },
            {HintRegion.FACTORY_RESEARCH_DEVELOPMENT_AREA, "Factory R&D Area" },
            {HintRegion.GALLEON_FIVE_DOOR_SHIP, "Galleon 5-Door Ship"},
            {HintRegion.TROFF_N_SCOFF, "Troff N' Scoff" }
        };
        public static Dictionary<string, Dictionary<string, string>> UserShortcuts { get; private set; }
        public static List<string> SortedRegions { get; private set; }
        public static List<string> SortedMoves { get; private set; }
        public static List<string> SortedChecks { get; private set; }
        private static void InitUserShortcuts()
        {
            using StreamReader reader = new("shortcuts.json");
            string json = reader.ReadToEnd();
            UserShortcuts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            foreach (var entry in UserShortcuts["Hint Regions"].ToList())
            {
                if (Enum.TryParse(entry.Value, out HintRegion region))
                {
                    var hintRegion = HINT_REGION_TO_STRING[region];
                    UserShortcuts["Hint Regions"][entry.Key] = hintRegion;
                }
            }
        }

        private static void InitSortedRegions()
        {
            SortedRegions = new();
            var regions = (HintRegion[])Enum.GetValues(typeof(HintRegion));
            foreach (var hintRegion in regions)
            {
                var hintRegionString = hintRegion.ToString();
                if (HINT_REGION_TO_STRING.ContainsKey(hintRegion))
                {
                    SortedRegions.Add(HINT_REGION_TO_STRING[hintRegion]);
                    continue;
                }
                var textinfo = new CultureInfo("en-US", false).TextInfo;
                hintRegionString = hintRegionString.Replace("_AND_", "_&_");
                hintRegionString = hintRegionString.Replace("_", " ");
                hintRegionString = textinfo.ToTitleCase(hintRegionString.ToLower());
                HINT_REGION_TO_STRING[hintRegion] = hintRegionString;
                SortedRegions.Add(hintRegionString);
            }
            SortedRegions.Sort();
        }

        private static void InitSortedMoves()
        {
            SortedMoves = new();
            var acceptedItemTypes = new List<ItemType>
                    {
                        ItemType.SHARED_MOVE,
                        ItemType.TRAINING_MOVE,
                        ItemType.GUN,
                        ItemType.INSTRUMENT,
                        ItemType.PHYSICAL_MOVE,
                        ItemType.BARREL_MOVE,
                        ItemType.PAD_MOVE
                    };
            foreach (var entry in ImportantCheckList.ITEMS)
            {
                var info = entry.Value;
                if (acceptedItemTypes.Contains(info.ItemType))
                {
                    var moveString = info.ItemName.ToString();
                    var textinfo = new CultureInfo("en-US", false).TextInfo;
                    if (moveString.Contains("PROGRESSIVE_SLAM")) moveString = "PROGRESSIVE_SLAM";
                    moveString = moveString.Replace("_", " ");
                    moveString = textinfo.ToTitleCase(moveString.ToLower());
                    if (!SortedMoves.Contains(moveString))
                    {
                        SortedMoves.Add(moveString);
                    }
                }
            }
            SortedMoves.Sort();
        }

        private static void InitSortedChecks()
        {
            SortedChecks = new();
            foreach (var entry in UserShortcuts["Item Locations"])
            {
                SortedChecks.Add(entry.Value);
            }
            SortedChecks.Sort();
        }

        static HintData()
        {
            InitSortedRegions();
            InitUserShortcuts();
            InitSortedMoves();
            InitSortedChecks();
        }
    }
}
