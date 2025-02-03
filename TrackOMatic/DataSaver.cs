using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Linq;
using Newtonsoft.Json;
using System.Timers;
using System.Threading;

namespace TrackOMatic
{
    public class DataSaver
    {
        private SavedProgress savedProgress;
        public MainWindow MainWindow { get; }

        public bool WriteToFile { get; set; }

        public DataSaver(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            savedProgress = new SavedProgress();
        }

        public void Reset()
        {
            savedProgress = new SavedProgress();
        }

        private void FindSavedHints()
        {
            savedProgress.SavedHints.Clear();
            foreach(var hintPanel in MainWindow.HintPanels)
            {
                foreach(var hint in hintPanel.GetSavedHints())
                {
                    savedProgress.SavedHints.Add(hint);
                }
            }
        }

        public void Save(string filePath = "autosave.json", bool writeToFile = true)
        {
            if (savedProgress == null) return;
            FindSavedHints();
            savedProgress.SavedGBCounts = MainWindow.BLockerHints.GetGBCounts();
            savedProgress.BLockerImageIndexes = MainWindow.BLockerHints.GetImageIndexes();
            savedProgress.HelmDoorImageIndexes = MainWindow.HelmDoorHints.GetImageIndexes();
            savedProgress.HelmDoorCounts = MainWindow.HelmDoorHints.GetItemCounts();
            var JSONString = JsonConvert.SerializeObject(savedProgress);
            if(writeToFile) File.WriteAllText(filePath, JSONString);
        }

        private Item FindMatchingItem(ItemName toFind)
        {
            foreach(var item in MainWindow.DraggableItems)
            {
                var itemName = (ItemName)item.Tag;
                if (itemName == toFind) return item;
            }
            return null;
        }

        //if the user turns autotracking off we need to remark the items as not autotracked in the saved data
        public void TurnOffAutotrackingField()
        {
            if(savedProgress == null) return;
            foreach (var savedItemEntry in savedProgress.SavedItems)
            {
                savedItemEntry.Value.Autotracked = false;
            }
        }

        private void ReadSavedProgress()
        {
            if (savedProgress == null) return;
            if(savedProgress.spoilerPath != "" && File.Exists(savedProgress.spoilerPath))
            {
                MainWindow.ParseSpoiler(savedProgress.spoilerPath);
            }
            foreach (var savedItemEntry in savedProgress.SavedItems)
            {
                var savedItem = savedItemEntry.Value;
                var region = savedItem.Region;
                bool autoPlace = (savedItem.Autotracked || savedItem.Hinted);
                Item matchingItem = FindMatchingItem(savedItem.ItemName);
                matchingItem.SetStarVisibility(savedItem.Starred);
                matchingItem.ChangeOpacity(savedItem.Opacity);
                if(savedItem.Autotracked) MainWindow.Autotracker.ProcessSavedItem(savedItem.ItemName);
                if (savedItem.Region != RegionName.UNKNOWN && !savedItem.Hinted)
                {
                    MainWindow.Regions[region].RegionGrid.Add_Item(matchingItem, !savedItem.Autotracked, !savedItem.Hinted);
                }
                if (savedItem.Hinted)
                {
                    matchingItem.Darken();
                }
                matchingItem.ChangeOpacity(savedItem.Opacity);
            }
            foreach (var savedHint in savedProgress.SavedHints.ToList())
            {
                var hintPanel = (HintPanel)MainWindow.FindName(savedHint.HintPanelKey);
                hintPanel.AddSavedHint(savedHint);
            }
            MainWindow.BLockerHints.LoadSavedGBCounts(savedProgress.SavedGBCounts);
            MainWindow.BLockerHints.LoadSavedImageIndexes(savedProgress.BLockerImageIndexes);
            MainWindow.HelmDoorHints.LoadSavedHelmDoorCounts(savedProgress.HelmDoorCounts);
            MainWindow.HelmDoorHints.LoadSavedImageIndexes(savedProgress.HelmDoorImageIndexes);
        }

        public void AddSavedItem(SavedItem savedItem)
        {
            if (savedItem == null || savedProgress == null) return;
            var itemName = savedItem.ItemName;
            if (savedProgress.SavedItems.ContainsKey(itemName))
            {
                savedProgress.SavedItems[itemName] = savedItem;
            }
            else savedProgress.SavedItems.Add(itemName, savedItem);
        }

        public void ReadSavedDataFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                var jsonString = File.ReadAllText(filePath);
                SavedProgress savedData = JsonConvert.DeserializeObject<SavedProgress>(jsonString);
                savedProgress = savedData;
                ReadSavedProgress();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void setSpoilerPath(string newSpoilerPath)
        {
            if (savedProgress == null) return;
            savedProgress.spoilerPath = newSpoilerPath;
        }

        /*

        private void CheckForAutosave()
        {
            var filePath = "autosave.json";
            ReadSavedDataFromFile(filePath);
        }

        public void InitSavedDataFromSpoiler(string fileName)
        {
            savedProgress = new SavedProgress(fileName);
            CheckForAutosave();
        }
        */
    }
}
