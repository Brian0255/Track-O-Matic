using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using TrackOMatic.Properties;
using AutoUpdaterDotNET;

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
        public BroadcastView BroadcastView { get; private set; }
        public Dictionary<RegionName, Region> Regions { get; private set; }
        public Dictionary<ItemType, CollectibleItem> Collectibles { get; private set; }
        public Dictionary<Item, ItemBackground> ITEM_TO_BACKGROUND_IMAGE { get; } = new();
        public Dictionary<ItemBackground, Item> BACKGROUND_IMAGE_TO_ITEM { get; } = new();
        public Dictionary<ItemName, RegionName> ITEM_NAME_TO_REGION { get; } = new();
        public List<ProgressiveItem> KroolKongs { get; private set; }
        public List<ProgressiveItem> HelmKongs { get; private set; }
        public List<HintPanel> HintPanels { get; private set; }

        public List<Item> DraggableItems { get; private set; } = new();
        public List<HitListItem> HitListItems { get; private set; }

        public int collected;
        public Autotracker Autotracker { get; private set; }
        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; }
        public SpoilerParser SpoilerParser { get; private set; }
        public DataSaver DataSaver { get; private set; }
        public HitListHintManager HitListHintManager { get; private set; }
        public SpoilerSettings SpoilerSettings { get; private set; }

        private List<BitmapImage> ProgressiveKongSource = new()
        {
                new BitmapImage( new Uri("images/bw/unknown_kong.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/donkey.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/diddy.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/lanky.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/tiny.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/chunky.png", UriKind.Relative)),
        };
        private List<BitmapImage> BossSource = new()
        {
                new BitmapImage( new Uri("images/dk64/army.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/doga.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/madjack.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/pufftoss.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/doga2.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/army2.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/kutout.png", UriKind.Relative)),
        };

        public Dictionary<ItemName, PathOrFoundItem> ITEM_TO_DIRECT_HINT { get; } = new();
        public Dictionary<ItemName, Item> ITEM_NAME_TO_ITEM { get; } = new();

        private Timer SaveTimer;

        public MainWindow()
        {
            InitializeComponent();
            HintData.Init();
            InitOptions();
            InitData();
            foreach (var progressiveItem in HelmKongs)
            {
                progressiveItem.ImageSources = new() { ProgressiveKongSource };
            }
            List<List<BitmapImage>> KRoolBosses = new() { ProgressiveKongSource, BossSource };
            foreach (var progressiveItem in KroolKongs)
            {
                progressiveItem.ImageSources = KRoolBosses;
            }

            SpoilerParser = new(this);
            DataSaver = new(this);
            HitListHintManager = new(this);
            Reset();
        }

        private void UpdateHintDisplayToggles()
        {
            hintDisplayOff.IsChecked = (Settings.Default.HintDisplay == "Off");
            hintDisplayMP.IsChecked = (Settings.Default.HintDisplay == "Multipath Hints");
            hintDisplayDirect.IsChecked = (Settings.Default.HintDisplay == "Direct Item Hints");
        }

        private ItemBackground FindMatchingBackgroundImage(Item item)
        {
            foreach (var control in Items.Children)
            {
                if (control is ItemBackground button)
                {
                    if ((ItemName)button.Tag == (ItemName)item.Tag) return button;
                }
            }

            return null;
        }

        private void InitData()
        {
            UpdateHintDisplayToggles();
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
            HitListItems = new() { Goal1, Goal2, Goal3, Goal4, Goal5, Goal6, Goal7, Goal8, Goal9, Goal10};
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
            KroolKongs = new(){ KRoolKong1, KRoolKong2, KRoolKong3, KRoolKong4, KRoolKong5 };
            HelmKongs = new() { HelmKong1, HelmKong2, HelmKong3, HelmKong4, HelmKong5 };
            foreach (var control in ItemGrid.Children)
            {
                if (control is Item item)
                {
                    var itemName = (ItemName)item.Tag;
                    ITEM_NAME_TO_ITEM[itemName] = item;
                }
            }

            //have a separate list of the movable tracker items so it's easy to find them even if they are moved out of the grid
            foreach (Item item in ITEM_NAME_TO_ITEM.Values)
            {
                DraggableItems.Add(item);
                var matchingButton = FindMatchingBackgroundImage(item);
                if (matchingButton != null)
                {
                    ITEM_TO_BACKGROUND_IMAGE[item] = matchingButton;
                    BACKGROUND_IMAGE_TO_ITEM[matchingButton] = item;
                }
            }


            HintPanels = new() { 
            IslesPanel,
            FactoryPanel,
            CavesPanel,
            JapesPanel,
            GalleonPanel,
            CastlePanel,
            AztecPanel,
            ForestPanel,
            HelmPanel,
            PathsPanel,
            KongsPanel,
            WotHPanel,
            FoolishPanel,
            PathlessPanel,
            PotionCountsPanel,
            UnhintedPanel

            };
            Autotracker = new Autotracker(ProcessNewAutotrackedItem, UpdateCollectible, SetRegionLighting, SetShopkeepers, SetSong);
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

        private void OnTimerSave(object sender, ElapsedEventArgs e)
        {
            //probably don't need this
            //DataSaver.Save();
        }

        public void UpdateCollectible(ItemType collectibleType, int newTotal)
        {
            if (Collectibles.ContainsKey(collectibleType))
            {
                Collectibles[collectibleType].SetAmount(newTotal);
                if (BroadcastView != null) BroadcastView.UpdateCollectible(collectibleType, newTotal);
            }
        }

        public bool ProcessNewAutotrackedItem(ItemName itemToProcess, RegionName regionName, bool hint = false)
        {
            if (regionName == RegionName.UNKNOWN) return false;
            if (!ITEM_NAME_TO_ITEM.ContainsKey(itemToProcess)) return false;
            var item = ITEM_NAME_TO_ITEM[itemToProcess];
            bool darken = hint && item.Parent == ItemGrid;
            if (item.Parent != ItemGrid)
            {
                var parent = (RegionGrid)item.Parent;
                parent.Handle_RegionGrid(item, false);
            }
            if(!hint) item.ChangeOpacity(1.0);
            Regions[regionName].RegionGrid.Add_Item(item, false, !darken);
            item.SyncImages();
            //should mean that there was no matching vial, item couldn't be placed as a result
            if (item.Parent == ItemGrid) return false;
            if (BroadcastView != null && !hint) BroadcastView.TurnItemOn(itemToProcess);
            DataSaver.AddSavedItem(new SavedItem(itemToProcess, regionName, item.Star.Visibility, true, item.ItemImage.Opacity));
            DataSaver.Save();
            return true;
        }

        private void InitOptions()
        {
            TopMostOption.IsChecked = Properties.Settings.Default.TopMost;
            TopMostToggle(null, null);

            Top = Properties.Settings.Default.WindowY;
            Left = Properties.Settings.Default.WindowX;

            ResetWidthHeight();
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
            Width = (Settings.Default.HintDisplay != "Off") ? 1800 : 580;
            Height = (Settings.Default.HitList) ? 980 : 820;
            double newColumnWidth = (Settings.Default.HintDisplay != "Off") ? 2.15 : 0;
            HintsColumn.Width = new GridLength(newColumnWidth, GridUnitType.Star);
            if (Settings.Default.HintDisplay == "Multipath Hints")
            {
                MultipathGrid.Visibility = Visibility.Visible;
                DirectItemHintGrid.Visibility = Visibility.Hidden;
            }
            if (Settings.Default.HintDisplay == "Direct Item Hints")
            {
                DirectItemHintGrid.Visibility = Visibility.Visible;
                MultipathGrid.Visibility = Visibility.Hidden;
            }
        }

        private void ResetSize(object sender, RoutedEventArgs e)
        {
            ResetWidthHeight();
        }

        public void SetSong(string songGame, string songName)
        {
            if(songName == "")
            {
                songGame = "";
                songName = "Waiting for a 4.0 ROM...";
            }
            SongGame.Text = songGame;
            SongName.Text = songName;
            if(BroadcastView != null)
            {
                BroadcastView.UpdateSongInfo(songGame, songName);
            }
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

        public void InitRegionsFromEmptySpoiler()
        {
            foreach (var entry in Regions) entry.Value.SetAsEmptySpoiler();
        }

        private void ParseSpoiler(string fileName)
        {
            SpoilerSettings = SpoilerParser.ParseSpoiler(fileName);
            foreach(var entry in SpoilerParser.StartingItems)
            {
                if (BroadcastView != null) BroadcastView.TurnItemOn(entry.Key);
            }
            Autotracker.SetStartingItems(SpoilerParser.StartingItems);
            Autotracker.SetSpoilerLoaded(fileName);
            foreach (var entry in Regions) entry.Value.SetSpoilerAsLoaded();
            if (SpoilerSettings.Empty()) InitRegionsFromEmptySpoiler();
            if (BroadcastView != null) BroadcastView.ProcessSpoilerSettings(SpoilerSettings);
            DataSaver.InitSavedDataFromSpoiler(fileName);
            HitListHintManager.InitializeFromSpoiler(SpoilerParser.StartingItems, SpoilerParser.TrainingItems);
            foreach (var entry in ITEM_TO_BACKGROUND_IMAGE) entry.Key.InitHoverPoints();
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
                Reset();
                ParseSpoiler(selectedFilePath);
            }
        }

        public void OnCollectibleTextChanged()
        {
            /*
            if (ITEM_NAME_TO_REGION.Count == 0) return;
            var itemToHint = HitListHintManager.OnGBUpdate(GBs.Text);
            if (itemToHint == ItemName.NONE) return;
            var image = (Image)FindResource(itemToHint.ToString().ToLower());
            ItemHintIcon.Source = image.Source;
            ItemHintIcon.UpdateLayout();
            var regionName = ITEM_NAME_TO_REGION[itemToHint];
            ItemHintText.Text = JSONKeyMappings.REGION_NAME_TO_SHORTENED[regionName];
            //not really an autotracked item but it's easier to just call this function with an extra parameter
            if(Settings.Default.Autotracking) ProcessNewAutotrackedItem(itemToHint, regionName, true);
            */
        }

        public void Reset()
        {
            TotalGBs = 0;
            SpoilerLoaded = false;
            ITEM_NAME_TO_REGION.Clear();
            PointValues.SpecificValues.Clear();
            PointValues.GroupedValues.Clear();
            SpoilerSettings = new SpoilerSettings();
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                region.Reset();
                region.SetLevelOrderNumber(-1);
            }
            foreach (var item in DraggableItems.Cast<Item>())
            {
                item.CanLeftClick = true;
                item.SetStarVisibility(Visibility.Hidden);
                item.ChangeOpacity(1.0);
                item.InitHoverPoints();
            }
            foreach(var hintPanel in HintPanels)
            {
                hintPanel.Reset();
            }
            BLockerHints.Reset();
            foreach (var item in HitListItems) item.Reset();
            foreach (var key in Collectibles.Keys.ToList()) Collectibles[key].SetAmount(0);
            foreach (var progressiveImage in KroolKongs) progressiveImage.Reset();
            foreach (var progressiveImage in HelmKongs) progressiveImage.Reset();
            SetSong("", "");
            Autotracker.Reset();
            if (BroadcastView != null) BroadcastView.Reset();
        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(1300, 600);
            AutoUpdater.Icon = Properties.Resources.app.ToBitmap();

            AutoUpdater.InstalledVersion = new Version("1.8.2");

            AutoUpdater.Start("https://raw.githubusercontent.com/Brian0255/Track-O-Matic/master/TrackOMatic/AutoUpdateInfo.xml");
            if (Settings.Default.DesiredHeight == 0 || Settings.Default.DesiredWidth == 0) return;
            Width = Settings.Default.DesiredWidth;
            Height = Settings.Default.DesiredHeight;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.DesiredWidth = Width;
            Settings.Default.DesiredHeight = Height;
            Settings.Default.Save();
            DataSaver.Save();
            if(BroadcastView != null)
            {
                BroadcastView.Close();
            }
        }

        public void SetShopkeepers(bool on)
        {
            var currentlyOn = ShopkeeperColumn.Width.Value > 0;
            if (currentlyOn == on) return;
            var separatorWidth = on ? 1.0 : 1.25;
            var shopkeeperColumnWidth = on ? 1.0 : 0;
            ItemsSeperator.Width = new GridLength(separatorWidth, GridUnitType.Star);
            ShopkeeperColumn.Width = new GridLength(shopkeeperColumnWidth, GridUnitType.Star);
        }

        private void BroadcastClosed(object sender, EventArgs e)
        {
            BroadcastOption.IsChecked = false;
            BroadcastView = null;
        }
    }
}
