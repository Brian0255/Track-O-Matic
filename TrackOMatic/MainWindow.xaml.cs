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
        private static readonly TimeSpan _saveInterval = TimeSpan.FromSeconds(30);

        private BroadcastView? BroadcastView { get; set; }
        public Dictionary<Item, ItemBackground> ITEM_TO_BACKGROUND_IMAGE { get; } = new();

        public bool SpoilerLoaded { get; private set; }
        public static Grid Items { get; private set; } = null!;
        public SpoilerParser SpoilerParser { get; private set; }
        public SpoilerSettings SpoilerSettings { get; private set; } = null!;

        public Dictionary<ItemName, PathOrFoundItem> ITEM_TO_DIRECT_HINT { get; } = new();

        private IAutotrackerService Autotracker { get; init; }
        private IUserSettingsService UserSettings { get; init; }
        private IApplicationStateService AppState { get; init; }
        private ISpoilerService SpoilerService { get; init; }
        private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }
        private IDataSaverService DataSaverService { get; init; }

        // Timer to save the data every minute.
        private Timer _saveTimer { get; init; }

        public MainWindow(
            IUserSettingsService settingsSergice,
            IApplicationStateService appStateService,
            ISpoilerService spoilerService,
            IParsedSpoilerDataService parsedSpoilerDataService,
            ILevelOrderService levelOrderService,
            IAutotrackerService autotrackerService,
            IThemeService themeService,
            ISavedProgressProvider savedProgressProvider,
            IDataSaverService dataSaverService
        )
        {
            InitializeComponent();
            UserSettings = settingsSergice;
            AppState = appStateService;
            DataSaverService = dataSaverService;
            SpoilerService = spoilerService;
            ParsedSpoilerDataService = parsedSpoilerDataService;
            Autotracker = autotrackerService;
            DataContext = new MainWindowViewModel(settingsSergice, themeService, autotrackerService, savedProgressProvider);

            _saveTimer = new Timer(_saveInterval);
            _saveTimer.Elapsed += async (sender, e) => await OnTimerSave(sender, e);
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
                ResetWidthHeight();
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

        private async Task OnTimerSave(object? sender, ElapsedEventArgs e)
        {
            try
            {
                await DataSaverService.SaveAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auto-save failed: {ex.Message}");
            }
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
                newWidth = UserSettings.CompactMode ? 1392.0 : 1800.0;
            }
            Width = newWidth;
            Height = 820;
        }

        private void OnResetSize(object sender, RoutedEventArgs e)
        {
            ResetWidthHeight();
        }

        private void OnMouseDown(object sender, RoutedEventArgs e)
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
                    try
                    {
                        await OpenSpoilerFromPath(files[0]);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to open spoiler: {ex.Message}");
                    }
                }
            }
        }

        private async void OnOpenSpoiler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            string lastFolderPath = AppState.LastFolderPath;

            if (!string.IsNullOrEmpty(lastFolderPath))
            {
                openFileDialog.InitialDirectory = lastFolderPath;
            }

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                string selectedFilePath = openFileDialog.FileName;
                string folderPath = Path.GetDirectoryName(selectedFilePath) ?? "";

                AppState.LastFolderPath = folderPath;
                await OpenSpoilerFromPath(selectedFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open spoiler: {ex.Message}");
            }
        }

        private async Task OpenSpoilerFromPath(string filePath)
        {
            await DataSaverService.InitWithSpoilerAsync(filePath);
        }

        public void Reset()
        {
            SpoilerLoaded = false;
            SpoilerSettings = new SpoilerSettings();

            HintHelper.GenerateThresholds(AppState.ProgressiveHintCap);
            Autotracker.Reset();
            DataSaverService.Reset();
        }
        private void OnResetTracker(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private async void OnSave(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                RestoreDirectory = false,
                InitialDirectory = AppContext.BaseDirectory,
                Title = "Save Data",
                Filter = "JSON Files (*.json)|*.json"
            };
            if (saveDialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                var filePath = saveDialog.FileName;
                await DataSaverService.SaveAsync(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save data: {ex.Message}");
            }
        }


        private async void OnLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = false,
                InitialDirectory = AppContext.BaseDirectory,
                Title = "Load Data",
                Filter = "JSON Files (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                var filePath = openFileDialog.FileName;
                Reset();
                await DataSaverService.LoadAsync(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load data: {ex.Message}");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext;

            AutoUpdater.UpdateFormSize = new System.Drawing.Size(1300, 600);
            AutoUpdater.Icon = Properties.Resources.app.ToBitmap();

            AutoUpdater.InstalledVersion = new Version("3.0.0");

            AutoUpdater.Start("https://raw.githubusercontent.com/Brian0255/Track-O-Matic/master/TrackOMatic/AutoUpdateInfo.xml");

            // Only restore saved dimensions if they make sense for current mode
            // This prevents flicker when CompactMode changes between sessions
            if (AppState.DesiredHeight == 0 || AppState.DesiredWidth == 0)
            {
                return;
            }

            double expectedWidth = viewModel.CurrentWindowWidth;

            // Only restore saved width if it matches the expected width for current mode
            // If mode changed since last save, use the correct width for current mode
            if (Math.Abs(AppState.DesiredWidth - expectedWidth) < 1)
            {
                Width = AppState.DesiredWidth;
                Height = AppState.DesiredHeight;
            }
            // else: Keep current width from InitOptions, don't restore
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _saveTimer.Stop();
            _saveTimer.Dispose();

            try
            {
                AppState.DesiredWidth = Width;
                AppState.DesiredHeight = Height;
                Autotracker.Shutdown();
                BroadcastView?.Close();

                // Fire and forget the async save - it will complete in the background
                _ = DataSaverService.SaveAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save on exit: {ex.Message}");
            }
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
