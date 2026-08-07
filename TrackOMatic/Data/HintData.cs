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
                if (HintRegion.ByFieldName.TryGetValue(entry.Value, out var region))
                {
                    UserShortcuts["Hint Regions"][entry.Key] = region.ShortName;
                }
            }
            foreach (var entry in UserShortcuts["Item Locations"].ToList())
            {
                if (HintLocation.ByFieldName.TryGetValue(entry.Value, out var location))
                {
                    UserShortcuts["Item Locations"][entry.Key] = location.ShortName;
                }
            }
            foreach (var entry in UserShortcuts["Moves"].ToList())
            {
                if (HintMove.ByFieldName.TryGetValue(entry.Value, out var move))
                {
                    UserShortcuts["Moves"][entry.Key] = move.ShortName;
                }
            }
        }

        private static void InitSortedRegions()
        {
            SortedRegions = HintRegion.All
                .Select(region => region.ShortName)
                .OrderBy(name => name)
                .ToList();
        }

        private static void InitSortedMoves()
        {
            SortedChecks = HintMove.All
                .Select(move => move.ShortName)
                .OrderBy(move => move)
                .ToList();
        }

        private static void InitSortedChecks()
        {
            SortedChecks = HintLocation.All
                .Select(loc => loc.ShortName)
                .OrderBy(name => name)
                .ToList();
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
