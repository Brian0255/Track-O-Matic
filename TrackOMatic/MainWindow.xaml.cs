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
using System.Timers;

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

namespace TrackOMatic
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
        public List<System.Windows.Controls.Image> kroolKongs;
        public List<System.Windows.Controls.Image> helmKongs;

        private List<Item> DraggableItems = new();
        private List<HitListItem> hitListItems;

        public int collected;
        public Autotracker Autotracker { get; private set; }
        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; }

        private Timer SaveTimer;

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

                { RegionName.JUNGLE_JAPES, new Region(RegionName.JUNGLE_JAPES, Level1, Level1Picture, Level1RegionGrid, Level1Points, Level1TopLabel, Level1Order) },
                { RegionName.ANGRY_AZTEC, new Region(RegionName.ANGRY_AZTEC, Level2, Level2Picture, Level2RegionGrid, Level2Points,Level2TopLabel, Level2Order) },
                { RegionName.FRANTIC_FACTORY, new Region(RegionName.FRANTIC_FACTORY, Level3, Level3Picture, Level3RegionGrid, Level3Points,Level3TopLabel, Level3Order) },
                { RegionName.GLOOMY_GALLEON, new Region(RegionName.GLOOMY_GALLEON, Level4, Level4Picture, Level4RegionGrid, Level4Points,Level4TopLabel, Level4Order) },
                { RegionName.FUNGI_FOREST, new Region(RegionName.FUNGI_FOREST, Level5, Level5Picture, Level5RegionGrid, Level5Points, Level5TopLabel, Level5Order) },
                { RegionName.CRYSTAL_CAVES, new Region(RegionName.CRYSTAL_CAVES, Level6, Level6Picture, Level6RegionGrid, Level6Points,Level6TopLabel, Level6Order) },
                { RegionName.CREEPY_CASTLE, new Region(RegionName.CREEPY_CASTLE, Level7, Level7Picture, Level7RegionGrid, Level7Points,Level7TopLabel, Level7Order) },

                { RegionName.HIDEOUT_HELM, new Region(RegionName.HIDEOUT_HELM, HideoutHelm, HideoutHelmPicture, HideoutHelmRegionGrid, HideoutHelmPoints, HideoutHelmTopLabel) },
            };
            hitListItems = new() { Goal1, Goal2, Goal3, Goal4, Goal5, Goal6, Goal7, Goal8, Goal9, Goal10};
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
            kroolKongs = new(){ KRoolKong1, KRoolKong2, KRoolKong3 };
            helmKongs = new() { HelmKong1, HelmKong2, HelmKong3 };

            //have a separate list of the movable tracker items so it's easy to find them even if they are moved out of the grid
            foreach (var control in Items.Children)
            {
                if (control is Item item)
                {
                    DraggableItems.Add(item);
                }
            }
            Autotracker = new Autotracker(ProcessNewAutotrackedItem, UpdateCollectible, SetRegionLighting);
            SaveTimer = new Timer(60000);
            SaveTimer.Elapsed += OnTimerSave;
            SaveTimer.Start();

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
            foreach (var entry in Collectibles)
            {
                entry.Value.SetAmount(0);
            }
        }
        private void MainWindow_Closing()
        {
            Save();
        }

        public void UpdateCollectible(ItemType collectibleType, int newTotal)
        {
            if (Collectibles.ContainsKey(collectibleType))
            {
                Collectibles[collectibleType].SetAmount(newTotal);
            }
        }

        public bool ProcessNewAutotrackedItem(ItemName itemToProcess, RegionName regionName)
        {
            foreach (var itemControl in DraggableItems)
            {
                if (itemControl is Item item && itemToProcess == (ItemName)item.Tag)
                {
                    if(item.Parent != ItemGrid)
                    {
                        var parent = (RegionGrid)item.Parent;
                        parent.Handle_RegionGrid(item, false);
                    }
                    item.Opacity = 1.0;
                    itemControl.CanLeftClick = false;
                    Regions[regionName].RegionGrid.Add_Item(item, false);
                    //should mean that there was no matching vial, item couldn't be placed as a result
                    if (item.Parent == ItemGrid) return false;
                    AddSavedItem(new SavedItem(itemToProcess, regionName, item.Star.Visibility, true, item.ItemImage.Opacity));
                    Save();
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

            ResetWidthHeight();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            Save();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WindowY = RestoreBounds.Top;
            Properties.Settings.Default.WindowX = RestoreBounds.Left;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void ResetWidthHeight()
        {
            Width = 570;
            Height = (Properties.Settings.Default.HitList) ? 980 : 820;
        }

        private void ResetSize(object sender, RoutedEventArgs e)
        {
            ResetWidthHeight();
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

        private void Reset()
        {
            TotalGBs = 0;
            SpoilerLoaded = false;
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                region.Reset();
            }
            foreach (var item in DraggableItems.Cast<Item>())
            {
                item.CanLeftClick = true;
                item.Star.Visibility = Visibility.Hidden;
            }
            foreach (var item in hitListItems) item.Reset();
            foreach (var key in Collectibles.Keys.ToList()) Collectibles[key].SetAmount(0);

            HelmKong1.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            HelmKong2.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            HelmKong3.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));

            KRoolKong1.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            KRoolKong2.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));
            KRoolKong3.Source = new BitmapImage(new Uri("Images/dk64/unknown_kong.png", UriKind.Relative));

            //Notes.Text = "";
           // Notes.Reset();
            Autotracker.Reset();
        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }

    }
}
