using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DK64PointsTracker
{
    public partial class MainWindow : Window
    {
        /// 
        /// Options
        ///

        private void SaveProgress(object sender, RoutedEventArgs e)
        {
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "txt files (*.txt)|*.txt";
            saveFileDialog.FileName = "pape-tracker-save";
            saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (saveFileDialog.ShowDialog() == true)
            {
                Save(saveFileDialog.FileName);
            }
            */
        }

        public void Save(string filename)
        {
           /*
            string mode = "Mode: " + data.mode.ToString();
            // save settings
            string settings = "Settings: ";
            //if (PromiseCharmOption.IsChecked)
            //    settings += "Promise Charm - ";
            //if (ReportsOption.IsChecked)
            //    settings += "Secret Ansem Reports - ";
            //if (AbilitiesOption.IsChecked)
            //    settings += "Second Chance & Once More - ";
            //if (TornPagesOption.IsChecked)
            //    settings += "Torn Pages - ";
            //if (CureOption.IsChecked)
            //    settings += "Cure - ";
            //if (FinalFormOption.IsChecked)
            //    settings += "Final Form - ";
            //if (SoraHeartOption.IsChecked)
            //    settings += "Sora's Heart - ";
            //if (SimulatedOption.IsChecked)
            //    settings += "Simulated Twilight Town - ";
            //if (ForeverForestOption.IsChecked)
            //    settings += "100 Acre Wood - ";
            //if (ShiverMountainOption.IsChecked)
            //    settings += "ShiverMountain - ";

            // save hint state (hint info, hints, track attempts)
            string attempts = "";
            string hintValues = "";
            if (data.mode == Mode.Hints || data.mode == Mode.OpenKHHints)
            {
                attempts = "Attempts: ";
                if (data.hintsLoaded)
                {
                    foreach (int num in data.reportAttempts)
                    {
                        attempts += " - " + num.ToString();
                    }
                }
                // store hint values
                hintValues = "HintValues:";
                foreach (RegionData regionData in data.RegionsData.Values.ToList())
                {
                    if (regionData.hint == null)
                        continue;

                    int num = Int32.Parse(regionData.hint.Text);
                    hintValues += " " + num.ToString();
                }
            }

            // Save progress of regions
            //string Progress = "Progress:";
            //Progress += " " + data.RegionsData["ToadTownTunnels"].progress.ToString();
            //Progress += " " + data.RegionsData["KoopaRegion"].progress.ToString();
            //Progress += " " + data.RegionsData["KoopaFortress"].progress.ToString();
            //Progress += " " + data.RegionsData["MtRugged"].progress.ToString();
            //Progress += " " + data.RegionsData["DryDryOutpost"].progress.ToString();
            //Progress += " " + data.RegionsData["DryDryDesert"].progress.ToString();
            //Progress += " " + data.RegionsData["DryDryRuins"].progress.ToString();
            //Progress += " " + data.RegionsData["ForeverForest"].progress.ToString();
            //Progress += " " + data.RegionsData["BooMansion"].progress.ToString();
            //Progress += " " + data.RegionsData["GustyGulch"].progress.ToString();
            //Progress += " " + data.RegionsData["TubbaCastle"].progress.ToString();
            //Progress += " " + data.RegionsData["ShyGuyToybox"].progress.ToString();
            //Progress += " " + data.RegionsData["JadeJungle"].progress.ToString();
            //Progress += " " + data.RegionsData["MtLavalava"].progress.ToString();

            // save items in regions
            string GoombaRegion = "GoombaRegion:";
            foreach (Item item in data.RegionsData["GoombaRegion"].regionGrid.Children)
            {
                GoombaRegion += " " + item.Name;
            }
            string ToadTown = "ToadTown:";
            foreach (Item item in data.RegionsData["ToadTown"].regionGrid.Children)
            {
                ToadTown += " " + item.Name;
            }
            string simulated = "ToadTownTunnels:";
            foreach (Item item in data.RegionsData["ToadTownTunnels"].regionGrid.Children)
            {
                simulated += " " + item.Name;
            }
            string KoopaRegion = "KoopaRegion:";
            foreach (Item item in data.RegionsData["KoopaRegion"].regionGrid.Children)
            {
                KoopaRegion += " " + item.Name;
            }
            string KoopaFortress = "KoopaFortress:";
            foreach (Item item in data.RegionsData["KoopaFortress"].regionGrid.Children)
            {
                KoopaFortress += " " + item.Name;
            }
            string beastCastle = "MtRugged:";
            foreach (Item item in data.RegionsData["MtRugged"].regionGrid.Children)
            {
                beastCastle += " " + item.Name;
            }
            string DryDryOutpost = "DryDryOutpost:";
            foreach (Item item in data.RegionsData["DryDryOutpost"].regionGrid.Children)
            {
                DryDryOutpost += " " + item.Name;
            }
            string DryDryDesert = "DryDryDesert:";
            foreach (Item item in data.RegionsData["DryDryDesert"].regionGrid.Children)
            {
                DryDryDesert += " " + item.Name;
            }
            string DryDryRuins = "DryDryRuins:";
            foreach (Item item in data.RegionsData["DryDryRuins"].regionGrid.Children)
            {
                DryDryRuins += " " + item.Name;
            }
            string ForeverForest = "ForeverForest:";
            foreach (Item item in data.RegionsData["ForeverForest"].regionGrid.Children)
            {
                ForeverForest += " " + item.Name;
            }
            string BooMansion = "BooMansion:";
            foreach (Item item in data.RegionsData["BooMansion"].regionGrid.Children)
            {
                BooMansion += " " + item.Name;
            }
            string GustyGulch = "GustyGulch:";
            foreach (Item item in data.RegionsData["GustyGulch"].regionGrid.Children)
            {
                GustyGulch += " " + item.Name;
            }
            string TubbaCastle = "TubbaCastle:";
            foreach (Item item in data.RegionsData["TubbaCastle"].regionGrid.Children)
            {
                TubbaCastle += " " + item.Name;
            }
            string ShyGuyToybox = "ShyGuyToybox:";
            foreach (Item item in data.RegionsData["ShyGuyToybox"].regionGrid.Children)
            {
                ShyGuyToybox += " " + item.Name;
            }
            string JadeJungle = "JadeJungle:";
            foreach (Item item in data.RegionsData["JadeJungle"].regionGrid.Children)
            {
                JadeJungle += " " + item.Name;
            }
            string MtLavalava = "MtLavalava:";
            foreach (Item item in data.RegionsData["MtLavalava"].regionGrid.Children)
            {
                MtLavalava += " " + item.Name;
            }
            string ShiverMountain = "ShiverMountain:";
            foreach (Item item in data.RegionsData["ShiverMountain"].regionGrid.Children)
            {
                ShiverMountain += " " + item.Name;
            }
            string FlowerFields = "FlowerFields:";
            foreach (Item item in data.RegionsData["FlowerFields"].regionGrid.Children)
            {
                FlowerFields += " " + item.Name;
            }
            string CrystalPalace = "CrystalPalace:";
            foreach (Item item in data.RegionsData["CrystalPalace"].regionGrid.Children)
            {
                FlowerFields += " " + item.Name;
            }
            string StartingGear = "StartingGear:";
            foreach (Item item in data.RegionsData["StartingGear"].regionGrid.Children)
            {
                FlowerFields += " " + item.Name;
            }

            FileStream file = File.Create(filename);
            StreamWriter writer = new StreamWriter(file);

            writer.WriteLine(mode);
            writer.WriteLine(settings);
            if (data.mode == Mode.Hints)
            {
                writer.WriteLine(attempts);
                writer.WriteLine(data.hintFileText[0]);
                writer.WriteLine(data.hintFileText[1]);
                writer.WriteLine(hintValues);
            }
            else if (data.mode == Mode.OpenKHHints)
            {
                writer.WriteLine(attempts);
                writer.WriteLine(data.openKHHintText);
                writer.WriteLine(hintValues);
            }
            else if (data.mode == Mode.AltHints)
            {
                Dictionary<string, List<string>> test = new Dictionary<string, List<string>>();
                foreach (string key in data.RegionsData.Keys.ToList())
                {
                    test.Add(key, data.RegionsData[key].checkCount);
                }
                string hintObject = JsonSerializer.Serialize(test);
                string hintText = Convert.ToBase64String(Encoding.UTF8.GetBytes(hintObject));
                writer.WriteLine(hintText);
            }
            else if (data.mode == Mode.OpenKHAltHints)
            {
                writer.WriteLine(data.openKHHintText);
            }
            //writer.WriteLine(Progress);
            writer.WriteLine(GoombaRegion);
            writer.WriteLine(ToadTown);
            writer.WriteLine(simulated);
            writer.WriteLine(KoopaRegion);
            writer.WriteLine(KoopaFortress);
            writer.WriteLine(beastCastle);
            writer.WriteLine(DryDryOutpost);
            writer.WriteLine(DryDryDesert);
            writer.WriteLine(DryDryRuins);
            writer.WriteLine(ForeverForest);
            writer.WriteLine(BooMansion);
            writer.WriteLine(GustyGulch);
            writer.WriteLine(TubbaCastle);
            writer.WriteLine(ShyGuyToybox);
            writer.WriteLine(JadeJungle);
            writer.WriteLine(MtLavalava);
            writer.WriteLine(ShiverMountain);
            writer.WriteLine(FlowerFields);
            writer.WriteLine(CrystalPalace);
            writer.WriteLine(StartingGear);

            foreach (var region in data.RegionsData)
            {
                writer.Write(region.Key + ": ");
                foreach (var item in region.Value.Checks)
                {
                    if (item.Item is object) writer.Write(item.Value + ", ");
                }
                writer.WriteLine();
            }

            writer.Close();
           */
        }

        private void LoadProgress(object sender, RoutedEventArgs e)
        {
            /*
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.FileName = "pape-tracker-save";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog.ShowDialog() == true)
            {
                Load(openFileDialog.FileName);
            }
            */
        }

        private void Load(string filename)
        {
            /*
            Stream file = File.Open(filename, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            // reset tracker
            OnReset(null, null);

            string mode = reader.ReadLine().Substring(6);
            //if (mode == "Hints")
            //    SetMode(Mode.Hints);
            //else if (mode == "AltHints")
            //    SetMode(Mode.AltHints);
            //else if (mode == "OpenKHHints")
            //    SetMode(Mode.OpenKHHints);
            //else if (mode == "OpenKHAltHints")
            //    SetMode(Mode.OpenKHAltHints);

            //// set settings
            string settings = reader.ReadLine();
            //LoadSettings(settings.Substring(10));

            //// set hint state
            //if (mode == "Hints")
            //{
            //    string attempts = reader.ReadLine();
            //    attempts = attempts.Substring(13);
            //    string[] attemptsArray = attempts.Split('-');
            //    for (int i = 0; i < attemptsArray.Length; ++i)
            //    {
            //        data.reportAttempts[i] = int.Parse(attemptsArray[i]);
            //    }

            //    string line1 = reader.ReadLine();
            //    data.hintFileText[0] = line1;
            //    string[] reportvalues = line1.Split('.');

            //    string line2 = reader.ReadLine();
            //    data.hintFileText[1] = line2;
            //    line2 = line2.TrimEnd('.');
            //    string[] reportorder = line2.Split('.');

            //    for (int i = 0; i < reportorder.Length; ++i)
            //    {
            //        data.reportLocations.Add(data.codes.FindCode(reportorder[i]));
            //        string[] temp = reportvalues[i].Split(',');
            //        data.reportInformation.Add(new Tuple<string, int>(data.codes.FindCode(temp[0]), int.Parse(temp[1]) - 32));
            //    }

            //    data.hintsLoaded = true;
            //    HintText.Content = "Hints Loaded";
            //}
            //else if (mode == "AltHints")
            //{
            //    var hintText = Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadLine()));
            //    var regions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(hintText);

            //    foreach (var region in regions)
            //    {
            //        if (region.Key == "FlowerFields")
            //        {
            //            continue;
            //        }
            //        foreach (var item in region.Value)
            //        {
            //            data.RegionsData[region.Key].checkCount.Add(item);
            //        }

            //    }
            //    foreach (var key in data.RegionsData.Keys.ToList())
            //    {
            //        if (key == "FlowerFields")
            //            continue;

            //        data.RegionsData[key].regionGrid.RegionComplete();
            //        SetReportValue(data.RegionsData[key].hint, 1);
            //    }
            //}
            //else if (mode == "OpenKHHints")
            //{
            //    string attempts = reader.ReadLine();
            //    attempts = attempts.Substring(13);
            //    string[] attemptsArray = attempts.Split('-');
            //    for (int i = 0; i < attemptsArray.Length; ++i)
            //    {
            //        data.reportAttempts[i] = int.Parse(attemptsArray[i]);
            //    }
            //    data.openKHHintText = reader.ReadLine();
            //    var hintText = Encoding.UTF8.GetString(Convert.FromBase64String(data.openKHHintText));
            //    var hintObject = JsonSerializer.Deserialize<Dictionary<string, object>>(hintText);
            //    var reports = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(hintObject["Reports"].ToString());

            //    List<int> reportKeys = reports.Keys.Select(int.Parse).ToList();
            //    reportKeys.Sort();

            //    foreach (var report in reportKeys)
            //    {
            //        var region = convertOpenKH[reports[report.ToString()]["Region"].ToString()];
            //        var count = reports[report.ToString()]["Count"].ToString();
            //        var location = convertOpenKH[reports[report.ToString()]["Location"].ToString()];
            //        data.reportInformation.Add(new Tuple<string, int>(region, int.Parse(count)));
            //        data.reportLocations.Add(location);
            //    }

            //    data.hintsLoaded = true;
            //    HintText.Content = "Hints Loaded";
            //}
            //else if (mode == "OpenKHAltHints")
            //{
            //    data.openKHHintText = reader.ReadLine();
            //    var hintText = Encoding.UTF8.GetString(Convert.FromBase64String(data.openKHHintText));
            //    var hintObject = JsonSerializer.Deserialize<Dictionary<string, object>>(hintText);
            //    var regions = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(hintObject["region"].ToString());

            //    foreach (var region in regions)
            //    {
            //        if (region.Key == "Critical Bonuses" || region.Key == "Garden of Assemblage")
            //        {
            //            continue;
            //        }
            //        foreach (var item in region.Value)
            //        {
            //            data.RegionsData[convertOpenKH[region.Key]].checkCount.Add(convertOpenKH[item]);
            //        }

            //    }
            //    foreach (var key in data.RegionsData.Keys.ToList())
            //    {
            //        if (key == "FlowerFields")
            //            continue;

            //        data.RegionsData[key].regionGrid.RegionComplete();
            //        SetReportValue(data.RegionsData[key].hint, 1);
            //    }
            //}


            // set hint values (DUMB)
            if (data.hintsLoaded)
            {
                string[] hintValues = reader.ReadLine().Substring(12).Split(' ');
                SetReportValue(data.RegionsData["GoombaRegion"].hint, int.Parse(hintValues[0]));
                SetReportValue(data.RegionsData["ToadTown"].hint, int.Parse(hintValues[1]));
                SetReportValue(data.RegionsData["ToadTownTunnels"].hint, int.Parse(hintValues[2]));
                SetReportValue(data.RegionsData["KoopaRegion"].hint, int.Parse(hintValues[3]));
                SetReportValue(data.RegionsData["KoopaFortress"].hint, int.Parse(hintValues[4]));
                SetReportValue(data.RegionsData["MtRugged"].hint, int.Parse(hintValues[5]));
                SetReportValue(data.RegionsData["DryDryOutpost"].hint, int.Parse(hintValues[6]));
                SetReportValue(data.RegionsData["DryDryDesert"].hint, int.Parse(hintValues[7]));
                SetReportValue(data.RegionsData["DryDryRuins"].hint, int.Parse(hintValues[8]));
                SetReportValue(data.RegionsData["ForeverForest"].hint, int.Parse(hintValues[9]));
                SetReportValue(data.RegionsData["BooMansion"].hint, int.Parse(hintValues[10]));
                SetReportValue(data.RegionsData["GustyGulch"].hint, int.Parse(hintValues[11]));
                SetReportValue(data.RegionsData["TubbaCastle"].hint, int.Parse(hintValues[12]));
                SetReportValue(data.RegionsData["ShyGuyToybox"].hint, int.Parse(hintValues[13]));
                SetReportValue(data.RegionsData["JadeJungle"].hint, int.Parse(hintValues[14]));
                SetReportValue(data.RegionsData["MtLavalava"].hint, int.Parse(hintValues[15]));
                SetReportValue(data.RegionsData["ShiverMountain"].hint, int.Parse(hintValues[16]));
                SetReportValue(data.RegionsData["FlowerFields"].hint, int.Parse(hintValues[17]));
                SetReportValue(data.RegionsData["CrystalPalace"].hint, int.Parse(hintValues[18]));
                SetReportValue(data.RegionsData["StartingGear"].hint, int.Parse(hintValues[19]));
            }

            //string[] progress = reader.ReadLine().Substring(10).Split(' ');
            //data.RegionsData["ToadTownTunnels"].progress = int.Parse(progress[0]);
            //data.RegionsData["KoopaRegion"].progress = int.Parse(progress[1]);
            //data.RegionsData["KoopaFortress"].progress = int.Parse(progress[2]);
            //data.RegionsData["MtRugged"].progress = int.Parse(progress[3]);
            //data.RegionsData["DryDryOutpost"].progress = int.Parse(progress[4]);
            //data.RegionsData["DryDryDesert"].progress = int.Parse(progress[5]);
            //data.RegionsData["DryDryRuins"].progress = int.Parse(progress[6]);
            //data.RegionsData["ForeverForest"].progress = int.Parse(progress[7]);
            //data.RegionsData["BooMansion"].progress = int.Parse(progress[8]);
            //data.RegionsData["GustyGulch"].progress = int.Parse(progress[9]);
            //data.RegionsData["TubbaCastle"].progress = int.Parse(progress[10]);
            //data.RegionsData["ShyGuyToybox"].progress = int.Parse(progress[11]);
            //data.RegionsData["JadeJungle"].progress = int.Parse(progress[12]);
            //data.RegionsData["MtLavalava"].progress = int.Parse(progress[13]);

            //SetProgressIcons();

            // add items to regions
            while (reader.EndOfStream == false)
            {
                string region = reader.ReadLine();
                string regionName = region.Substring(0, region.IndexOf(':'));
                string items = region.Substring(region.IndexOf(':') + 1).Trim();
                if (items != string.Empty)
                {
                    foreach (string item in items.Split(' '))
                    {
                        RegionGrid grid = FindName(regionName + "Grid") as RegionGrid;
                        Item importantCheck = FindName(item) as Item;

                        if (grid.Handle_Report(importantCheck, this, data))
                            grid.Add_Item(importantCheck, this);
                    }
                }
            }

            reader.Close();
            */
        }

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

        private string TrimShopsPricing(string itemName)
        {
            var pattern = @"(.*) \(.*\)";
            Match match = Regex.Match(itemName, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                itemName = match.Groups[1].Value;
            }
            return itemName;
        }

        private ImportantCheck GetImportantCheckFromKey(string JSONKey)
        {
            if (!JSONKeyMappings.ITEM_MAP.ContainsKey(JSONKey))
            {
                return null;
            }
            var matchingEnum = JSONKeyMappings.ITEM_MAP[JSONKey];
            if (ImportantCheckList.ITEMS.ContainsKey(matchingEnum))
            {
                return (ImportantCheckList.ITEMS[matchingEnum]);
            }
            return null;
        }

        private RegionName GetRegionThatShuffledInto(Dictionary<string,string> levelOrder, RegionName regionToLookFor)
        {
            if (!JSONKeyMappings.REGION_TO_SHUFFLED_LEVEL.ContainsKey(regionToLookFor))
            {
                return regionToLookFor;
            }
            var stringToLookFor = JSONKeyMappings.REGION_TO_SHUFFLED_LEVEL[regionToLookFor];
            foreach(var kvp in levelOrder)
            {
                if (kvp.Value == stringToLookFor)
                {
                    return JSONKeyMappings.SHUFFLED_LEVELS_TO_REGION[kvp.Key];
                }
            }
            return RegionName.UNKNOWN;
        }

        private void ReadShuffledLevelOrder(dynamic JSONObject)
        {
            ShuffledLevelOrder = new();
            Dictionary<string, string> levelOrder = new();
            if (JSONObject["Shuffled Level Order"] !=null)
            {
                levelOrder = JSONObject["Shuffled Level Order"].ToObject<IDictionary<string, string>>();
            }
            foreach(var entry in Regions)
            {
                var regionName = entry.Key;
                if(levelOrder.Count == 0)
                {
                    ShuffledLevelOrder[regionName] = regionName;
                    continue;
                }
                if (JSONKeyMappings.REGION_TO_SHUFFLED_LEVEL.ContainsKey(regionName))
                {
                    var shuffledName = JSONKeyMappings.REGION_TO_SHUFFLED_LEVEL[regionName];
                    var shuffledRegion = GetRegionThatShuffledInto(levelOrder, regionName);
                    ShuffledLevelOrder[regionName] = shuffledRegion;
                    Regions[shuffledRegion].SetShuffledRegion(regionName);
                }
            }
        }

        private RegionName ConvertToRegionWithPrefix(string regionName)
        {
            var prefix = regionName.Split(' ')[0];
            if(JSONKeyMappings.REGION_PREFIX_TO_REGION.ContainsKey(prefix)) return JSONKeyMappings.REGION_PREFIX_TO_REGION[prefix];
            return RegionName.UNKNOWN;
        }

        private RegionName CheckSpecialCase(string itemArea)
        { 
            if (JSONKeyMappings.SPECIAL_EXCEPTIONS_ITEM_MAP.ContainsKey(itemArea))
            {
                return JSONKeyMappings.SPECIAL_EXCEPTIONS_ITEM_MAP[itemArea];
            }
            return ConvertToRegionWithPrefix(itemArea);
        }

        private RegionName GetRegionNameFromString(string regionName)
        {
            RegionName enumName;
            if (JSONKeyMappings.REGION_MAP.ContainsKey(regionName))
            {
                enumName = JSONKeyMappings.REGION_MAP[regionName];
            }
            else
            {
                enumName = CheckSpecialCase(regionName);
            }
            return enumName;
        }

        private void ParseItems(dynamic JSONObject)
        {
            ReadShuffledLevelOrder(JSONObject);
            int slamCount = 0;
            int pearlCount = 0;
            var items = JSONObject.Items;
            foreach (var regionData in items)
            {
                string regionName = regionData.Name;
                var regionItems = regionData.Value;
                foreach (var itemEntry in regionItems)
                {
                    string itemArea = itemEntry.Name;
                    RegionName enumName = GetRegionNameFromString(itemArea);
                    if (enumName == RegionName.UNKNOWN) enumName = GetRegionNameFromString(regionName);
                    var regionToUse = (ShuffledLevelOrder.ContainsKey(enumName)) ? ShuffledLevelOrder[enumName] : enumName;
                    string itemName = itemEntry.Value;
                    itemName = TrimShopsPricing(itemName);
                    if (itemName == "Progressive Slam")
                    {
                        slamCount++;
                        itemName = itemName + " " + slamCount.ToString();
                    }
                    if(itemName == "Pearl")
                    {
                        pearlCount++;
                        itemName = itemName + " " + pearlCount.ToString();
                    }
                    var importantCheck = GetImportantCheckFromKey(itemName);
                    if (null == importantCheck) continue;
                    Regions[regionToUse].IncreaseInitialPointTotal(importantCheck.PointValue);
                    ItemNameToRegion[importantCheck.ItemName] = regionToUse;
                }
            }
        }

        private void ReadStartingMoves(dynamic JSONObject, Dictionary<ItemName, RegionName> startingItems)
        {
            Dictionary<string,string> islesItems = JSONObject["Items"]["DK Isles"].ToObject<Dictionary<string, string>>();
            int startingMovesCount = JSONObject["Settings"]["Starting Moves Count"];

            IEnumerable<KeyValuePair<string,string>> startingMoveEntries = islesItems.Take(startingMovesCount);
            int slamCount = 0;
            foreach(var entry in startingMoveEntries)
            {
                var itemName = entry.Value;
                if (itemName.Equals("Progressive Slam"))
                {
                    slamCount++;
                    itemName = itemName + " " + slamCount.ToString();
                }
                if (JSONKeyMappings.ITEM_MAP.ContainsKey(itemName))
                {
                    var enumName = JSONKeyMappings.ITEM_MAP[itemName];
                    //need to subtract the values that were initially read to DK Isles
                    Regions[RegionName.DK_ISLES].DecreaseInitialPointTotal(ImportantCheckList.ITEMS[enumName].PointValue);
                    startingItems[enumName] = RegionName.START;
                }
            }
        }
        private void FigureOutStartingKeys(dynamic JSONObject, Dictionary<ItemName,RegionName> startingItems)
        {
            //the spoiler log only tells us keys that we have, so this is kind of messy
            var Paths = JSONObject["Paths"].ToObject<IDictionary<string,object>>();
            var keyList = new List<string>
            {
                "Key 1",
                "Key 2",
                "Key 3",
                "Key 4",
                "Key 5",
                "Key 6",
                "Key 7",
            };
            foreach(var key in keyList)
            {
                if (!Paths.ContainsKey(key)) {
                    startingItems[JSONKeyMappings.ITEM_MAP[key]] = RegionName.START;
                    Regions[RegionName.START].IncreaseRequiredCheckTotal();
                }
            }
            //special check for helm key
            if (JSONObject["Settings"]["Key 8 in Helm"] == true)
            {
                startingItems[ItemName.KEY_8] = RegionName.HIDEOUT_HELM;
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
                    if (ImportantCheckList.ITEMS[itemName].ItemType == ItemType.KEY)
                    {
                        foreach (var entry in Regions)
                        {
                            entry.Value.ProcessKey(itemName, true);
                        }
                    }
                }
            }
        }
        private void ReadEndGame(dynamic JSONObject)
        {
            var HelmList = JSONObject["End Game"]["Helm Rooms"].ToObject<List<string>>();
            var KRoolList = JSONObject["End Game"]["K Rool Phases"].ToObject<List<string>>();
            /*
            EndgameHints = new();
            foreach(var kong in HelmList)
            {
                EndgameHints.Add((kong + " is part of helm").ToUpper());
            }
            foreach (var kong in KRoolList)
            {
                EndgameHints.Add((kong + " is part of k. rool").ToUpper());
            }
            */

            HelmKong1.Source = new BitmapImage(new Uri(@"/Images/dk64/" + HelmList[0]+".png", UriKind.Relative));
            HelmKong2.Source = new BitmapImage(new Uri(@"/Images/dk64/" + HelmList[1] + ".png", UriKind.Relative));
            HelmKong3.Source = new BitmapImage(new Uri(@"/Images/dk64/" + HelmList[2] + ".png", UriKind.Relative));

            /*
            KRoolKong1.Source = new BitmapImage(new Uri(@"/Images/dk64/" + KRoolList[0] + ".png", UriKind.Relative));
            KRoolKong2.Source = new BitmapImage(new Uri(@"/Images/dk64/" + KRoolList[1] + ".png", UriKind.Relative));
            KRoolKong3.Source = new BitmapImage(new Uri(@"/Images/dk64/" + KRoolList[2] + ".png", UriKind.Relative));*/
        }

        private Dictionary<ItemName, RegionName> FindStartingItems(dynamic JSONObject)
        {
            var startingItems = new Dictionary<ItemName, RegionName>();
            FigureOutStartingKeys(JSONObject, startingItems);
            ReadStartingMoves(JSONObject, startingItems);
            var startingKongList = JSONObject["Kongs"]["Starting Kong List"].ToObject<IList<string>>();
            foreach (var kong in startingKongList)
            {
                var matchingEnum = JSONKeyMappings.ITEM_MAP[kong];
                startingItems.Add(matchingEnum, RegionName.START);
                Regions[RegionName.START].IncreaseRequiredCheckTotal();
            }
            return startingItems;
        }

        public void ReadBLockerAmounts(dynamic JSONObject)
        {
            var BLockerGBs = JSONObject["Requirements"]["B Locker GBs"].ToObject<Dictionary<string, int>>();
            Dictionary<string,string> levelOrder = null;
            if (JSONObject["Shuffled Level Order"] != null)
            {
                levelOrder = JSONObject["Shuffled Level Order"].ToObject<Dictionary<string, string>>();
            }
            foreach (var entry in BLockerGBs)
            {
                var levelName = entry.Key;
                var GBs = entry.Value;
                var regionToUse = JSONKeyMappings.REGION_MAP[levelName];
                if(levelOrder != null)
                {
                    regionToUse = GetRegionThatShuffledInto(levelOrder, regionToUse);
                }
                Regions[regionToUse].SetBLockerAmount(GBs);
            }
        }

        private void ReadWayOfTheHoard(dynamic JSONObject, Dictionary<ItemName, RegionName> startingItems)
        {
            /*
            var WOTH = JSONObject["Way of the Hoard"].ToObject<Dictionary<string, string>>();
            foreach(var entry in WOTH)
            {
                var itemName = entry.Value;
                if (!JSONKeyMappings.ITEM_MAP.ContainsKey(itemName)) continue;
                ItemName item = JSONKeyMappings.ITEM_MAP[itemName];
                RegionName region = ItemNameToRegion[item];
                Regions[region].IncreaseRequiredCheckTotal();
            }
            CheckWOTHSlam(JSONObject, startingItems);*/
            var playthroughInfo = JSONObject["Playthrough"].ToObject<Dictionary<string, Dictionary<string, string>>>();
            var slamCount = 0;
            foreach (var sphereEntry in playthroughInfo)
            {
                foreach (var itemEntry in sphereEntry.Value)
                {
                    var itemArea = itemEntry.Key;
                    var itemName = itemEntry.Value;
                    if (itemName == "Progressive Slam")
                    {
                        slamCount++;
                        itemName = itemName + " " + slamCount.ToString();
                    }
                    if (!JSONKeyMappings.ITEM_MAP.ContainsKey(itemName)) continue;
                    var item = JSONKeyMappings.ITEM_MAP[itemName];
                    var region = GetRegionNameFromString(itemArea);
                    var shuffledRegion = (ShuffledLevelOrder.ContainsKey(region)) ? ShuffledLevelOrder[region] : region;
                    if (startingItems.ContainsKey(item)) shuffledRegion = RegionName.START;
                    if (itemName == "Key 8") shuffledRegion = RegionName.HIDEOUT_HELM;
                    Regions[shuffledRegion].IncreaseRequiredCheckTotal();
                }
            }
        }



        public void ParseSpoiler(string fileName)
        {
            Reset();
            using StreamReader reader = new(fileName);
            string json = reader.ReadToEnd();
            dynamic JSONObject = JsonConvert.DeserializeObject(json);
            ParseItems(JSONObject);
            Dictionary<ItemName, RegionName> startingItems = FindStartingItems(JSONObject);
            foreach(var entry in startingItems)
            {
                var item = entry.Key;
                if(item != ItemName.KEY_8) ItemNameToRegion[item] = RegionName.START;
            }
            ReadStartingItemsIntoUI(startingItems);
            ReadEndGame(JSONObject);
            ReadBLockerAmounts(JSONObject);
            ReadWayOfTheHoard(JSONObject, startingItems);
            SpoilerLoaded = true;
            Autotracker.SetSpoilerLoaded();
            Autotracker.SetStartingItems(startingItems);
            foreach (var entry in Regions) entry.Value.SetSpoilerAsLoaded();
        }

        private void Reset()
        {
            TotalGBs = 0;
            //GBs.SetGBTotal(TotalGBs);
            //ModeDisplay.Header = "";
            SpoilerLoaded = false;
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                region.Reset();
            }
            foreach (var item in DraggableItems.Cast<Item>())
            {
                item.CanLeftClick = true;
            }
            foreach (var key in Collectibles.Keys.ToList()) Collectibles[key].SetAmount(0);
            HelmKong1.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            HelmKong2.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            HelmKong3.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            DonkeyBPs.Text = 0;
            DiddyBPs.Text = 0;
            LankyBPs.Text = 0;
            TinyBPs.Text = 0;
            ChunkyBPs.Text = 0;
            Notes.Text = "";
            Notes.Reset();
            Autotracker.Reset();

        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }
    }
}
