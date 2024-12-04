using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Text.RegularExpressions;
using TrackOMatic.Properties;

namespace TrackOMatic
{
    public class SpoilerParser
    {
        private int slamCount = 0;
        public int RNGSeed = 0;
        public MainWindow MainWindow { get; }
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; } = new();
        public Dictionary<ItemName, RegionName> TrainingItems { get; private set; } = new();
        public SpoilerParser(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        private string CheckForSlam(string itemString)
        {
            if (itemString != "Progressive Slam") return itemString;
            slamCount++;
            itemString += " " + slamCount;
            return itemString;
        }
        private bool ValidItemAndRegionString(string regionString, string itemString)
        {
            if (!JSONKeyMappings.ITEM_MAP.ContainsKey(itemString)) return false;
            if (!JSONKeyMappings.REGION_MAP.ContainsKey(regionString)) return false;
            return true;
        }
        private void CheckForTrainingItem(string itemString, ItemName itemName)
        {
            if(itemString.Contains("Training Barrel") || itemString.Contains("Pre-Given Move"))
            {
                TrainingItems[itemName] = RegionName.DK_ISLES;
            }
        }
        private void ReadSpecialItemRegion(Dictionary<string, string> region)
        {
            foreach(var entry in region)
            {
                var itemLocation = entry.Key;
                var itemString = entry.Value;
                itemString = CheckForSlam(itemString);
                if (!ValidItemAndRegionString(itemLocation, itemString)) continue;
                var regionName = JSONKeyMappings.REGION_MAP[itemLocation];
                var itemName = JSONKeyMappings.ITEM_MAP[itemString];
                MainWindow.ITEM_NAME_TO_REGION[itemName] = regionName;
            }
        }

        private void ReadShops(Dictionary<string, string> shopRegion)
        {
            foreach(var entry in shopRegion)
            {
                var fullShopName = entry.Key;
                var itemString = entry.Value;
                itemString = CheckForSlam(itemString);
                var pricePattern = @"^(.*)\s+\(";
                Match match = Regex.Match(itemString, pricePattern);
                if (!match.Success) continue;
                var itemWithoutPrice = match.Groups[1].Value;
                var shortenedShopName = fullShopName.Split(' ')[0];
                if (!JSONKeyMappings.ITEM_MAP.ContainsKey(itemWithoutPrice)) continue;
                var itemName = JSONKeyMappings.ITEM_MAP[itemWithoutPrice];
                MainWindow.ITEM_NAME_TO_REGION[itemName] = JSONKeyMappings.SHORTENED_SHOP_TO_REGION[shortenedShopName];
            }
        }

        private void ReadStandardRegion(string regionString, Dictionary<string, string> itemList)
        {
            foreach(var entry in itemList)
            {
                var itemLocation = entry.Key;
                var itemString = entry.Value;
                itemString = CheckForSlam(itemString);
                if (!ValidItemAndRegionString(regionString, itemString)) continue;
                var regionName = JSONKeyMappings.REGION_MAP[regionString];
                var itemName = JSONKeyMappings.ITEM_MAP[itemString];
                CheckForTrainingItem(itemLocation, itemName);
                MainWindow.ITEM_NAME_TO_REGION[itemName] = regionName;
            }
        }

        private void ReadItems(dynamic JSONObject)
        {
            if (JSONObject["Items"] == null) return;
            var items = JSONObject["Items"].ToObject<Dictionary<string, Dictionary<string, string>>>();
            foreach(var entry in items)
            {
                var itemArea = entry.Key;
                var itemList = entry.Value;
                switch (itemArea)
                {
                    case "Shops": 
                        ReadShops(itemList);
                        break;
                    case "Special":
                    case "Kongs":
                        ReadSpecialItemRegion(itemList);
                        break;
                    default:
                        ReadStandardRegion(itemArea, itemList);
                        break;
                }
            }
        }

        private void ReadStartingItemsIntoUI()
        {
            var childElements = new List<UIElement>(MainWindow.Items.Children.Count);
            foreach (UIElement child in MainWindow.Items.Children)
            {
                childElements.Add(child);
            }
            foreach (var itemControl in childElements)
            {
                if (itemControl is Item item && StartingItems.ContainsKey((ItemName)item.Tag))
                {
                    var itemName = (ItemName)item.Tag;
                    var region = StartingItems[itemName];
                    MainWindow.Regions[region].RegionGrid.Add_Item(item);
                    MainWindow.DataSaver.AddSavedItem(new SavedItem(itemName,  region, item.Star.Visibility, true, 1.0));
                    MainWindow.DataSaver.Save();

                    item.ItemImage = (Image)MainWindow.FindResource(itemName.ToString().ToLower());
                }
            }
        }

        private void GenerateHitList()
        {
            List<string> possibleGoals = Enum.GetValues(typeof(HitListGoal)).Cast<HitListGoal>().Select(e => e.ToString()).ToList();
            possibleGoals.Shuffle(RNGSeed);
            for(int i = 0; i < MainWindow.HitListItems.Count; ++i)
            {
                var imagePath = "Images/dk64/" + possibleGoals[i].ToLower() + ".png";
                var newImage = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                MainWindow.HitListItems[i].SetImage(newImage);
            }
        }

        private class RegionSpoilerInfo {
            public string level_name { get; }
            public int level_order { get; }
            public List<string> vial_colors { get; }
            public int points { get; }
            public int woth_count { get; }

            public RegionSpoilerInfo(string level_name, int level_order, List<string> vial_colors, int points, int woth_count)
            {
                this.level_name = level_name;
                this.level_order = level_order;
                this.vial_colors = vial_colors;
                this.points = points;
                this.woth_count = woth_count;
            }
        }

        private class StartingInfo
        {
            public List<int> krool_order { get; }
            public List<int> helm_order { get; }
            public List<int> starting_kongs { get; }
            public List<string> starting_keys { get; }
            public List<int> level_order { get; }

            public StartingInfo(List<int> krool_order, List<int> helm_order, List<int> starting_kongs, List<string> starting_keys, List<int> level_order)
            {
                this.krool_order = krool_order;
                this.helm_order = helm_order;
                this.starting_kongs = starting_kongs;
                this.starting_keys = starting_keys;
                this.level_order = level_order;
            }
        }

        private void ReadStartingInfo(string JSONString)
        {
            StartingInfo info = System.Text.Json.JsonSerializer.Deserialize<StartingInfo>(JSONString);
            ReadKongsAndKeys(info);
            ReadStartingItemsIntoUI();
            ReadHelmAndKRoolOrder(info);
            ReadLevelOrder(info);
        }

        private void ReadKongsAndKeys(StartingInfo info)
        {
            foreach (var kongIndex in info.starting_kongs)
            {
                var kongItem = JSONKeyMappings.KONGS[kongIndex];
                StartingItems.Add(kongItem, RegionName.START);
            }
            foreach (var keyString in info.starting_keys)
            {
                var key = JSONKeyMappings.ITEM_MAP[keyString];
                StartingItems.Add(key, RegionName.START);
            }
        }

        private void ReadHelmAndKRoolOrder(StartingInfo info)
        {
            if (Settings.Default.HelmOrder)
            {
                for (int i = 0; i < MainWindow.HelmKongs.Count; ++i)
                {
                    if (i < info.helm_order.Count)
                    {
                        var imageName = JSONKeyMappings.KONGS[info.helm_order[i]].ToString().ToLower();
                        MainWindow.HelmKongs[i].SetImage(new BitmapImage(new Uri("Images/dk64/" + imageName + ".png", UriKind.Relative)));
                        MainWindow.HelmKongs[i].Enabled = false;
                        MainWindow.HelmKongs[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MainWindow.HelmKongs[i].Visibility = Visibility.Hidden;
                    }
                }
            }
            if (Settings.Default.KRoolOrder)
            {
                for (int i = 0; i < MainWindow.KroolKongs.Count; ++i)
                {
                    if (i < info.krool_order.Count)
                    {
                        int kroolIndex = info.krool_order[i];
                        string imageName = "";
                        if (JSONKeyMappings.KROOL_MAP_TO_IMAGE.ContainsKey(kroolIndex))
                        {
                            imageName = JSONKeyMappings.KROOL_MAP_TO_IMAGE[kroolIndex];
                        }
                        else if (kroolIndex < JSONKeyMappings.KONGS.Count)
                        {
                            imageName = JSONKeyMappings.KONGS[kroolIndex].ToString();
                        }
                        imageName = imageName.ToLower();
                        MainWindow.KroolKongs[i].SetImage(new BitmapImage(new Uri("Images/dk64/" + imageName + ".png", UriKind.Relative)));
                        MainWindow.KroolKongs[i].Enabled = false;
                        MainWindow.KroolKongs[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MainWindow.KroolKongs[i].Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void ReadLevelOrder(StartingInfo info)
        {
            for(int i = 0; i < Region.LOBBY_ORDER.Count; ++i)
            {
                var levelOrderNumber = (info.level_order == null || i >= info.level_order.Count) ? 0 : info.level_order[i];
                var newLevelOrderNumber = (info.level_order == null || i >= info.level_order.Count) ? 0 : (i + 1);
                var toChange = (info.level_order == null || i >= info.level_order.Count) ? Region.LOBBY_ORDER[i] : Region.LOBBY_ORDER[levelOrderNumber];
                MainWindow.Regions[toChange].SetLevelOrderNumber(newLevelOrderNumber);
                if(info.level_order != null && info.level_order.Count == 7)
                {
                    MainWindow.Regions[RegionName.HIDEOUT_HELM].SetLevelOrderNumber(8);
                }
            }
        }

        private void ReadPointSpread(string JSONString)
        {
            var pointPairs = JsonConvert.DeserializeObject <Dictionary<string, int>>(JSONString);
            if (!pointPairs.ContainsKey("fairy_moves"))
            {
                pointPairs["fairy_moves"] = pointPairs["training_moves"];
            }
            foreach(var pair in pointPairs)
            {
                var name = pair.Key;
                var pointValue = pair.Value;
                if (JSONKeyMappings.POINT_NAME_TO_GROUP.ContainsKey(name))
                {
                    var itemType = JSONKeyMappings.POINT_NAME_TO_GROUP[name];
                    PointValues.GroupedValues[itemType] = pointValue;
                }
                else if (JSONKeyMappings.POINT_NAME_TO_SPECIFIC_VALUE.ContainsKey(name))
                {
                    var itemName = JSONKeyMappings.POINT_NAME_TO_SPECIFIC_VALUE[name];
                    PointValues.SpecificValues[itemName] = pointValue;
                }
            }
        }
        private void CheckIfShopkeepersAreOn(dynamic JSONObject)
        {
            if (JSONObject["Item Pool"] == null) return;
            //first version of randomizer with this key is 4.0 so we don't need to check if version >= 4.0
            //if (JSONObject["Randomizer Version"] == null) return;
            List<string> items = JSONObject["Item Pool"].ToObject < List<string> >();
            if (items.Contains("Cranky") || items.Contains("Candy") || items.Contains("Funky") || items.Contains("Snide")) return;
            StartingItems.Add(ItemName.CRANKY, RegionName.START);
            StartingItems.Add(ItemName.CANDY, RegionName.START);
            StartingItems.Add(ItemName.FUNKY, RegionName.START);
            StartingItems.Add(ItemName.SNIDE, RegionName.START);
        }
        public SpoilerSettings ParseRegions(dynamic JSONObject)
        {
            var regionInfo = JSONObject["Spoiler Hints Data"].ToObject<Dictionary<string, string>>();
            SpoilerSettings settings = null;

            List<string> Isles_Vials = new();

            CheckIfShopkeepersAreOn(JSONObject);

            foreach (var regionEntry in regionInfo)
            {
                if(regionEntry.Key == "starting_info")
                {
                    ReadStartingInfo(regionEntry.Value);
                    continue;
                }
                else if(regionEntry.Key == "point_spread")
                {
                    ReadPointSpread(regionEntry.Value);
                    continue;
                }
                RegionSpoilerInfo info = System.Text.Json.JsonSerializer.Deserialize<RegionSpoilerInfo>(regionEntry.Value);
                if (!JSONKeyMappings.REGION_MAP.ContainsKey(info.level_name)) continue;
                settings ??= SetUpSettings(info);
                RegionName regionName = JSONKeyMappings.REGION_MAP[info.level_name];
                MainWindow.Regions[regionName].AddPoints(info.points);
                MainWindow.Regions[regionName].AddRequiredCheckTotal(info.woth_count);
                MainWindow.Regions[regionName].SpoilerSettings = settings;
                var grid = MainWindow.Regions[regionName].RegionGrid;
                if (regionName == RegionName.DK_ISLES)
                {
                    Isles_Vials = Isles_Vials.Concat(info.vial_colors).ToList();
                }
                else
                {
                    ProcessVials(info.vial_colors, grid);
                }
            }
            //jetpac goes into isles, and we want to make sure it stays sorted so you can't metagame that an unsorted vial at the end of DK Isles is jetpac
            ProcessVials(Isles_Vials, MainWindow.Regions[RegionName.DK_ISLES].RegionGrid);
            return settings;
        }

        private void ProcessVials(List<string> vial_colors, RegionGrid grid)
        {
            vial_colors.Sort((a, b) => JSONKeyMappings.VIAL_MAP[a] - JSONKeyMappings.VIAL_MAP[b]);
            foreach (var vial in vial_colors) grid.AddInitialVial(JSONKeyMappings.VIAL_MAP[vial]);
        }

        private SpoilerSettings SetUpSettings(RegionSpoilerInfo info)
        {
            SpoilerSettings settings;
            bool pointsEnabled = info.points != -1;
            bool vialsEnabled = !pointsEnabled;
            bool WOTHEnabled = info.woth_count != -1;
            settings = new SpoilerSettings(pointsEnabled, vialsEnabled, WOTHEnabled);
            return settings;
        }

        private void GenerateHitListSeed(string jsonString, string salt)
        {
            var toHash = jsonString + salt;
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));
            RNGSeed = BitConverter.ToInt32(hashBytes, 0);
        }

        private void ReadSettings(dynamic JSONObject)
        {
            var settingsDict = JSONObject["Settings"].ToObject<Dictionary<string, object>>();
            if (settingsDict["Shockwave Shuffle"] == null) return;
            var shockwaveShuffle = (string)settingsDict["Shockwave Shuffle"];
            if (shockwaveShuffle == "start_with")
            {
                StartingItems.Add(ItemName.FAIRY_CAMERA, RegionName.START);
                StartingItems.Add(ItemName.SHOCKWAVE, RegionName.START);
            }
        }

        public SpoilerSettings ParseSpoiler(string fileName)
        {
            var spoilerSettings = new SpoilerSettings();
            StartingItems = new();
            TrainingItems = new();
            slamCount = 0;
            using StreamReader reader = new(fileName);
            string json = reader.ReadToEnd();

            dynamic JSONObject = JsonConvert.DeserializeObject(json);
            if (JSONObject["Settings"] != null) ReadSettings(JSONObject);
            if (JSONObject["Spoiler Hints Data"] != null)
            {
                spoilerSettings = ParseRegions(JSONObject);
            }
            else
            {
                MainWindow.InitRegionsFromEmptySpoiler();
            }
            //ReadItems(JSONObject);
            foreach (var entry in ImportantCheckList.ITEMS) entry.Value.InitPointValue();
            if (Properties.Settings.Default.HitList)
            {
                //the chef has decreed a touch of salt to nearly eliminate the chances of a repeat hash
                string salt = JSONObject["Settings"]["Seed"].ToObject<string>();
                string dataToHash = JsonConvert.SerializeObject(JSONObject["Spoiler Hints Data"]);
                GenerateHitListSeed(dataToHash, salt);
                GenerateHitList();
            }
            return spoilerSettings;
        }
    }
}
