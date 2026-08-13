using System.ComponentModel;
using System.Windows;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for HintItemSelectionDialog.xaml
    /// </summary>
    public partial class BroadcastView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        #region Proxy Properties for User Settings

        public IUserSettingsService UserSettings { get; init; }
        public IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

        /// <summary>
        /// Proxy property for XAML binding to TopMost setting.
        /// </summary>
        public bool TopMostSetting
        {
            get => UserSettings.TopMost;
            set
            {
                if (UserSettings.TopMost != value)
                {
                    UserSettings.TopMost = value;
                    OnPropertyChanged(nameof(TopMostSetting));
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to Helm & K. Rool broadcast setting.
        /// </summary>
        public bool BroadcastHelmKRoolSetting
        {
            get => UserSettings.BroadcastHelmKRool;
            set
            {
                if (UserSettings.BroadcastHelmKRool != value)
                {
                    UserSettings.BroadcastHelmKRool = value;
                    OnPropertyChanged(nameof(BroadcastHelmKRoolSetting));
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to shopkeepers broadcast setting.
        /// </summary>
        public bool BroadcastShopkeepersSetting
        {
            get => UserSettings.BroadcastShopkeepers;
            set
            {
                if (UserSettings.BroadcastShopkeepers != value)
                {
                    UserSettings.BroadcastShopkeepers = value;
                    OnPropertyChanged(nameof(BroadcastShopkeepersSetting));
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to song display broadcast setting.
        /// </summary>
        public bool BroadcastSongDisplaySetting
        {
            get => UserSettings.BroadcastSongDisplay;
            set
            {
                if (UserSettings.BroadcastSongDisplay != value)
                {
                    UserSettings.BroadcastSongDisplay = value;
                    OnPropertyChanged(nameof(BroadcastSongDisplaySetting));
                }
            }
        }

        /// <summary>
        /// Proxy property for XAML binding to the broadcast number label setting.
        /// </summary>
        public BroadcastNumberLabel BroadcastNumberLabelSetting
        {
            get => UserSettings.BroadcastNumberLabel;
            set
            {
                if (UserSettings.BroadcastNumberLabel != value)
                {
                    UserSettings.BroadcastNumberLabel = value;
                    OnPropertyChanged(nameof(BroadcastNumberLabelSetting));
                }
            }
        }

        #endregion

        public BroadcastView(IUserSettingsService userSettings, IParsedSpoilerDataService parsedSpoilerDataService)
        {
            UserSettings = userSettings;
            ParsedSpoilerDataService = parsedSpoilerDataService;
            UserSettings.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IUserSettingsService.TopMost))
                {
                    OnPropertyChanged(nameof(TopMostSetting));
                }
                else if (e.PropertyName == nameof(IUserSettingsService.BroadcastHelmKRool))
                {
                    OnPropertyChanged(nameof(BroadcastHelmKRoolSetting));
                }
                else if (e.PropertyName == nameof(IUserSettingsService.BroadcastShopkeepers))
                {
                    OnPropertyChanged(nameof(BroadcastShopkeepersSetting));
                }
                else if (e.PropertyName == nameof(IUserSettingsService.BroadcastSongDisplay))
                {
                    OnPropertyChanged(nameof(BroadcastSongDisplaySetting));
                }
                else if (e.PropertyName == nameof(IUserSettingsService.BroadcastNumberLabel))
                {
                    OnPropertyChanged(nameof(BroadcastNumberLabelSetting));
                    AdjustLayout();
                }
            };

            InitializeComponent();
        }

        public void Reset()
        {
            foreach (var key in SharedMoves.Keys.ToList())
            {
                SharedMoves[key] = false;
            }
            foreach (var key in StarredSharedMoves.Keys.ToList())
            {
                StarredSharedMoves[key] = false;
            }
            MovesWidth.Width = new GridLength(345, GridUnitType.Pixel);
        }

        public void AdjustLayout()
        {
            var pointsEnabled = ParsedSpoilerDataService.HasItemPoints();
            var hoardEnabled = ParsedSpoilerDataService.HasHoardPoints();
            var bothEnabled = pointsEnabled && hoardEnabled;
            var displayOption = UserSettings.BroadcastNumberLabel;

            var pointsCanDisplay = pointsEnabled && (!bothEnabled || displayOption == BroadcastNumberLabel.Points);
            var hoardCanDisplay = hoardEnabled && (!bothEnabled || displayOption == BroadcastNumberLabel.WothCount);

            MovesWidth.Width = new GridLength(pointsEnabled ? 315 : 345, GridUnitType.Pixel);
        }

        public void ProcessSpoilerSettings(SpoilerSettings settings)
        {
            AdjustLayout();
        }

        private ItemBackground? GetMatchingItem(ItemName item)
        {
            var name = item.ToString();
            if (name.StartsWith("KEY"))
            {
                return (ItemBackground)FindName(item.ToString().ToLower());
            }

            return null;
        }

        public void ActivateTooltip(ItemName item, string hoverText)
        {
            var match = GetMatchingItem(item);
            if (match == null)
            {
                return;
            }

            match.ToolTip.Content = hoverText;
            match.ToolTip.Visibility = Visibility.Visible;
        }

        public void DisableTooltip(ItemName item)
        {
            var match = GetMatchingItem(item);
            if (match == null)
            {
                return;
            }

            match.ToolTip.Visibility = Visibility.Collapsed;
        }
    }
}
