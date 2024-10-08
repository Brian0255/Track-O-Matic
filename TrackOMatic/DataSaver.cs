﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Linq;
using Newtonsoft.Json;
using System.Timers;

namespace TrackOMatic
{
    public class DataSaver
    {
        private SavedProgress savedProgress;
        public MainWindow MainWindow { get; }

        public DataSaver(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public void LoadProgress(object Sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "json files (*.json)|*.json"
            };

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

                ReadSavedDataFromFile(selectedFilePath);
            }
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

        public void Save()
        {
            if (savedProgress == null) return;
            FindSavedHints();
            savedProgress.SavedGBCounts = MainWindow.BLockerHints.GetGBCounts();
            savedProgress.BLockerImageIndexes = MainWindow.BLockerHints.GetImageIndexes();
            savedProgress.HelmDoorImageIndexes = MainWindow.HelmDoorHints.GetImageIndexes();
            savedProgress.HelmDoorCounts = MainWindow.HelmDoorHints.GetItemCounts();
            var JSONString = JsonConvert.SerializeObject(savedProgress);
            var tempPath = "autosave_temp.json";
            var filePath = "autosave.json";
            File.WriteAllText(tempPath, JSONString);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.Move(tempPath, filePath);
            File.Delete(tempPath);
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

        private void ReadSavedDataFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                var jsonString = File.ReadAllText(filePath);
                SavedProgress savedData = JsonConvert.DeserializeObject<SavedProgress>(jsonString);
                if (savedData.SpoilerLogName == savedProgress.SpoilerLogName) savedProgress = savedData;
                ReadSavedProgress();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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
    }
}
