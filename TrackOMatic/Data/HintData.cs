using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public static Dictionary<RegionName, List<string>> REGIONS_WITHOUT_LEVEL_NAME = new()
        {
            {RegionName.DK_ISLES, new() },
            {RegionName.JUNGLE_JAPES, new() },
            {RegionName.ANGRY_AZTEC, new() },
            {RegionName.FRANTIC_FACTORY, new() },
            {RegionName.GLOOMY_GALLEON, new() },
            {RegionName.FUNGI_FOREST, new() },
            {RegionName.CRYSTAL_CAVES, new() },
            {RegionName.CREEPY_CASTLE, new() },
            {RegionName.HIDEOUT_HELM, new() },
        };
        public static List<string> MISC_DIRECT_HINT_TYPES = new()
        {
            "Chunky","Crate","Diddy","Dirt Patch","Donkey","Fairy","Kasplat","Lanky","Tiny"
        };
        public static readonly Dictionary<string, RegionName> SHORTENED_REGION_NAME_TO_REGION = new()
        {
            {"isles", RegionName.DK_ISLES },
            {"japes", RegionName.JUNGLE_JAPES },
            {"aztec", RegionName.ANGRY_AZTEC },
            {"factory", RegionName.FRANTIC_FACTORY },
            {"galleon", RegionName.GLOOMY_GALLEON },
            {"forest", RegionName.FUNGI_FOREST },
            {"caves", RegionName.CRYSTAL_CAVES},
            {"castle", RegionName.CREEPY_CASTLE },
            {"hideout", RegionName.HIDEOUT_HELM },
        };

        public static Dictionary<string, Dictionary<string, string>> UserShortcuts { get; private set; }
        public static List<string> SortedRegions { get; private set; }
        public static List<string> SortedMoves { get; private set; }
        public static List<string> SortedChecks { get; private set; }
        private static void CreateUserShortcuts()
        {
            var defaultShortcutsResource = "TrackOMatic.default_shortcuts.json";
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultShortcutsResource);
            if (stream != null)
            {
                using FileStream fileStream = File.Create("shortcuts.json");
                stream.CopyTo(fileStream);
            }
        }
        private static void InitUserShortcuts()
        {
            var userShortcutsFile = "shortcuts.json";
            if (!File.Exists(userShortcutsFile)) CreateUserShortcuts();
            using StreamReader reader = new(userShortcutsFile);
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

        public static void InitDirectItemHintList()
        {
            foreach(var hintRegion in SortedRegions)
            {
                var words = hintRegion.Split(' ');
                var firstWord = words[0].ToLower();
                if (firstWord == "troff") continue;
                //ignore something like "Aztec Colored Bananas" because that is only foolish hint relevant
                if (words.Length > 1 && words[1].ToLower() == "colored") continue;
                RegionName region = RegionName.DK_ISLES;
                var shortenedName = hintRegion;
                if (SHORTENED_REGION_NAME_TO_REGION.ContainsKey(firstWord))
                {
                    region = SHORTENED_REGION_NAME_TO_REGION[firstWord];
                    shortenedName = hintRegion.Substring(firstWord.Length).TrimStart();
                };
                REGIONS_WITHOUT_LEVEL_NAME[region].Add(shortenedName);
            }
            foreach (var key in REGIONS_WITHOUT_LEVEL_NAME.Keys.ToList())
            {

                var regionList = REGIONS_WITHOUT_LEVEL_NAME[key];
                regionList = regionList.Concat(MISC_DIRECT_HINT_TYPES).ToList();
                regionList.Sort();
                REGIONS_WITHOUT_LEVEL_NAME[key] = regionList;
            }
            foreach(var entry in REGIONS_WITHOUT_LEVEL_NAME)
            {
            
            }
        }

        public static void Init()
        {
            InitSortedRegions();
            InitUserShortcuts();
            InitSortedMoves();
            InitSortedChecks();
            InitDirectItemHintList();
        }

        static HintData()
        {
        }
    }
}
