using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using AutoUpdaterDotNET;

using Microsoft.Win32;

using TrackOMatic.HintsUI;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

using Timer = System.Timers.Timer;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BroadcastView? BroadcastView { get; set; }
        public Dictionary<Item, ItemBackground> ITEM_TO_BACKGROUND_IMAGE { get; } = new();

        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; } = null!;
        public SpoilerParser SpoilerParser { get; private set; }
        public DataSaver DataSaver { get; private set; }
        public SpoilerSettings SpoilerSettings { get; private set; } = null!;

        public Dictionary<ItemName, PathOrFoundItem> ITEM_TO_DIRECT_HINT { get; } = new();

        private IAutotrackerService Autotracker { get; init; }
        private IUserSettingsService UserSettings { get; init; }
        private IApplicationStateService AppState { get; init; }
        private IDataPersistenceService DataPersistenceService { get; init; }
        private ISpoilerService SpoilerService { get; init; }
        private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

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
            IThemeService themeService,
            ISavedProgressProvider savedProgressProvider
        )
        {
            InitializeComponent();
            UserSettings = settingsSergice;
            AppState = appStateService;
            DataPersistenceService = dataPersistenceService;
            SpoilerService = spoilerService;
            ParsedSpoilerDataService = parsedSpoilerDataService;
            Autotracker = autotrackerService;
            DataContext = new MainWindowViewModel(settingsSergice, themeService, autotrackerService, savedProgressProvider);

            _saveTimer = new Timer(60000);
            _saveTimer.Elapsed += OnTimerSave;
            _saveTimer.Start();

            // Subscribe to settings changes to notify XAML bindings
            UserSettings.PropertyChanged += OnSettingChanged;

            if (UserSettings.Autotracking)
            {
                Autotracker.Start();
            }

            HintData.Init();
            InitOptions();
            InitData();

            SpoilerParser = new(UserSettings, SpoilerService);
            DataSaver = new(this, DataPersistenceService, ParsedSpoilerDataService, SpoilerService);
            Reset();
        }

        private void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UserSettings.HintDisplay))
            {
                UpdateUiViaHintDisplay();
            }
            else if (e.PropertyName == nameof(UserSettings.CompactMode))
            {
                AdjustBasedOnCompactMode();
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
        }

        /// <summary>
        /// Updates the UI based on the current HintDisplay setting.
        /// </summary>
        /// <remarks>This method will be refactored eventually to not have MainWindow doing everything.</remarks>
        private void UpdateUiViaHintDisplay()
        {
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

            return false; // Will come back.
            Item? item;
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
        }

        private void AdjustBasedOnCompactMode()
        {
            var on = UserSettings.CompactMode;
            var totalColumns = on ? 2 : 3;

            var newWidth = on ? (Width * (1392.0 / 1800.0)) : (Width * (1800.0 / 1392.0));
            Width = newWidth;
        }

        private void ResetSize(object sender, RoutedEventArgs e)
        {
            ResetWidthHeight();
        }

        private void rootGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
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
                    await OpenSpoilerFromPath(files[0]);
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
                await OpenSpoilerFromPath(selectedFilePath);
            }
        }

        private async Task OpenSpoilerFromPath(string filePath)
        {
            if (File.Exists(filePath))
            {
                Reset();
                await ParseSpoiler(filePath);
                DataSaver.SetSpoilerPath(filePath);
            }
        }

        public void Reset()
        {
            SpoilerLoaded = false;
            SpoilerSettings = new SpoilerSettings();

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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(1300, 600);
            AutoUpdater.Icon = Properties.Resources.app.ToBitmap();

            AutoUpdater.InstalledVersion = new Version("3.0.0");

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

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            UserSettings.PropertyChanged -= OnSettingChanged;
        }
    }
}
