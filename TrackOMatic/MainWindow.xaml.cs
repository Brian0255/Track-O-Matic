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
using TrackOMatic.ViewModels;

using Timer = System.Timers.Timer;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BroadcastView? BroadcastView { get; set; }
        public Dictionary<RegionName, Region> Regions { get; private set; } = null!;
        public Dictionary<Item, ItemBackground> ITEM_TO_BACKGROUND_IMAGE { get; } = new();
        public Dictionary<ItemBackground, Item> BACKGROUND_IMAGE_TO_ITEM { get; } = new();
        public Dictionary<ItemName, RegionName> ITEM_NAME_TO_REGION { get; } = new();
        public List<ProgressiveItem> BossKongs { get; private set; }
        public List<ProgressiveItem> HelmKongs { get; private set; }
        public List<HintPanel> HintPanels { get; private set; } = null!;

        public List<Item> DraggableItems { get; private set; } = new();

        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; } = null!;
        public SpoilerParser SpoilerParser { get; private set; }
        public DataSaver DataSaver { get; private set; }
        public SpoilerSettings SpoilerSettings { get; private set; } = null!;

        private List<BitmapImage> ProgressiveKongSource { get; init; }
        private List<BitmapImage> BossSource { get; init; }

        public Dictionary<ItemName, PathOrFoundItem> ITEM_TO_DIRECT_HINT { get; } = new();
        public Dictionary<ItemName, Item> ITEM_NAME_TO_ITEM { get; } = new();

        private IAutotrackerService Autotracker { get; init; }
        private IUserSettingsService UserSettings { get; init; }
        private IApplicationStateService AppState { get; init; }
        private IDataPersistenceService DataPersistenceService { get; init; }
        private ISpoilerService SpoilerService { get; init; }
        private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

        private ILevelOrderService LevelOrderService { get; init; }

        private AppMenuBarViewModel MenuViewModel { get; init; } = null!;

        /// <summary>
        /// Computed property for the Blueprint Total column based on collectible visibility settings.
        /// When ShowTotalBPs is true and ShowCompanyCoins is false, position is column 6.
        /// Otherwise, position is column 4.
        /// </summary>
        public bool ShowsBlueprintsButNotCoins
        {
            get => UserSettings.ShowTotalBPs && !UserSettings.ShowCompanyCoins;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        // Timer to save the data every minute.
        private Timer _saveTimer { get; init; }

        public MainWindow(
            IUserSettingsService settingsSergice,
            IApplicationStateService appStateService,
            IDataPersistenceService dataPersistenceService,
            ISpoilerService spoilerService,
            IParsedSpoilerDataService parsedSpoilerDataService,
            ILevelOrderService levelOrderService,
            IAutotrackerService autotrackerService,
            IThemeService themeService
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
            MenuViewModel = new AppMenuBarViewModel(UserSettings, themeService);

            _saveTimer = new Timer(60000);
            _saveTimer.Elapsed += OnTimerSave;
            _saveTimer.Start();

            // Subscribe to settings changes to notify XAML bindings
            UserSettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserSettings.CompactMode))
                {
                    AdjustBasedOnCompactMode();
                }
                else if (e.PropertyName == nameof(UserSettings.ShowTotalBPs) || e.PropertyName == nameof(UserSettings.ShowCompanyCoins))
                {
                    OnPropertyChanged(nameof(ShowsBlueprintsButNotCoins));
                }
                else if (e.PropertyName == nameof(UserSettings.HintDisplay))
                {
                    UpdateUiViaHintDisplay();
                }
                else if (e.PropertyName == nameof(UserSettings.BroadcastViewActive))
                {
                    if (UserSettings.BroadcastViewActive)
                    {
                        ShowBroadcastWindow();
                    }
                    else
                    {
                        // Defer close to dispatcher to avoid InvalidOperationException
                        // when called during BroadcastView's Closing event
                        Dispatcher.BeginInvoke(() => BroadcastView?.Close());
                    }
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

        /// <summary>
        /// Updates the UI based on the current HintDisplay setting.
        /// </summary>
        /// <remarks>This method will be refactored eventually to not have MainWindow doing everything.</remarks>
        private void UpdateUiViaHintDisplay()
        {
            var mode = UserSettings.HintDisplay;
            bool compactModeOn = UserSettings.CompactMode;
            var newRatio = compactModeOn ? 1.43 : 2.15;
            switch (mode)
            {
                case HintDisplayMode.Off:
                    MultipathGrid.Visibility = Visibility.Hidden;
                    DirectItemHintGrid.Visibility = Visibility.Hidden;
                    HelmPanel.Visibility = Visibility.Hidden;
                    PotionCountsPanel.Visibility = Visibility.Hidden;
                    newRatio = 0;
                    break;
                case HintDisplayMode.MultipathHints:
                    MultipathGrid.Visibility = Visibility.Visible;
                    DirectItemHintGrid.Visibility = Visibility.Hidden;
                    HelmPanel.Visibility = Visibility.Hidden;
                    PotionCountsPanel.Visibility = Visibility.Visible;
                    break;
                case HintDisplayMode.DirectItemHints:
                    MultipathGrid.Visibility = Visibility.Hidden;
                    DirectItemHintGrid.Visibility = Visibility.Visible;
                    HelmPanel.Visibility = Visibility.Visible;
                    PotionCountsPanel.Visibility = Visibility.Hidden;
                    break;
            }
            HintsColumn.Width = new GridLength(newRatio, GridUnitType.Star);
            ResetWidthHeight();
            UpdateHintDisplayToggles();
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

        private void InitData()
        {
            UpdateHintDisplayToggles();
            UpdateBroadcastNumberDisplayToggles();
            Regions = [];
            Items = ItemGrid;
            BossKongs = [];
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
        }

        private void OnTimerSave(object? sender, ElapsedEventArgs e)
        {
            //probably don't need this
            //DataSaver.Save();
        }

        public bool ProcessNewAutotrackedItem(ItemName itemToProcess, RegionName regionName, bool hint = false, bool canAutosave = true)
        {
            if (regionName == RegionName.UNKNOWN)
            {
                return false;
            }

            if (!ITEM_NAME_TO_ITEM.TryGetValue(itemToProcess, out Item? item))
            {
                return false;
            }

            return false; // Will come back.
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

            //should mean that there was no matching vial, item couldn't be placed as a result
            if (item.Parent == ItemGrid)
            {
                return false;
            }

            DataSaver.AddSavedItem(new SavedItem(itemToProcess, regionName, item.Star.Visibility.ToItemVisibility(), true, item.Image.Opacity));
            DataSaver.Save("autosave.json", canAutosave);
            return true;
        }

        private void InitOptions()
        {
            TopMostOption.IsChecked = UserSettings.TopMost;

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
            Dispatcher.BeginInvoke(() => BroadcastView?.Close());
            AppState.DesiredWidth = Width;
            AppState.DesiredHeight = Height;
            DataSaver.Save();
            Autotracker.Shutdown();
        }

        private void ShowBroadcastWindow()
        {
            if (BroadcastView == null)
            {
                BroadcastView = new BroadcastView(UserSettings, ParsedSpoilerDataService);
                // Wire up the Closed event so we can clean up when the window closes
                BroadcastView.Closed += ClosedBroadcastWindow;
            }

            BroadcastView.Show();
            BroadcastView.Activate();
        }

        private void ClosedBroadcastWindow(object? sender, EventArgs e)
        {
            BroadcastView = null;
            UserSettings.BroadcastViewActive = false;
        }
    }
}
