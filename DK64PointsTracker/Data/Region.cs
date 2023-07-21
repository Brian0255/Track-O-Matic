using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Windows;

namespace DK64PointsTracker
{
    public class Region
    {
        public bool complete;
        public RegionName RegionName;
        public Dictionary<ImportantCheck, bool> CurrentChecks { get; private set; }
        public ItemName KeyLock;
        private bool Locked;
        private bool isLobby1 = false;
        private int requiredChecks = 0;
        public Grid MainUIGrid { get; }
        public Button RegionButton { get; }
        public RegionGrid RegionGrid { get; }
        public TextBlock PointsLabel { get; }
        public TextBlock TopLabel { get; }
        public int TotalPoints { get; private set; }
        public bool SpoilerLoaded { get; private set; }
        public int CurrentPoints { get; private set; }
        public int BLockerAmount { get; private set; }

        public Region(RegionName regionName, Grid mainUIGrid, Button regionButton, RegionGrid checksContainer, TextBlock pointsLabel, TextBlock topLabel = null, ItemName keyLock = ItemName.NONE)
        {
            RegionName = regionName;
            BLockerAmount = 0;
            TotalPoints = 0;
            MainUIGrid = mainUIGrid;
            RegionButton = regionButton;
            RegionGrid = checksContainer;
            PointsLabel = pointsLabel;
            TopLabel = topLabel;
            CurrentChecks = new();
            RegionGrid.Region = this;
            KeyLock = keyLock;
            Locked = (keyLock != ItemName.NONE);
            UpdateUIFromShuffledRegion();
            TopLabel.SetResourceReference(TextBlock.ForegroundProperty, "RequiredChecksColor");
        }

        //Should only be called when the spoiler log is being read, to initialize the point total
        public void AddCheck(ImportantCheck check)
        {
            CurrentChecks[check] = true;
            UpdatePoints();
        }

        public void SetAsLobby1() 
        { 
            isLobby1 = true;
        }

        public void UpdateRequiredChecksTotal()
        {
            if (!SpoilerLoaded) return;
            int total = 0;
            foreach (var control in RegionGrid.Children)
            {
                if(control is Item item)
                {
                    total += (item.Checkmark.Visibility == Visibility.Visible) ? 1 : 0;
                }
            }
            var toDisplay = Math.Max(requiredChecks - total, 0);
            TopLabel.Text = toDisplay.ToString();
        }

        public void Reset()
        {
            TotalPoints = 0;
            requiredChecks = 0;
            BLockerAmount = 0;
            CurrentChecks = new();
            SpoilerLoaded = false;
            Locked = (KeyLock != ItemName.NONE || isLobby1);
            UpdatePoints();
            UpdateUIFromShuffledRegion();
            if (TopLabel != null) TopLabel.Text = "?";

            //put them into a list first to avoid the "cant remove while enumerating" 
            var elements = new List<Item>(RegionGrid.Children.Count);

            foreach(var control in RegionGrid.Children)
            {
                elements.Add(control as Item);
            }
            foreach(var item in elements)
            {
                item.HandleItemReturn();
            }
        }

        public void RemoveCheck(ImportantCheck check)
        {
            CurrentChecks.Remove(check);
            UpdatePoints();
        }

        public void UpdatePoints()
        {
            CurrentPoints = 0;
            foreach(var entry in CurrentChecks)
            {
                var check = entry.Key;
                CurrentPoints += check.PointValue;
            }
            var remaining = Math.Max(0,TotalPoints - CurrentPoints);
            PointsLabel.Text = (SpoilerLoaded) ? remaining.ToString() : "?";
            var resource = (CurrentPoints >= TotalPoints && SpoilerLoaded) ? "RegionComplete" : "RegionInProgress";
            PointsLabel.SetResourceReference(TextBlock.ForegroundProperty, resource);
        }

        public void SetShuffledRegion(RegionName newRegionName)
        {
            RegionName = newRegionName;
        }

        public void IncreaseInitialPointTotal(int toAdd)
        {
            TotalPoints += toAdd;
        }

        public void DecreaseInitialPointTotal(int toAdd)
        {
            TotalPoints -= toAdd;
        }

        public void IncreaseRequiredCheckTotal()
        {
            requiredChecks++;
        }

        public void DecreaseRequiredCheckTotal()
        {
            requiredChecks--;
        }

        public void SetSpoilerAsLoaded()
        {
            SpoilerLoaded = true;
            if (isLobby1) Locked = false;
            UpdatePoints();
            UpdateUIFromShuffledRegion();
            if (TopLabel != null) TopLabel.Text = requiredChecks.ToString();
        }

        private void UpdateUIFromShuffledRegion()
        {
            var resourceNameToUse = (Locked) ? "locked_region": RegionName.ToString().ToLower();
            RegionButton.ClearValue(Button.ContentProperty);
            RegionButton.SetResourceReference(Button.ContentProperty, resourceNameToUse);
        }

        public void ProcessKey(ItemName key, bool addingKey)
        {
            if(KeyLock == key)
            {
                Locked = !addingKey;
            }
            if (!SpoilerLoaded) return;
            UpdateUIFromShuffledRegion();
            UpdatePoints();
        }

        public void SetBLockerAmount(int newAmount)
        {
            BLockerAmount = newAmount;
        }
    }
}
