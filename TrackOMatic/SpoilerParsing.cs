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

namespace TrackOMatic
{
    public partial class MainWindow : Window
    {
        private void OpenSpoiler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            string lastFolderPath = Properties.Settings.Default.LastFolderPath;

            if (!string.IsNullOrEmpty(lastFolderPath))
            {
                openFileDialog.InitialDirectory = lastFolderPath;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string folderPath = Path.GetDirectoryName(selectedFilePath);

                Properties.Settings.Default.LastFolderPath = folderPath;
                Properties.Settings.Default.Save();

                ParseSpoiler(selectedFilePath);
            }
        }

        private void DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Path.GetExtension(files[0]).ToUpper().Equals(".JSON"))
                {
                    Reset();
                    ParseSpoiler(files[0]);
                }
            }
        }

        private void ReadStartingItemsIntoUI(Dictionary<ItemName, RegionName> startingItems)
        {
            var childElements = new List<UIElement>(Items.Children.Count);
            foreach (UIElement child in Items.Children)
            {
                childElements.Add(child);
            }
            foreach (var itemControl in childElements)
            {
                if (itemControl is Item item && startingItems.ContainsKey((ItemName)item.Tag))
                {
                    var itemName = (ItemName)item.Tag;
                    var region = startingItems[itemName];
                    Regions[region].RegionGrid.Add_Item(item);
                    item.CanLeftClick = false;
                }
            }
        }

        private void GenerateHitList(int randomSeed)
        {
            List<string> possibleGoals = Enum.GetValues(typeof(HitListGoal)).Cast<HitListGoal>().Select(e => e.ToString()).ToList();
            possibleGoals.Shuffle(randomSeed);
            for(int i = 0; i < 12; ++i)
            {
                var imagePath = "Images/dk64/" + possibleGoals[i].ToLower() + ".png";
                var newImage = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                hitListItems[i].SetImage(newImage);
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

            public StartingInfo(List<int> krool_order, List<int> helm_order, List<int> starting_kongs, List<string> starting_keys)
            {
                this.krool_order = krool_order;
                this.helm_order = helm_order;
                this.starting_kongs = starting_kongs;
                this.starting_keys = starting_keys;
            }
        }
        private void ReadStartingInfo(string JSONString)
        {
            StartingInfo info = System.Text.Json.JsonSerializer.Deserialize<StartingInfo>(JSONString);
            Dictionary<ItemName, RegionName> startingItems = new();
            foreach (var kongIndex in info.starting_kongs)
            {
                var kongItem = JSONKeyMappings.KONGS[kongIndex];
                startingItems.Add(kongItem, RegionName.START);
            }
            foreach(var keyString in info.starting_keys)
            {
                var key = JSONKeyMappings.ITEM_MAP[keyString];
                startingItems.Add(key, RegionName.START);
            }
            ReadStartingItemsIntoUI(startingItems);
            Autotracker.SetStartingItems(startingItems);
            for(int i = 0; i < helmKongs.Count; ++i)
            {
                if (i < info.helm_order.Count)
                {
                    var imageName = JSONKeyMappings.KONGS[info.helm_order[i]].ToString().ToLower();
                    helmKongs[i].Source = new BitmapImage(new Uri("Images/dk64/" + imageName + ".png", UriKind.Relative));
                }
                else helmKongs[i].Source = null;
            }
            for (int i = 0; i < kroolKongs.Count; ++i)
            {
                if (i < info.krool_order.Count)
                {
                    var imageName = JSONKeyMappings.KONGS[info.krool_order[i]].ToString().ToLower();
                    kroolKongs[i].Source = new BitmapImage(new Uri("Images/dk64/" + imageName + ".png", UriKind.Relative));
                }
                else kroolKongs[i].Source = null;
            }
        }

        private void ReadPointSpread(string JSONString)
        {
            var pointPairs = JsonConvert.DeserializeObject <Dictionary<string, int>>(JSONString);
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
        public void ParseRegions(dynamic JSONObject)
        {
            var regionInfo = JSONObject["Spoiler Hints Data"].ToObject<Dictionary<string, string>>();
            SpoilerSettings settings = null;

            foreach (var regionEntry in regionInfo)
            {
                RegionSpoilerInfo info = System.Text.Json.JsonSerializer.Deserialize<RegionSpoilerInfo>(regionEntry.Value);
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
                if (!JSONKeyMappings.REGION_MAP.ContainsKey(info.level_name)) continue;
                if(settings == null)
                {
                    bool pointsEnabled = info.points != -1;
                    bool vialsEnabled = !pointsEnabled;
                    bool WOTHEnabled = info.woth_count != -1;
                    settings = new SpoilerSettings(pointsEnabled, vialsEnabled, WOTHEnabled);
                }
                RegionName regionName = JSONKeyMappings.REGION_MAP[info.level_name];
                Regions[regionName].SetInitialPoints(info.points);
                Regions[regionName].SetRequiredCheckTotal(info.woth_count);
                Regions[regionName].SpoilerSettings = settings;
                var grid = Regions[regionName].RegionGrid;
                //hoo boy i love enums
                info.vial_colors.Sort( (a,b) => JSONKeyMappings.VIAL_MAP[a] - JSONKeyMappings.VIAL_MAP[b]);
                foreach(var vial in info.vial_colors) grid.AddInitialVial(JSONKeyMappings.VIAL_MAP[vial]);
            }
        }

        private void ReadLevelOrder(dynamic JSONObject)
        {
            var levelOrder = JSONObject["Shuffled Level Order"].ToObject <Dictionary<string, string>>();
            foreach(var entry in levelOrder)
            {
                var originalLevel = entry.Key;
                var newLevel = entry.Value;
                var regionName = JSONKeyMappings.SHUFFLED_LEVEL_NAME_TO_REGION[newLevel];
                var levelOrderNumber = JSONKeyMappings.LEVEL_NAME_TO_LEVEL_ORDER[originalLevel];
                Regions[regionName].SetLevelOrderNumber(levelOrderNumber);
            }
        }

        public void ParseSpoiler(string fileName)
        {
            Reset();
            using StreamReader reader = new(fileName);
            string json = reader.ReadToEnd();

            dynamic JSONObject = JsonConvert.DeserializeObject(json);
            ParseRegions(JSONObject);
            foreach (var entry in ImportantCheckList.ITEMS) entry.Value.InitPointValue();
            SpoilerLoaded = true;
            Autotracker.SetSpoilerLoaded(fileName);
            foreach (var entry in Regions) entry.Value.SetSpoilerAsLoaded();
            InitSavedDataFromSpoiler(fileName);
            if (Properties.Settings.Default.HitList)
            {
                int hashSum = 0;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(json));
                    foreach (var b in hashBytes) hashSum += b;
                }
                GenerateHitList(hashSum);
                ReadLevelOrder(JSONObject);
            }
        }
    }
}
