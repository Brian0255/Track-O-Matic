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
using TrackOMatic.Properties;

namespace TrackOMatic
{
    public class HitListHintManager
    {
        public MainWindow MainWindow { get; }
        private int NextHintBarrier = 5;
        private int CurrentIndex = 0;
        private List<ItemName> ItemsToPullFrom;
        private Dictionary<ItemName, RegionName> StartingItems;
        private Dictionary<ItemName, RegionName> TrainingItems;
        private bool SpoilerLoaded = false;

        public HitListHintManager(MainWindow mainWindow) 
        {
            MainWindow = mainWindow;
        }

        private bool ShouldExclude(ItemName itemName)
        {
            if (StartingItems.ContainsKey(itemName)) return true;
            if (TrainingItems.ContainsKey(itemName)) return true;
            return false;
        }

        public void InitializeFromSpoiler(Dictionary<ItemName, RegionName> startingItems, Dictionary<ItemName, RegionName> trainingItems)
        {
            SpoilerLoaded = true;
            NextHintBarrier = 5;
            CurrentIndex = 0;
            ItemsToPullFrom = new();
            StartingItems = startingItems;
            TrainingItems = trainingItems;
            foreach(var entry in MainWindow.ITEM_NAME_TO_REGION)
            {
                var itemName = entry.Key;
                if (ShouldExclude(itemName)) continue;
                ItemsToPullFrom.Add(itemName);
            }
            ItemsToPullFrom.Shuffle(MainWindow.SpoilerParser.RNGSeed);
        }

        public ItemName OnGBUpdate(int newTotal)
        {
            if (!Settings.Default.HitList) return ItemName.NONE;
            if(!SpoilerLoaded) return ItemName.NONE;
            if (newTotal < NextHintBarrier) return ItemName.NONE;
            CurrentIndex = (NextHintBarrier - 5) / 5;
            NextHintBarrier += 5;
            if (CurrentIndex > ItemsToPullFrom.Count - 1) return ItemName.NONE;
            return ItemsToPullFrom[CurrentIndex];
        }
    }
}
