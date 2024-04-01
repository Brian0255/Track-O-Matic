using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Windows;

namespace TrackOMatic
{
    public class Region
    {
        public static readonly List<RegionName> LOBBY_ORDER = new()
        {
            RegionName.JUNGLE_JAPES, 
            RegionName.ANGRY_AZTEC, 
            RegionName.FRANTIC_FACTORY, 
            RegionName.GLOOMY_GALLEON, 
            RegionName.FUNGI_FOREST, 
            RegionName.CRYSTAL_CAVES, 
            RegionName.CREEPY_CASTLE
        };

        public bool complete;
        public SpoilerSettings SpoilerSettings { get; set; }
        public RegionName RegionName;
        public Dictionary<ImportantCheck, bool> CurrentChecks { get; private set; }

        private Dictionary<string, TextBlock> SpoilerSettingToLabel = new()
        {
            {"RequiredChecks", null },
            {"PointsLabel", null },
        };
        private int requiredChecks = 0;
        public Grid MainUIGrid { get; }
        public Image RegionButton { get; }
        public RegionGrid RegionGrid { get; }
        public TextBlock BottomLabel { get; }
        public TextBlock TopLabel { get; }
        public LevelOrderNumber LevelOrderNumber { get; }
        public int TotalPoints { get; private set; }
        public bool SpoilerLoaded { get; private set; }
        public int CurrentPoints { get; private set; }
        public int BLockerAmount { get; private set; }

        private void ResetLabels()
        {
            if (BottomLabel != null)
            {
                BottomLabel.SetResourceReference(TextBlock.ForegroundProperty, "RequiredChecksColor");
                BottomLabel.Text = "?";
                BottomLabel.Visibility = Visibility.Visible;
            }
            if (TopLabel != null)
            {
                TopLabel.Foreground = Brushes.White;
                TopLabel.Text = "?";
                TopLabel.Visibility = Visibility.Visible;
            }
        }

        public Region(RegionName regionName, Grid mainUIGrid, Image regionButton, RegionGrid checksContainer, TextBlock bottomLabel = null, TextBlock topLabel = null, LevelOrderNumber levelOrderNumber = null)
        {
            RegionName = regionName;
            BLockerAmount = 0;
            TotalPoints = 0;
            MainUIGrid = mainUIGrid;
            RegionButton = regionButton;
            RegionGrid = checksContainer;
            BottomLabel = bottomLabel;
            TopLabel = topLabel;
            LevelOrderNumber = levelOrderNumber;
            CurrentChecks = new();
            RegionGrid.Region = this;
            ResetLabels();
        }

        //Should only be called when the spoiler log is being read, to initialize the point total
        public void AddCheck(ImportantCheck check)
        {
            if (check == null) return;
            CurrentChecks[check] = true;
            UpdatePoints();
        }

        public void UpdateRequiredChecksTotal()
        {
            if (!SpoilerLoaded) return;
            int total = 0;
            foreach (var control in RegionGrid.Children)
            {
                if(control is Item item)
                {
                    total += (item.Star.Visibility == Visibility.Visible) ? 1 : 0;
                }
            }
            var toDisplay = Math.Max(requiredChecks - total, 0);
            var label = SpoilerSettingToLabel["RequiredChecks"];
            if(label != null) label.Text = toDisplay.ToString();
        }

        public void Reset()
        {
            TotalPoints = 0;
            requiredChecks = 0;
            CurrentChecks = new();
            SpoilerLoaded = false;
            //put them into a list first to avoid the "cant remove while enumerating" 
            var elements = new List<Item>(RegionGrid.Children.Count);

            foreach(var control in RegionGrid.Children)
            {
                elements.Add(control as Item);
            }
            foreach(var item in elements)
            {
                if (item.Tag == null) continue;
                item.HandleItemReturn();
            }
            RegionGrid.ResetVials();
            ResetLabels();
            if(LevelOrderNumber != null) LevelOrderNumber.Reset();
            SetAsEmptySpoiler();
        }

        public void RemoveCheck(ImportantCheck check)
        {
            if (check == null) return;
            CurrentChecks.Remove(check);
            UpdatePoints();
        }

        public void UpdatePoints()
        {
            if (SpoilerSettings == null || !SpoilerSettings.PointsEnabled) return;
            CurrentPoints = 0;
            foreach(var entry in CurrentChecks)
            {
                var check = entry.Key;
                CurrentPoints += check.PointValue;
            }
            var remaining = Math.Max(0,TotalPoints - CurrentPoints);
            var pointsLabel = SpoilerSettingToLabel["PointsLabel"];
            if(pointsLabel == null) return;
            pointsLabel.Text = (SpoilerLoaded) ? remaining.ToString() : "?";
            var resource = (CurrentPoints >= TotalPoints && SpoilerLoaded) ? "RegionComplete" : "RegionInProgress";
            pointsLabel.SetResourceReference(TextBlock.ForegroundProperty, resource);
        }

        public void SetShuffledRegion(RegionName newRegionName)
        {
            RegionName = newRegionName;
        }

        public void AddPoints(int newValue)
        {
            TotalPoints += newValue;
        }

        public void AddRequiredCheckTotal(int newValue)
        {
            requiredChecks += newValue;
        }

        private void ConfigureLabelsFromSettings()
        {
            if (SpoilerSettings == null) return;
            if (SpoilerSettings.PointsEnabled)
            {
                SpoilerSettingToLabel["PointsLabel"] = BottomLabel;
                TopLabel.SetResourceReference(TextBlock.ForegroundProperty, "RequiredChecksColor");
                BottomLabel.Foreground = Brushes.White;
                SpoilerSettingToLabel["RequiredChecks"] = TopLabel;
            }
            else if (SpoilerSettings.VialsEnabled)
            {
                TopLabel.Visibility = Visibility.Hidden;
                BottomLabel.SetResourceReference(TextBlock.ForegroundProperty, "RequiredChecksColor");
                SpoilerSettingToLabel["RequiredChecks"] = BottomLabel;
            }
            if (SpoilerSettingToLabel["PointsLabel"] != null)
            {
                SpoilerSettingToLabel["PointsLabel"].Visibility = (SpoilerSettings.PointsEnabled) ? Visibility.Visible : Visibility.Hidden;
            }
            if (SpoilerSettingToLabel["RequiredChecks"] != null) 
            {
                SpoilerSettingToLabel["RequiredChecks"].Visibility = (SpoilerSettings.WOTHEnabled) ? Visibility.Visible : Visibility.Hidden;
            }
            if (RegionName == RegionName.START) return;
            int columnSpan = (SpoilerSettings.PointsEnabled || SpoilerSettings.WOTHEnabled) ? 2 : 4;
            Grid.SetColumnSpan(RegionButton, columnSpan);
        }

        public void SetSpoilerAsLoaded()
        {
            SpoilerLoaded = true;
            Grid.SetColumnSpan(RegionButton, 4);
            ConfigureLabelsFromSettings();
            UpdatePoints();
            UpdateRequiredChecksTotal();
            if (RegionName == RegionName.START) return;
        }
        
        public void SetLevelOrderNumber(int number)
        {
            if (LevelOrderNumber == null) return;
            LevelOrderNumber.SetNumber(number);
        }

        public void SetAsEmptySpoiler()
        {
            if (RegionName == RegionName.START) return;
            if(BottomLabel != null) BottomLabel.Visibility = Visibility.Collapsed;
            if(TopLabel != null) TopLabel.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(RegionButton, 3);
        }
    }
}
