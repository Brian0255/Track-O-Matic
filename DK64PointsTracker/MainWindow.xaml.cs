using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Documents;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Animation;

namespace ClassLibrary
{
    public record Class(string Str)
    {
        internal int Int { get; init; }
    }
}
namespace System.Runtime.CompilerServices
{
    using System.ComponentModel;
    /// <summary>
    /// Reserved to be used by the compiler for tracking metadata.
    /// This class should not be used by developers in source code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

namespace DK64PointsTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int TotalGBs { get; private set; }
        public Button SelectedButton { get; }
        public Dictionary<RegionName, Region> Regions { get; private set;  }
        public Dictionary<ItemType,CollectibleItem> Collectibles { get; private set; }
        public List<string> EndgameHints { get; private set; }
        public Dictionary<ItemName, RegionName> ItemNameToRegion { get; } = new();
        private List<Item> DraggableItems = new();
        public int collected;
        public Autotracker Autotracker { get; private set; }
        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            InitOptions();
            InitData();
        }

        private void InitData()
        {
            Regions = new()
            {
                { RegionName.DK_ISLES, new Region(RegionName.DK_ISLES, DKIslesRegion, DKIslesPicture, DKIslesRegionGrid, DKIslesPoints, DKIslesTopLabel) },
                { RegionName.START, new Region(RegionName.START, StartRegion, StartPicture, StartRegionGrid) },

                { RegionName.JUNGLE_JAPES, new Region(RegionName.LOCKED_REGION, Level1, Level1Picture, Level1RegionGrid, Level1Points, Level1TopLabel) },
                { RegionName.ANGRY_AZTEC, new Region(RegionName.LOCKED_REGION, Level2, Level2Picture, Level2RegionGrid, Level2Points,Level2TopLabel, ItemName.KEY_1) },
                { RegionName.FRANTIC_FACTORY, new Region(RegionName.LOCKED_REGION, Level3, Level3Picture, Level3RegionGrid, Level3Points,Level3TopLabel, ItemName.KEY_2) },
                { RegionName.GLOOMY_GALLEON, new Region(RegionName.LOCKED_REGION, Level4, Level4Picture, Level4RegionGrid, Level4Points,Level4TopLabel, ItemName.KEY_2) },
                { RegionName.FUNGI_FOREST, new Region(RegionName.LOCKED_REGION, Level5, Level5Picture, Level5RegionGrid, Level5Points, Level5TopLabel,ItemName.KEY_4) },
                { RegionName.CRYSTAL_CAVES, new Region(RegionName.LOCKED_REGION, Level6, Level6Picture, Level6RegionGrid, Level6Points,Level6TopLabel, ItemName.KEY_5) },
                { RegionName.CREEPY_CASTLE, new Region(RegionName.LOCKED_REGION, Level7, Level7Picture, Level7RegionGrid, Level7Points,Level7TopLabel, ItemName.KEY_5) },

                { RegionName.HIDEOUT_HELM, new Region(RegionName.HIDEOUT_HELM, HideoutHelm, HideoutHelmPicture, HideoutHelmRegionGrid, HideoutHelmPoints, HideoutHelmTopLabel) },

            };
            Collectibles = new()
            {

                {ItemType.DONKEY_BLUEPRINT, DonkeyBPs },
                {ItemType.DIDDY_BLUEPRINT, DiddyBPs },
                {ItemType.LANKY_BLUEPRINT, LankyBPs },
                {ItemType.TINY_BLUEPRINT, TinyBPs },
                {ItemType.CHUNKY_BLUEPRINT, ChunkyBPs },

                {ItemType.PEARL, Pearls },
                {ItemType.BATTLE_CROWN, BattleCrowns },
                {ItemType.BANANA_MEDAL, BananaMedals },
                {ItemType.RAINBOW_COIN, RainbowCoins },
                {ItemType.FAIRY, Fairies },
                {ItemType.GOLDEN_BANANA, GBs },

            };
            Items = ItemGrid;
           Regions[RegionName.JUNGLE_JAPES].SetAsLobby1();

            //have a separate list of the movable tracker items so it's easy to find them even if they are moved out of the grid
            foreach (var control in Items.Children)
            {
                if (control is Item item)
                {
                    DraggableItems.Add(item);
                }
            }
            Autotracker = new Autotracker(ProcessNewAutotrackedItem, UpdateCollectible, SetRegionLighting);
        }

        public void SetRegionLighting(RegionName regionName, bool lightUp)
        {
            string resource = (lightUp) ? "RegionBGLitUp" : "RegionBG";
            if (!Regions.ContainsKey(regionName)) return;
            var region = Regions[regionName];
            region.MainUIGrid.SetResourceReference(Panel.BackgroundProperty, resource);
            region.RegionGrid.SetResourceReference(Panel.BackgroundProperty, resource);
        }

        public void ResetCollectibles()
        {
            foreach(var entry in Collectibles)
            {
                entry.Value.SetAmount(0);
            }
        }

        private void DisplayEndgameHint()
        {
            Random random = new();
            int randomIndex = random.Next(EndgameHints.Count);
            string hint = EndgameHints[randomIndex];
            EndgameHints.RemoveAt(randomIndex);
            HintsLabel.Text = hint;
        }

        public void UpdateCollectible(ItemType collectibleType, int newTotal)
        {
            if (Collectibles.ContainsKey(collectibleType))
            {
                Collectibles[collectibleType].SetAmount(newTotal);
            }
        }

        private bool LookingForSlam(ItemName item)
        {
            return (item == ItemName.PROGRESSIVE_SLAM_1 || item == ItemName.PROGRESSIVE_SLAM_2);
        }

        public bool ProcessNewAutotrackedItem(ItemName itemToProcess, RegionName regionName)
        {
            var check = ImportantCheckList.ITEMS[itemToProcess];
            foreach (var itemControl in DraggableItems)
            {
                if (itemControl is Item item && itemToProcess == (ItemName)item.Tag)
                {
                    if(item.Parent != ItemGrid)
                    {
                        var parent = (RegionGrid)item.Parent;
                        parent.Handle_RegionGrid(item, false);
                    }
                    var itemName = (ItemName)item.Tag;
                    item.Opacity = 1.0;
                    itemControl.CanLeftClick = false;
                    Regions[regionName].RegionGrid.Add_Item(item, false);
                    //should mean that there was no matching vial, item couldn't be placed as a result
                    if (item.Parent == ItemGrid) return false;
                    return true;
                }
            }
            return false;
        }

        private void InitOptions()
        {
            TopMostOption.IsChecked = Properties.Settings.Default.TopMost;
            TopMostToggle(null, null);

            Top = Properties.Settings.Default.WindowY;
            Left = Properties.Settings.Default.WindowX;

            Width = 570;
            Height = 1020;
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
            
        {
            

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowY = RestoreBounds.Top;
            Properties.Settings.Default.WindowX = RestoreBounds.Left;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        /// 
        /// Handle UI Changes
        /// 
        private void HandleReportValue(TextBlock Hint, int delta)
        {

            int num = Int32.Parse(Hint.Text);

            if (delta > 0)
                ++num;
            else
                --num;

            // cap hint value to 51
            if (num > 52)
                num = 52;

            if (num < 0)
                Hint.Text = 0.ToString();
            else
                Hint.Text = num.ToString();

        }

        public void SetReportValue(TextBlock Hint, int value)
        {
            Hint.Text = value.ToString();
            
        }

        public void SetHintText(string text)
        {
            //HintText.Content = text;
        }

        private void ResetSize(object sender, RoutedEventArgs e)
        {
            Width = 570;
            Height = 1020;
        }

        private void ItemGridSwap(object sender, RoutedEventArgs e)
        {
        }

        private void StarCompletionToggle(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if(button.Opacity == 0.25)
            {
                button.Opacity = 1;
            }
            else
            {
                button.Opacity = 0.25;
            }
        }

        private void FryingPanOption_Click(object sender, RoutedEventArgs e)
        {

        }

        public bool TopMostSetting
        {
            get { return Properties.Settings.Default.TopMost; }
            set { Properties.Settings.Default.TopMost = value; }
        }

        public bool AutotrackingSetting
        {
            get { return Properties.Settings.Default.Autotracking; }
            set { Properties.Settings.Default.Autotracking = value; }
        }

        private void rootGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

    }
}
