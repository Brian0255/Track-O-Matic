using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic;
using TrackOMatic.Services;

namespace TrackOMatic
{
    public partial class MainWindow : Window
    {
        private void TopMostToggle(object? sender, RoutedEventArgs? e)
        {
            UserSettings.TopMost = TopMostOption.IsChecked;
        }

        private void AutoSortToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.AutoSortPathHints = SortNewHintsOption.IsChecked;
        }

        private void EnemyAutofillToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.EnemiesInAutofill = EnemiesInAutofillOption.IsChecked;
        }
        private void CompactModeToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.CompactMode = CompactOption.IsChecked;
            AdjustBasedOnCompactMode();
        }

        private void SongDisplayToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.SongDisplay = SongDisplayOption.IsChecked;
        }

        private void BroadcastSongDisplayToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.BroadcastSongDisplay = BroadcastSongDisplay.IsChecked;
        }

        private void BroadcastHelmKRoolToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.BroadcastHelmKRool = BroadcastHelmKRool.IsChecked;
        }

        private void BroadcastShopkeepersToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.BroadcastShopkeepers = BroadcastShopkeepers.IsChecked;
        }

        private void HintDisplayToggle(object sender, RoutedEventArgs e)
        {
            // XAML Contract: This method is wired via the Checked="HintDisplayToggle" attribute
            // on RadioButtons in MainWindow.xaml (lines 64-66). WPF automatically invokes this method
            // when any RadioButton with that Checked handler is selected.
            //
            // RadioButton content (e.g., "Off", "Multipath Hints") maps to HintDisplayMode enum values
            // via the LegacyNameAttribute. The content string MUST exactly match a LegacyName on an enum value,
            // or the conversion will fail silently (defaulting to the first enum value).
            //
            // If you change a RadioButton's content text or rename this method, you must also update:
            // 1. The MainWindow.xaml Checked attribute to match the new method name
            // 2. The HintDisplayMode enum's LegacyNameAttribute to match the new RadioButton content

            var button = sender as RadioButton;
            string? legacyName = button?.Content.ToString();
            if (string.IsNullOrEmpty(legacyName))
            {
                return;
            }

            var mode = legacyName.FromLegacyToEnum<HintDisplayMode>();
            UserSettings.HintDisplay = mode;
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
                    DirectItemHintGrid.Visibility = Visibility.Visible;
                    MultipathGrid.Visibility = Visibility.Hidden;
                    HelmPanel.Visibility = Visibility.Visible;
                    PotionCountsPanel.Visibility = Visibility.Hidden;
                    break;
            }
            HintsColumn.Width = new GridLength(newRatio, GridUnitType.Star);
            ResetWidthHeight();
            UpdateHintDisplayToggles();
        }

        private void BroadcastNumberDisplayToggle(object sender, RoutedEventArgs e)
        {
            // XAML Contract: This method is wired via the Checked="BroadcastNumberDisplayToggle" attribute
            // on RadioButtons in MainWindow.xaml (lines 80-81). WPF automatically invokes this method
            // when any RadioButton with that Checked handler is selected.
            //
            // RadioButton content (e.g., "Points", "WOTH Count") maps to BroadcastNumberLabel enum values
            // via the LegacyNameAttribute. The content string MUST exactly match a LegacyName on an enum value,
            // or the conversion will fail silently (defaulting to the first enum value).
            //
            // If you change a RadioButton's content text or rename this method, you must also update:
            // 1. The MainWindow.xaml Checked attribute to match the new method name
            // 2. The BroadcastNumberLabel enum's LegacyNameAttribute to match the new RadioButton content

            var button = (sender as RadioButton);
            string? legacyName = button?.Content.ToString();
            if (string.IsNullOrEmpty(legacyName))
            {
                return;
            }

            var label = legacyName.FromLegacyToEnum<BroadcastNumberLabel>();
            UserSettings.BroadcastNumberLabel = label;
            BroadcastView?.AdjustLayout();
            UpdateBroadcastNumberDisplayToggles();
        }

        private void AutotrackToggle(object sender, RoutedEventArgs e)
        {
            UserSettings.Autotracking = AutotrackOption.IsChecked;

            if (UserSettings.Autotracking)
            {
                Autotracker.Start();
            }
            else
            {
                Autotracker.Stop();
            }
        }

        private void BroadcastToggle(object sender, RoutedEventArgs e)
        {
            if (BroadcastView != null)
            {
                BroadcastView.Close();
                BroadcastView = null;
                return;
            }
            BroadcastView = new BroadcastView(UserSettings, ParsedSpoilerDataService);
            BroadcastView.Closed += BroadcastClosed;
            BroadcastView.Show();

            BroadcastView.ProcessSpoilerSettings(SpoilerSettings);
            BroadcastOption.IsChecked = true;
        }

        private void TotalBPs_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.ShowTotalBPs = TotalBPs.IsChecked;
        }

        private void CompanyCoins_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.ShowCompanyCoins = CompanyCoins.IsChecked;
        }

        private void KRoolOrder_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.ShowKRoolOrder = KRoolOrder.IsChecked;
        }

        private void HelmOrder_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.ShowHelmOrder = HelmOrder.IsChecked;
        }

        private void HelmInLevelOrder_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.HelmInLevelOrder = HelmInLevelOrder.IsChecked;
            if (!UserSettings.HelmInLevelOrder)
            {
                var levelOrder = LevelOrderService.GetLevelOrder();
                levelOrder[7] = 8;
                LevelOrderService.SetLevelOrder(levelOrder);
            }
        }

        private void ColoredBarrelPadMoves_Click(object sender, RoutedEventArgs e)
        {
            var themeService = ServiceLocator.GetService<IThemeService>();
            themeService.SetBarrelPadTheme(ColorBarrelPadMoves.IsChecked);
        }

        private void HelmDoors_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.HelmDoors = HelmDoors.IsChecked;
        }
    }
}
