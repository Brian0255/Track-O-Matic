using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrackOMatic;
using TrackOMatic.Properties;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for HintItemSelectionDialog.xaml
    /// </summary>
    public partial class BroadcastView : Window
    {
        public Dictionary<ItemName, bool> SelectedItems { get; private set; } = new();
        private Dictionary<ItemName, ItemBackground> ItemMap = new();

        private List<ProgressiveItem> KRoolKongs;
        private List<ProgressiveItem> HelmKongs;

        public Dictionary<ItemType, CollectibleItem> Collectibles { get; private set; }

        //need to keep count of which of these you currently have because their item display is different
        private Dictionary<ItemName, bool> SharedMoves = new()
        {
            {ItemName.PROGRESSIVE_SLAM_1, false },
            {ItemName.PROGRESSIVE_SLAM_2, false },
            {ItemName.PROGRESSIVE_SLAM_3, false },

            {ItemName.SNIPER_SCOPE, false },
            {ItemName.HOMING_AMMO, false },

            {ItemName.SHOCKWAVE, false },
            {ItemName.FAIRY_CAMERA, false },
        };

        private Dictionary<ItemName, bool> StarredSharedMoves = new()
        {
             {ItemName.PROGRESSIVE_SLAM_1, false },
            {ItemName.PROGRESSIVE_SLAM_2, false },
            {ItemName.PROGRESSIVE_SLAM_3, false },

            {ItemName.SNIPER_SCOPE, false },
            {ItemName.HOMING_AMMO, false },

            {ItemName.SHOCKWAVE, false },
            {ItemName.FAIRY_CAMERA, false },
        };

        private Dictionary<RegionName, int> LevelNumbers = new()
        {
            {RegionName.JUNGLE_JAPES, -1 },
            {RegionName.ANGRY_AZTEC , -1 },
            {RegionName.FRANTIC_FACTORY, -1 },
            {RegionName.GLOOMY_GALLEON , -1 },
            {RegionName.FUNGI_FOREST, -1 },
            {RegionName.CRYSTAL_CAVES , -1 },
            {RegionName.CREEPY_CASTLE, -1 },
        };

        private List<BitmapImage> homingScopeImages = new()
        {
                new BitmapImage( new Uri("images/bw/homingscope.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/homingonly.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/scopeonly.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/homingscope.png", UriKind.Relative)),
        };

        private List<BitmapImage> camShockwaveImages = new()
        {
                new BitmapImage( new Uri("images/bw/filmwave.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/fairycamonly.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/shockwaveonly.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/filmwave.png", UriKind.Relative)),
        };

        private List<BitmapImage> slamImages = new()
        {
                new BitmapImage( new Uri("images/bw/progressive_slam.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/progressive_slam.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/slam2.png", UriKind.Relative)),
                new BitmapImage( new Uri("images/dk64/slam3.png", UriKind.Relative)),
        };

        private List<Image> LevelNames;
        private List<TextBlock> PointLabels;
        private void InitializeMap()
        {
            var keys = new List<ItemBackground>() { key_1, key_2, key_3, key_4, key_5, key_6, key_7, key_8 };
            var itemGrids = new List<UIElementCollection>()
            {
                MainKongMoves.Children, TrainingMovesGrid.Children, CollectiblesGrid.Children, ShopkeepersGrid.Children
            };
            foreach (var itemGrid in itemGrids) 
            {
                foreach (var control in itemGrid)
                {
                    if (control is ItemBackground item)
                    {
                        ItemName itemName = (ItemName)item.Tag;
                        ItemMap[itemName] = item;
                    }
                }
            }
            foreach(var key in keys)
            {
                ItemName itemName = (ItemName)key.Tag;
                ItemMap[itemName] = key;
            }
        }

        public void InitializeFromItems(Dictionary<ItemName, Item> items)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            foreach(var entry in items)
            {
                var itemName = entry.Key;
                var item = entry.Value;
                if(itemName == ItemName.KEY_6)
                {
                    Console.WriteLine("???");
                }
                if (GetMatchingItem(itemName) != null || SharedMoves.ContainsKey(itemName))
                {
                    if (mainWindow.ITEM_NAME_TO_ITEM.ContainsKey(itemName))
                    {
                        mainWindow.ITEM_NAME_TO_ITEM[itemName].InitHoverPoints();
                    }
                    SetItemStar(itemName, item.Star.Visibility);
                    if(item.Brightened && item.ItemImage.Opacity > 0.9)
                    {
                        TurnItemOn(itemName);
                    }
                }
            }
        }
        public BroadcastView()
        {
            InitializeComponent();
            DataContext = this;
            InitializeMap();
            Collectibles = new() {
                { ItemType.DONKEY_BLUEPRINT, DonkeyBPs},
                { ItemType.DIDDY_BLUEPRINT, DiddyBPs},
                { ItemType.LANKY_BLUEPRINT, LankyBPs },
                { ItemType.TINY_BLUEPRINT, TinyBPs },
                { ItemType.CHUNKY_BLUEPRINT, ChunkyBPs },

                {ItemType.PEARL, pearls },
                {ItemType.BATTLE_CROWN, battle_crowns },
                {ItemType.BANANA_MEDAL, banana_medals },
                {ItemType.RAINBOW_COIN, rainbow_coins },
                {ItemType.FAIRY, banana_fairies },
                {ItemType.GOLDEN_BANANA, golden_bananas },
            };
            LevelNames = new()
            {
                Level1Image,
                Level2Image,
                Level3Image,
                Level4Image,
                Level5Image,
                Level6Image,
                Level7Image
            };
            PointLabels = new()
            {
                Level1Points,
                Level2Points,
                Level3Points,
                Level4Points,
                Level5Points,
                Level6Points,
                Level7Points,
                HelmPoints,
                IslesPoints,
            };
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            KRoolKongs = new() { KRoolKong1, KRoolKong2, KRoolKong3, KRoolKong4, KRoolKong5 };
            HelmKongs = new() { HelmKong1, HelmKong2, HelmKong3, HelmKong4, HelmKong5 };
            for(int i =0; i < KRoolKongs.Count;++i)
            {
                var item = KRoolKongs[i];
                item.Enabled = false;
                UpdateKRoolKong(i, mainWindow.KroolKongs[i].image.Source);
            }
            for (int i = 0; i < HelmKongs.Count; ++i)
            {
                var item = HelmKongs[i];
                item.Enabled = false;
                UpdateHelmKong(i, mainWindow.HelmKongs[i].image.Source);
            }
            UpdateShopkeeperHeight();
            AdjustWindowSize();
        }

        public void UpdateKRoolKong(int index, ImageSource newSource)
        {
            KRoolKongs[index].image.Source = newSource;
        }

        public void UpdateHelmKong(int index, ImageSource newSource)
        {
            HelmKongs[index].image.Source = newSource;
        }

        public void UpdateCollectible(ItemType itemType, int newAmount)
        {
            if (!Collectibles.ContainsKey(itemType)) return;
            Collectibles[itemType].SetAmount(newAmount);
        }

        private void ClearLevelLabels()
        {
            foreach(var image in LevelNames)
            {
                image.Source = new BitmapImage(new Uri("Images/dk64/unknown.png", UriKind.Relative));
            }
        }

        public void Reset()
        {
            ClearLevelLabels();
            foreach(var entry in Collectibles)
            {
                entry.Value.SetAmount(0);
            }
            foreach(var key in SharedMoves.Keys.ToList())
            {
                SharedMoves[key] = false;
            }
            foreach (var key in StarredSharedMoves.Keys.ToList())
            {
                StarredSharedMoves[key] = false;
            }
            foreach (var key in LevelNumbers.Keys.ToList())
            {
                LevelNumbers[key] = -1;
            }
            foreach(var label in PointLabels)
            {
                label.Text = "";
            }
            MovesWidth.Width = new GridLength(345, GridUnitType.Pixel);
            IslesGrid.Visibility = Visibility.Collapsed;
        }

        public void ProcessSpoilerSettings(SpoilerSettings settings)
        {
            var width = settings.PointsEnabled ? 315 : 345;
            foreach(var label in PointLabels)
            {
                label.Visibility = (settings.PointsEnabled) ? Visibility.Visible : Visibility.Collapsed;
            }
            IslesGrid.Visibility = (settings.PointsEnabled) ? Visibility.Visible : Visibility.Collapsed;
            MovesWidth.Width = new GridLength(width, GridUnitType.Pixel);
        }

        public void AdjustWindowSize()
        {
            var baseHeight = 394;
            if(ShopkeepersRow.Height.Value > 0)
            {
                baseHeight += 47;
            }
            if(song_display.Height.Value > 0)
            {
                baseHeight += 50;
            }
            if(HelmKRool.Height.Value > 0)
            {
                baseHeight += 47;
            }
            Height = baseHeight;
        }

        public void UpdateSongInfo(string songGame, string songName)
        {
            SongName.Text = songName;
            SongGame.Text = songGame;
        }

        public void UpdateShopkeeperHeight()
        {
            bool on = Settings.Default.BroadcastShopkeepers;
            var shopkeeperHeight = on ? 1.0 : 0;
            var mainItemsHeight = on ? 336 : 290;
            var climbingColumnWidth = on ? 0 : 0;
            ShopkeepersRow.Height = new GridLength(shopkeeperHeight, GridUnitType.Star);
            MainItemsRow.Height = new GridLength(mainItemsHeight,GridUnitType.Pixel);
            ClimbingColumn.Width = new GridLength(climbingColumnWidth, GridUnitType.Star);
            AdjustWindowSize();
        }
        private void UpdateLevelNumbers()
        {
            ClearLevelLabels();
            foreach(var entry in LevelNumbers)
            {
                var region = entry.Key;
                var levelNumber = entry.Value;
                if (levelNumber <= -1) continue;
                var matchingImage = LevelNames[levelNumber];
                var imagePath = "Images/dk64/" + region.ToString().ToLower() + "_label.png";
                matchingImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            }
        }

        public void UpdateRegionPoints(RegionName region, int points, string foregroundResource)
        {
            var levelIndex = -1;
            if (region == RegionName.HIDEOUT_HELM) levelIndex = 7;
            else if (region == RegionName.DK_ISLES) levelIndex = 8;
            else if(LevelNumbers.ContainsKey(region))
            {
                levelIndex = LevelNumbers[region];
            }
            if (levelIndex == -1) return;
            PointLabels[levelIndex].Text = points.ToString();
            PointLabels[levelIndex].SetResourceReference(TextBlock.ForegroundProperty, foregroundResource);
        }

        public void UpdateLevelNumber(RegionName region, int newNumber)
        {
            newNumber -= 1;
            LevelNumbers[region] = newNumber;
            UpdateLevelNumbers();
        }

        private void ProcessStarredSharedMoves()
        {
            var groupings = new Dictionary<ItemBackground, List<ItemName>>()
            {
                {homingscope, new(){ItemName.HOMING_AMMO, ItemName.SNIPER_SCOPE} },
                {camerashockwave, new(){ItemName.FAIRY_CAMERA, ItemName.SHOCKWAVE} },
                {slam, new(){ItemName.PROGRESSIVE_SLAM_1, ItemName.PROGRESSIVE_SLAM_2, ItemName.PROGRESSIVE_SLAM_3, } },
            };
            foreach(var entry in groupings)
            {
                var items = entry.Value;
                var image = entry.Key;
                image.SetStarVisibility(Visibility.Collapsed);
                foreach(var item in items)
                {
                    if (StarredSharedMoves[item])
                    {
                        image.SetStarVisibility(Visibility.Visible);
                    }
                }
            }
        }

        public void SetItemStar(ItemName item, Visibility visibility)
        {
            if (StarredSharedMoves.ContainsKey(item))
            {
                StarredSharedMoves[item] = (visibility == Visibility.Visible);
                ProcessStarredSharedMoves();
                return;
            }
            var match = GetMatchingItem(item);
            if (match != null) match.SetStarVisibility(visibility);
        }

        private void CheckGroupedItem(List<ItemName> items, List<BitmapImage> imageSources, ItemBackground itemBackground)
        {
            int imageIndex = 0;
            ItemName firstItem = items[0];
            ItemName secondItem = items[1];
            if (SharedMoves[firstItem]) imageIndex++;
            if (SharedMoves[secondItem]) imageIndex += 2;
            itemBackground.BackgroundItemImage.Source = imageSources[imageIndex];
        }

        public void HandleSharedMoves()
        {
            int slamCount = 0;
            foreach(var entry in SharedMoves)
            {
                if(entry.Key.ToString().Contains("PROGRESSIVE_SLAM") && entry.Value == true)
                {
                    slamCount++;
                }
            }
            slam.BackgroundItemImage.Source = slamImages[slamCount];
            var homingScopeGroup = new List<ItemName> { ItemName.HOMING_AMMO, ItemName.SNIPER_SCOPE };
            var camShockwaveGroup = new List<ItemName> { ItemName.FAIRY_CAMERA, ItemName.SHOCKWAVE };
            CheckGroupedItem(homingScopeGroup, homingScopeImages, homingscope);
            CheckGroupedItem(camShockwaveGroup, camShockwaveImages, camerashockwave);
        }

        private ItemBackground GetMatchingItem(ItemName item)
        {
            var name = item.ToString();
            if (name.StartsWith("KEY"))
            {
                return (ItemBackground)FindName(item.ToString().ToLower());
            }
            if(item == ItemName.PROGRESSIVE_SLAM_1 || item == ItemName.PROGRESSIVE_SLAM_2 || item == ItemName.PROGRESSIVE_SLAM_3)
            {
                return slam;
            }
            if (item == ItemName.SNIPER_SCOPE || item == ItemName.HOMING_AMMO) return homingscope;
            if (item == ItemName.FAIRY_CAMERA || item == ItemName.SHOCKWAVE) return camerashockwave;
            if (ItemMap.ContainsKey(item)) return ItemMap[item];
            return null;
        }

        public void TurnItemOn(ItemName item)
        {
            if (SharedMoves.ContainsKey(item))
            {
                SharedMoves[item] = true;
                HandleSharedMoves();
                return;
            }
            var match = GetMatchingItem(item);
            if(match != null) match.ItemBrightnessChanger.Brighten();
        }

        public void TurnItemOff(ItemName item)
        {
            if (SharedMoves.ContainsKey(item))
            {
                SharedMoves[item] = false;
                HandleSharedMoves();
                return;
            }
            var match = GetMatchingItem(item);
            if (match != null) match.ItemBrightnessChanger.Darken();
        }

        public void ActivateTooltip(ItemName item, string hoverText)
        {
            var match = GetMatchingItem(item);
            if (match == null) return;
            match.ToolTip.Content = hoverText;
            match.ToolTip.Visibility = Visibility.Visible;
        }

        public void DisableTooltip(ItemName item)
        {
            var match = GetMatchingItem(item);
            if (match == null) return;
            match.ToolTip.Visibility = Visibility.Collapsed;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           
        }
    }
}
