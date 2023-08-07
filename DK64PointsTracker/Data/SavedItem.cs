﻿using System.Windows;

namespace DK64PointsTracker
{
    public class SavedItem
    {
        public ItemName ItemName { get; }
        public RegionName Region { get; }
        public Visibility Starred { get; }
        public bool Autotracked { get; set; }
        public double Opacity { get; }
        public SavedItem(ItemName itemName, RegionName region, Visibility starred, bool autotracked, double opacity)
        {
            ItemName = itemName;
            Region = region;
            Starred = starred;
            Autotracked = autotracked;
            Opacity = opacity;  
        }
    }
}
