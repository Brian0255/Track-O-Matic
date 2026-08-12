using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using AutoUpdaterDotNET;

using Microsoft.Win32;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

using Timer = System.Timers.Timer;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int TotalGBs { get; private set; }
        public BroadcastView? BroadcastView { get; private set; }
        public Dictionary<RegionName, Region> Regions { get; private set; } = null!;
        public Dictionary<Item, ItemBackground> ITEM_TO_BACKGROUND_IMAGE { get; } = new();
        public Dictionary<ItemBackground, Item> BACKGROUND_IMAGE_TO_ITEM { get; } = new();
        public Dictionary<ItemName, RegionName> ITEM_NAME_TO_REGION { get; } = new();
        public List<ProgressiveItem> BossKongs { get; private set; }
        public List<ProgressiveItem> HelmKongs { get; private set; }
        public List<HintPanel> HintPanels { get; private set; } = null!;

        public List<Item> DraggableItems { get; private set; } = new();

        public int collected;
        public IAutotrackerService Autotracker { get; private set; } = null!;
        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; } = null!;
        public SpoilerParser SpoilerParser { get; private set; }
        public DataSaver DataSaver { get; private set; }
        public SpoilerSettings SpoilerSettings { get; private set; } = null!;

        private List<BitmapImage> ProgressiveKongSource { get; init; }
        private List<BitmapImage> BossSource { get; init; }

        public Dictionary<ItemName, PathOrFoundItem> ITEM_TO_DIRECT_HINT { get; } = new();
        public Dictionary<ItemName, Item> ITEM_NAME_TO_ITEM { get; } = new();

        private string _applicationVersion = "";
        public string ApplicationVersion
        {
            get => _applicationVersion;
            private set
            {
                if (_applicationVersion != value)
                {
                    _applicationVersion = value;
                    OnPropertyChanged(nameof(ApplicationVersion));
                }
            }
        }

        public IUserSettingsService UserSettings { get; init; }
        public IApplicationStateService AppState { get; init; }
        public IDataPersistenceService DataPersistenceService { get; init; }
        public ISpoilerService SpoilerService { get; init; }
        public IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

        public ILevelOrderService LevelOrderService { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Proxy Properties for User Settings

        /// <summary>
        /// Proxy property for XAML binding to the broadcast Helm & K. Rool setting.
        /// </summary>
        public bool BroadcastHelmKRoolSetting
        {
            get => UserSettings.BroadcastHelmKRool;
            set
            {
                if (UserSettings.BroadcastHelmKRool != value)
                {
                    UserSettings.BroadcastHelmKRool = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the broadcast shopkeepers setting.
        /// </summary>
        public bool BroadcastShopkeepersSetting
        {
            get => UserSettings.BroadcastShopkeepers;
            set
            {
                if (UserSettings.BroadcastShopkeepers != value)
                {
                    UserSettings.BroadcastShopkeepers = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the broadcast song display setting.
        /// </summary>
        public bool BroadcastSongDisplaySetting
        {
            get => UserSettings.BroadcastSongDisplay;
            set
            {
                if (UserSettings.BroadcastSongDisplay != value)
                {
                    UserSettings.BroadcastSongDisplay = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the colored barrel/pad moves setting.
        /// </summary>
        public bool ColoredBarrelPadMovesSetting
        {
            get => UserSettings.ColoredBarrelPadMoves;
            set
            {
                if (UserSettings.ColoredBarrelPadMoves != value)
                {
                    UserSettings.ColoredBarrelPadMoves = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the song display setting.
        /// </summary>
        public bool SongDisplaySetting
        {
            get => UserSettings.SongDisplay;
            set
            {
                if (UserSettings.SongDisplay != value)
                {
                    UserSettings.SongDisplay = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the compact mode setting.
        /// </summary>
        public bool CompactModeSetting
        {
            get => UserSettings.CompactMode;
            set
            {
                if (UserSettings.CompactMode != value)
                {
                    UserSettings.CompactMode = value;
                    AdjustBasedOnCompactMode();
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the topmost window setting.
        /// </summary>
        public bool TopMostSetting
        {
            get => UserSettings.TopMost;
            set
            {
                if (UserSettings.TopMost != value)
                {
                    UserSettings.TopMost = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the Helm in level order setting.
        /// </summary>
        public bool HelmInLevelOrderSetting
        {
            get => UserSettings.HelmInLevelOrder;
            set
            {
                if (UserSettings.HelmInLevelOrder != value)
                {
                    UserSettings.HelmInLevelOrder = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the Helm doors setting.
        /// </summary>
        public bool HelmDoorsSetting
        {
            get => UserSettings.HelmDoors;
            set
            {
                if (UserSettings.HelmDoors != value)
                {
                    UserSettings.HelmDoors = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the auto-sort path hints setting.
        /// </summary>
        public bool AutoSortPathHintsSetting
        {
            get => UserSettings.AutoSortPathHints;
            set
            {
                if (UserSettings.AutoSortPathHints != value)
                {
                    UserSettings.AutoSortPathHints = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the enemies in autofill setting.
        /// </summary>
        public bool EnemiesInAutofillSetting
        {
            get => UserSettings.EnemiesInAutofill;
            set
            {
                if (UserSettings.EnemiesInAutofill != value)
                {
                    UserSettings.EnemiesInAutofill = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the autotracking setting.
        /// </summary>
        public bool AutotrackingSetting
        {
            get => UserSettings.Autotracking;
            set
            {
                if (UserSettings.Autotracking != value)
                {
                    UserSettings.Autotracking = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the show total blueprints setting.
        /// </summary>
        public bool ShowTotalBlueprintsSetting
        {
            get => UserSettings.ShowTotalBPs;
            set
            {
                if (UserSettings.ShowTotalBPs != value)
                {
                    UserSettings.ShowTotalBPs = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the show total company coins setting.
        /// </summary>
        public bool ShowTotalCompanyCoinsSetting
        {
            get => UserSettings.ShowCompanyCoins;
            set
            {
                if (UserSettings.ShowCompanyCoins != value)
                {
                    UserSettings.ShowCompanyCoins = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the show K. Rool order setting.
        /// </summary>
        public bool ShowKRoolOrderSetting
        {
            get => UserSettings.ShowKRoolOrder;
            set
            {
                if (UserSettings.ShowKRoolOrder != value)
                {
                    UserSettings.ShowKRoolOrder = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the show Helm order setting.
        /// </summary>
        public bool ShowHelmOrderSetting
        {
            get => UserSettings.ShowHelmOrder;
            set
            {
                if (UserSettings.ShowHelmOrder != value)
                {
                    UserSettings.ShowHelmOrder = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the hint display setting.
        /// </summary>
        public HintDisplayMode HintDisplaySetting
        {
            get => UserSettings.HintDisplay;
            set
            {
                if (UserSettings.HintDisplay != value)
                {
                    UserSettings.HintDisplay = value;
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the show amount for hints setting.
        /// </summary>
        public bool ShowAmountForHintsSetting
        {
            get => UserSettings.ShowAmountForHints;
            set
            {
                if (UserSettings.ShowAmountForHints != value)
                {
                    UserSettings.ShowAmountForHints = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Helper method to get the LegacyName attribute value for a HintDisplayMode enum value.
        /// </summary>
        private static string GetHintDisplayModeName(HintDisplayMode mode)
        {
            var field = typeof(HintDisplayMode).GetField(mode.ToString());
            if (field == null)
            {
                return mode.ToString();
            }

            var legacyAttr = field.GetCustomAttribute<LegacyNameAttribute>();
            return legacyAttr?.LegacyName ?? mode.ToString();
        }

        /// <summary>
        /// Returns the LegacyName attribute value for the current HintDisplay setting.
        /// Used for XAML DataTrigger bindings in styles.
        /// Example: HintDisplayMode.DirectItemHints → "Direct Item Hints"
        /// </summary>
        public string HintDisplayModeName
        {
            get => GetHintDisplayModeName(UserSettings.HintDisplay);
        }

        // Timer to save the data every minute. This is properly initialized, but the compiler is finicky.
        private Timer SaveTimer = null!;
        public MainWindow(
            IUserSettingsService settingsSergice,
            IApplicationStateService appStateService,
            IDataPersistenceService dataPersistenceService,
            ISpoilerService spoilerService,
            IParsedSpoilerDataService parsedSpoilerDataService,
            ILevelOrderService levelOrderService,
            IAutotrackerService autotrackerService
        )
        {
            DataContext = this;
            InitializeComponent();
            UserSettings = settingsSergice;
            AppState = appStateService;
            DataPersistenceService = dataPersistenceService;
            SpoilerService = spoilerService;
            ParsedSpoilerDataService = parsedSpoilerDataService;
            Autotracker = autotrackerService;
            LevelOrderService = levelOrderService;

            // Subscribe to settings changes to notify XAML bindings
            UserSettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserSettings.BroadcastHelmKRool))
                {
                    OnPropertyChanged(nameof(BroadcastHelmKRoolSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.BroadcastSongDisplay))
                {
                    OnPropertyChanged(nameof(BroadcastSongDisplaySetting));
                }
                else if (e.PropertyName == nameof(UserSettings.BroadcastShopkeepers))
                {
                    OnPropertyChanged(nameof(BroadcastShopkeepersSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.ColoredBarrelPadMoves))
                {
                    OnPropertyChanged(nameof(ColoredBarrelPadMovesSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.SongDisplay))
                {
                    OnPropertyChanged(nameof(SongDisplaySetting));
                }
                else if (e.PropertyName == nameof(UserSettings.CompactMode))
                {
                    OnPropertyChanged(nameof(CompactModeSetting));
                    AdjustBasedOnCompactMode();
                }
                else if (e.PropertyName == nameof(UserSettings.TopMost))
                {
                    OnPropertyChanged(nameof(TopMostSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.HelmInLevelOrder))
                {
                    OnPropertyChanged(nameof(HelmInLevelOrderSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.HelmDoors))
                {
                    OnPropertyChanged(nameof(HelmDoorsSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.AutoSortPathHints))
                {
                    OnPropertyChanged(nameof(AutoSortPathHintsSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.EnemiesInAutofill))
                {
                    OnPropertyChanged(nameof(EnemiesInAutofillSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.Autotracking))
                {
                    OnPropertyChanged(nameof(AutotrackingSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.ShowTotalBPs))
                {
                    OnPropertyChanged(nameof(ShowTotalBlueprintsSetting));
                    AdjustCollectibleColumns();
                }
                else if (e.PropertyName == nameof(UserSettings.ShowCompanyCoins))
                {
                    OnPropertyChanged(nameof(ShowTotalCompanyCoinsSetting));
                    AdjustCollectibleColumns();
                }
                else if (e.PropertyName == nameof(UserSettings.ShowKRoolOrder))
                {
                    OnPropertyChanged(nameof(ShowKRoolOrderSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.ShowHelmOrder))
                {
                    OnPropertyChanged(nameof(ShowHelmOrderSetting));
                }
                else if (e.PropertyName == nameof(UserSettings.HintDisplay))
                {
                    OnPropertyChanged(nameof(HintDisplaySetting));
                    OnPropertyChanged(nameof(HintDisplayModeName));
                }
                else if (e.PropertyName == nameof(UserSettings.ShowAmountForHints))
                {
                    OnPropertyChanged(nameof(ShowAmountForHintsSetting));
                }
            };

            Autotracker.HintProgressUpdated += (sender, e) => Dispatcher.Invoke(() => UpdateUIAmountToNextHint(e.AmountToNextHint));
            Autotracker.ProgHintItemUpdated += (sender, e) => Dispatcher.Invoke(() => UpdateProgHintImage(e.ProgHintItem));

            if (UserSettings.Autotracking)
            {
                Autotracker.Start();
            }

            HintData.Init();
            InitOptions();
            InitData();
            ProgressiveKongSource =
            [
                (BitmapImage)FindResource("unknown_kong_bw"),
                (BitmapImage)FindResource("donkey"),
                (BitmapImage)FindResource("diddy"),
                (BitmapImage)FindResource("lanky"),
                (BitmapImage)FindResource("tiny"),
                (BitmapImage)FindResource("chunky"),
            ];
            foreach (var progressiveItem in HelmKongs!)
            {
                progressiveItem.ImageSources = [ProgressiveKongSource];
            }
            BossSource =
            [
                (BitmapImage)FindResource("army"),
                (BitmapImage)FindResource("doga"),
                (BitmapImage)FindResource("madjack"),
                (BitmapImage)FindResource("pufftoss"),
                (BitmapImage)FindResource("doga2"),
                (BitmapImage)FindResource("army2"),
                (BitmapImage)FindResource("kutout"),
            ];
            List<List<BitmapImage>> allBosses = [ProgressiveKongSource, BossSource];
            foreach (var progressiveItem in BossKongs!)
            {
                progressiveItem.ImageSources = allBosses;
            }

            SpoilerParser = new(this, UserSettings, SpoilerService);
            DataSaver = new(this, DataPersistenceService, ParsedSpoilerDataService, SpoilerService);
            Reset();
            AdjustBasedOnCompactMode();
        }

        private void UpdateHintDisplayToggles()
        {
            hintDisplayOff.IsChecked = (UserSettings.HintDisplay == HintDisplayMode.Off);
            hintDisplayMP.IsChecked = (UserSettings.HintDisplay == HintDisplayMode.MultipathHints);
            hintDisplayDirect.IsChecked = (UserSettings.HintDisplay == HintDisplayMode.DirectItemHints);
        }

        private void UpdateBroadcastNumberDisplayToggles()
        {
            broadcastNumberDisplayPoints.IsChecked = (UserSettings.BroadcastNumberLabel == BroadcastNumberLabel.Points);
            broadcastNumberDisplayWOTHCount.IsChecked = (UserSettings.BroadcastNumberLabel == BroadcastNumberLabel.WothCount);
        }

        private ItemBackground? FindMatchingBackgroundImage(Item item)
        {
            foreach (var control in Items.Children)
            {
                if (control is ItemBackground button)
                {
                    if ((ItemName)button.Tag == (ItemName)item.Tag)
                    {
                        return button;
                    }
                }
            }

            return null;
        }

        private void InitData()
        {
            UpdateHintDisplayToggles();
            UpdateBroadcastNumberDisplayToggles();
            Regions = [];
            Items = ItemGrid;
            BossKongs = new() { BossKong1, BossKong2, BossKong3, BossKong4, BossKong5 };
            HelmKongs = [];

            HintPanels = [
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
                PotionCountsPanel,
                UnhintedPanel
            ];

            SaveTimer = new Timer(60000);
            SaveTimer.Elapsed += OnTimerSave;
            SaveTimer.Start();
        }

        private void OnTimerSave(object? sender, ElapsedEventArgs e)
        {
            //probably don't need this
            //DataSaver.Save();
        }

        public bool ProcessNewAutotrackedItem(ItemName itemToProcess, RegionName regionName, bool hint = false, bool canAutosave = true)
        {
            return false; // Will come back.
            if (regionName == RegionName.UNKNOWN)
            {
                return false;
            }

            if (!ITEM_NAME_TO_ITEM.ContainsKey(itemToProcess))
            {
                return false;
            }

            var item = ITEM_NAME_TO_ITEM[itemToProcess];
            bool darken = hint && item.Parent == ItemGrid;
            if (item.Parent != ItemGrid)
            {
                var parent = (RegionGrid)item.Parent;
                parent.Handle_RegionGrid(item, false);
            }
            if (!hint)
            {
                item.ChangeOpacity(1.0);
            }

            Regions[regionName].RegionGrid.Add_Item(item, false, !darken);
            //should mean that there was no matching vial, item couldn't be placed as a result
            if (item.Parent == ItemGrid)
            {
                return false;
            }

            if (BroadcastView != null && !hint)
            {
                BroadcastView.TurnItemOn(itemToProcess);
            }

            DataSaver.AddSavedItem(new SavedItem(itemToProcess, regionName, item.Star.Visibility.ToItemVisibility(), true, item.Image.Opacity));
            DataSaver.Save("autosave.json", canAutosave);
            return true;
        }

        private void InitOptions()
        {
            TopMostOption.IsChecked = UserSettings.TopMost;
            TopMostToggle(null, null);

            Top = AppState.WindowY;
            Left = AppState.WindowX;

            ResetWidthHeight();
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            AppState.WindowY = RestoreBounds.Top;
            AppState.WindowX = RestoreBounds.Left;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Do we want to allow saving the resized values all the time, or just at the end? Hmm...
        }

        private void ResetWidthHeight()
        {
            double newWidth = 580;
            if (UserSettings.HintDisplay != HintDisplayMode.Off)
            {
                newWidth = (UserSettings.CompactMode) ? 1392.0 : 1800.0;
            }
            Width = newWidth;
            Height = 820;
            if (UserSettings.HintDisplay == HintDisplayMode.MultipathHints)
            {
                MultipathGrid.Visibility = Visibility.Visible;
                DirectItemHintGrid.Visibility = Visibility.Hidden;
            }
            if (UserSettings.HintDisplay == HintDisplayMode.DirectItemHints)
            {
                DirectItemHintGrid.Visibility = Visibility.Visible;
                MultipathGrid.Visibility = Visibility.Hidden;
            }
        }

        private void CompactModeMultipathChanges(bool on)
        {
            var multipath2Width = on ? 0 : 1;
            MultipathColumn2.Width = new GridLength(multipath2Width, GridUnitType.Star);
            var multipathColumn1Row0Height = on ? 1.6 : 0;
            MultipathColumn1Row0.Height = new GridLength(multipathColumn1Row0Height, GridUnitType.Star);
            var potionsRowHeight = on ? 2.54 : 0;
            OptionalPotionsRow.Height = new GridLength(potionsRowHeight, GridUnitType.Star);

            if (on)
            {
                MultipathMainColumn2.Children.Remove(FoolishPanel);
                MultipathMainColumn2.Children.Remove(PotionCountsPanel);
                MultipathColumn1.Children.Add(FoolishPanel);
                Grid.SetRow(FoolishPanel, 0);
                UnhintedColumn.Children.Add(PotionCountsPanel);
                Grid.SetRow(PotionCountsPanel, 0);
            }
            else
            {
                UnhintedColumn.Children.Remove(PotionCountsPanel);
                MultipathColumn1.Children.Remove(FoolishPanel);
                MultipathMainColumn2.Children.Add(FoolishPanel);
                Grid.SetRow(FoolishPanel, 0);
                MultipathMainColumn2.Children.Add(PotionCountsPanel);
                Grid.SetRow(PotionCountsPanel, 1);
            }
        }

        private void CompactModeDirectItemChanges(bool on)
        {
            //god i hate this
            var lastRowHeights = on ? 1 : 0;
            DirectHintsCol0LastRow.Height = new GridLength(lastRowHeights, GridUnitType.Star);
            DirectHintsCol1LastRow.Height = new GridLength(lastRowHeights, GridUnitType.Star);
            var thirdColumnWidth = on ? 0 : 1;
            DirectHintsThirdMain.Width = new GridLength(thirdColumnWidth, GridUnitType.Star);
            DirectHintsCol0.Children.Clear();
            DirectHintsCol1.Children.Clear();
            DirectHintsCol2.Children.Clear();
            if (on)
            {
                UIUtils.AddToGridRow(DirectHintsCol0, IslesPanel, 0);
                UIUtils.AddToGridRow(DirectHintsCol0, AztecPanel, 1);
                UIUtils.AddToGridRow(DirectHintsCol0, GalleonPanel, 2);
                UIUtils.AddToGridRow(DirectHintsCol0, CavesPanel, 3);
                UIUtils.AddToGridRow(DirectHintsCol1, JapesPanel, 0);
                UIUtils.AddToGridRow(DirectHintsCol1, FactoryPanel, 1);
                UIUtils.AddToGridRow(DirectHintsCol1, ForestPanel, 2);
                UIUtils.AddToGridRow(DirectHintsCol1, CastlePanel, 3);
                UIUtils.AddToGridRow(UnhintedColumn, HelmPanel, 0);
            }
            else
            {
                UnhintedColumn.Children.Remove(HelmPanel);
                UIUtils.AddToGridRow(DirectHintsCol0, IslesPanel, 0);
                UIUtils.AddToGridRow(DirectHintsCol0, FactoryPanel, 1);
                UIUtils.AddToGridRow(DirectHintsCol0, CavesPanel, 2);
                UIUtils.AddToGridRow(DirectHintsCol1, JapesPanel, 0);
                UIUtils.AddToGridRow(DirectHintsCol1, GalleonPanel, 1);
                UIUtils.AddToGridRow(DirectHintsCol1, CastlePanel, 2);
                UIUtils.AddToGridRow(DirectHintsCol2, AztecPanel, 0);
                UIUtils.AddToGridRow(DirectHintsCol2, ForestPanel, 1);
                UIUtils.AddToGridRow(DirectHintsCol2, HelmPanel, 2);
            }
        }

        private void AdjustBasedOnCompactMode()
        {
            var isActuallyOn = (MultipathColumns.Width.Value == 2 && MultipathColumns.Width.IsStar);
            var on = UserSettings.CompactMode;
            if (on == isActuallyOn)
            {
                return;
            }

            var totalColumns = on ? 2 : 3;
            MultipathColumns.Width = new GridLength(totalColumns, GridUnitType.Star);
            double newRatio = on ? 1.43 : 2.15;
            if (UserSettings.HintDisplay != HintDisplayMode.Off)
            {
                HintsColumn.Width = new GridLength(newRatio, GridUnitType.Star);
            }
            var newWidth = on ? (Width * (1392.0 / 1800.0)) : (Width * (1800.0 / 1392.0));
            Width = newWidth;
            CompactModeMultipathChanges(on);
            CompactModeDirectItemChanges(on);
        }

        private void AdjustCollectibleColumns()
        {
            // When ShowTotalBPs is false and ShowCompanyCoins is true, move Blueprints Total to column 6
            if (UserSettings.ShowTotalBPs && !UserSettings.ShowCompanyCoins)
            {
                Grid.SetColumn(BlueprintsTotal, 6);
            }
            else
            {
                Grid.SetColumn(BlueprintsTotal, 4);
            }
        }

        private void ResetSize(object sender, RoutedEventArgs e)
        {
            ResetWidthHeight();
        }

        private void rootGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        public void InitRegionsFromEmptySpoiler()
        {
            foreach (var entry in Regions)
            {
                entry.Value.SetAsEmptySpoiler();
            }
        }

        public async Task ParseSpoiler(string fileName)
        {
            try
            {
                SpoilerSettings = await SpoilerParser.ParseSpoilerAsync(fileName);

                if (!SpoilerSettings.Empty())
                {
                    Autotracker.SetStartingItems(SpoilerParser.StartingItems);
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception)
            {
                // No matter what the exception is, treat it as a reset situation.
                Reset();
            }
        }

        private async void DropFile(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Path.GetExtension(files[0]).ToUpper().Equals(".JSON"))
                {
                    Reset();
                    await ParseSpoiler(files[0]);
                    DataSaver.setSpoilerPath(files[0]);
                }
            }
        }

        private async void OpenSpoiler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            string lastFolderPath = AppState.LastFolderPath;

            if (!string.IsNullOrEmpty(lastFolderPath))
            {
                openFileDialog.InitialDirectory = lastFolderPath;
            }

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string folderPath = Path.GetDirectoryName(selectedFilePath) ?? "";

                AppState.LastFolderPath = folderPath;
                Reset();
                await ParseSpoiler(selectedFilePath);
                DataSaver.setSpoilerPath(selectedFilePath);
            }
        }

        public void OnCollectibleTextChanged()
        {
        }

        public void UpdateProgHintImage(ItemType itemType)
        {
            Dictionary<ItemType, string> itemTypeToResourceString = new()
            {
                {ItemType.GOLDEN_BANANA, "golden_banana" },
                {ItemType.TOTAL_BLUEPRINTS, "total_blueprints" },
                {ItemType.KEY, "basic_key" },
                {ItemType.BANANA_MEDAL, "medal" },
                {ItemType.BATTLE_CROWN, "crown" },
                {ItemType.FAIRY,"fairy" },
                {ItemType.RAINBOW_COIN, "rainbow_coin" },
                {ItemType.PEARL, "pearl" },
                {ItemType.COLORED_BANANA, "colored_bananas" }
            };
            ItemsToNextHintImage.Source = (BitmapImage)FindResource(itemTypeToResourceString[itemType]);
        }

        public void UpdateUIAmountToNextHint(int newAmount)
        {
            ItemsToNextHint.Text = newAmount.ToString();
        }

        public void Reset()
        {
            if (BroadcastView != null)
            {
                BroadcastView.Reset();
            }

            TotalGBs = 0;
            SpoilerLoaded = false;
            ITEM_NAME_TO_REGION.Clear();
            SpoilerSettings = new SpoilerSettings();
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                region.Reset();
            }
            foreach (var item in DraggableItems.Cast<Item>())
            {
                item.CanLeftClick = true;
                item.SetStarVisibility(Visibility.Hidden);
                item.ChangeOpacity(1.0);
                item.InitHoverPoints();
            }
            foreach (var hintPanel in HintPanels)
            {
                hintPanel.Reset();
            }
            BLockerHints.Reset();
            HelmDoorHints.Reset();

            foreach (var progressiveImage in BossKongs)
            {
                progressiveImage.Reset();
            }

            foreach (var progressiveImage in HelmKongs)
            {
                progressiveImage.Reset();
            }

            UpdateUIAmountToNextHint(0);
            HintHelper.GenerateThresholds(AppState.ProgressiveHintCap);
            Autotracker.Reset();
            DataSaver.Reset();
        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                RestoreDirectory = false,
                InitialDirectory = AppContext.BaseDirectory,
                Title = "Save Data",
                Filter = "JSON Files (*.json)|*.json"
            };
            if (saveDialog.ShowDialog() == true)
            {
                var filePath = saveDialog.FileName;
                DataSaver.Save(filePath);
            }
        }


        private void OnLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = false,
                InitialDirectory = AppContext.BaseDirectory,
                Title = "Load Data",
                Filter = "JSON Files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                Reset();
                DataSaver.ReadSavedDataFromFile(filePath);
            }
        }
        private List<int> GetProgressiveIndices(List<ProgressiveItem> items)
        {
            return items.Select(i => i.GetIndex()).ToList();
        }
        public List<int> GetBossKongs()
        {
            return GetProgressiveIndices(BossKongs);
        }
        private void LoadProgressiveIndices(List<int> indices, List<ProgressiveItem> modify)
        {
            for (int i = 0; i < modify.Count; ++i)
            {
                modify[i].SetIndex(indices[i]);
            }
        }
        public void LoadBossKongs(List<int> indices)
        {
            LoadProgressiveIndices(indices, BossKongs);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(1300, 600);
            AutoUpdater.Icon = Properties.Resources.app.ToBitmap();

            AutoUpdater.InstalledVersion = new Version("2.2.3");

            AutoUpdater.Start("https://raw.githubusercontent.com/Brian0255/Track-O-Matic/master/TrackOMatic/AutoUpdateInfo.xml");
            if (AppState.DesiredHeight == 0 || AppState.DesiredWidth == 0)
            {
                return;
            }

            Width = AppState.DesiredWidth;
            Height = AppState.DesiredHeight;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            AppState.DesiredWidth = Width;
            AppState.DesiredHeight = Height;
            DataSaver.Save();
            Autotracker.Shutdown();
            if (BroadcastView != null)
            {
                BroadcastView.Close();
            }
        }

        public void SetShopkeepers(bool on)
        {
            var currentlyOn = ShopkeeperColumn.Width.Value > 0;
            if (currentlyOn == on)
            {
                return;
            }

            var separatorWidth = on ? 1.0 : 1.25;
            var shopkeeperColumnWidth = on ? 1.0 : 0;
            ItemsSeparator.Width = new GridLength(separatorWidth, GridUnitType.Star);
            ShopkeeperColumn.Width = new GridLength(shopkeeperColumnWidth, GridUnitType.Star);
        }

        private void BroadcastClosed(object? sender, EventArgs e)
        {
            BroadcastOption.IsChecked = false;
            BroadcastView = null;
        }
    }
}
