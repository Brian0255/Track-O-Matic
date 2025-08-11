using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Data;
using TrackOMatic.Properties;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;

namespace TrackOMatic
{
    public partial class MainWindow : Window
    {
        private void TopMostToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.TopMost = TopMostOption.IsChecked;
            Topmost = TopMostOption.IsChecked;
            Settings.Default.Save();
        }

        private void AutoSortToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.AutoSortPathHints = SortNewHintsOption.IsChecked;
            Settings.Default.Save();
        }

        private void CompactModeToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.CompactMode = CompactOption.IsChecked;
            Settings.Default.Save();
            AdjustBasedOnCompactMode();
        }

        private void HitListToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.HitList = HitListOption.IsChecked;
            ResetWidthHeight();
            Settings.Default.Save();
        }

        private void SongDisplayToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.SongDisplay = SongDisplayOption.IsChecked;
            Settings.Default.Save();
        }

        private void BroadcastSongDisplayToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.BroadcastSongDisplay = BroadcastSongDisplay.IsChecked;
            Settings.Default.Save();
            if (BroadcastView != null)
            {
                BroadcastView.AdjustWindowSize();
            }
        }

        private void BroadcastHelmKRoolToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.BroadcastHelmKRool = BroadcastHelmKRool.IsChecked;
            Settings.Default.Save();
            if (BroadcastView != null)
            {
                BroadcastView.AdjustWindowSize();
            }
        }

        private void BroadcastShopkeepersToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.BroadcastShopkeepers = BroadcastShopkeepers.IsChecked;
            Settings.Default.Save();
            if (BroadcastView != null)
            {
                BroadcastView.UpdateShopkeeperHeight();
            }
        }

        private void HintDisplayToggle(object sender, RoutedEventArgs e)
        {
            var button = (sender as RadioButton);
            Settings.Default.HintDisplay = button.Content.ToString();
            bool compactModeOn = Settings.Default.CompactMode;
            var newRatio = compactModeOn ? 1.43 : 2.15;
            if (Settings.Default.HintDisplay == "Multipath Hints")
            {
                MultipathGrid.Visibility = Visibility.Visible;
                DirectItemHintGrid.Visibility = Visibility.Hidden;
                HelmPanel.Visibility = Visibility.Hidden;
                PotionCountsPanel.Visibility = Visibility.Visible;
            }
            else if (Settings.Default.HintDisplay == "Direct Item Hints")
            {
                DirectItemHintGrid.Visibility = Visibility.Visible;
                MultipathGrid.Visibility = Visibility.Hidden;
                HelmPanel.Visibility = Visibility.Visible;
                PotionCountsPanel.Visibility = Visibility.Hidden;
            }
            else
            {
                newRatio = 0;
            }
            HintsColumn.Width = new GridLength(newRatio, GridUnitType.Star);
            ResetWidthHeight();
            UpdateHintDisplayToggles();
            Settings.Default.Save();
        }

        private void AutotrackToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.Autotracking = AutotrackOption.IsChecked;
            Settings.Default.Save();
        }

        private void BroadcastToggle(object sender, RoutedEventArgs e)
        {
            if(BroadcastView != null)
            {
                BroadcastView.Close();
                BroadcastView = null;
                return;
            }
            BroadcastView = new BroadcastView();
            BroadcastView.UpdateSongInfo(SongGame.Text, SongName.Text);
            BroadcastView.Closed += BroadcastClosed;
            BroadcastView.Show();
            BroadcastView.InitializeFromItems(ITEM_NAME_TO_ITEM);
            foreach (var entry in Collectibles)
            {
                BroadcastView.UpdateCollectible(entry.Key, entry.Value.Text);
            }
            foreach (var entry in Regions)
            {
                var region = entry.Value;
                if (region.LevelOrderNumber != null) region.LevelOrderNumber.UpdateLabel();
                region.UpdatePoints();
            }
            BroadcastView.ProcessSpoilerSettings(SpoilerSettings);
            BroadcastOption.IsChecked = true;
        }

        private void TotalBPs_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.TotalBPs = TotalBPs.IsChecked;
            FormatCollectibles();
            Settings.Default.Save();
        }

        private void CompanyCoins_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.CompanyCoins = CompanyCoins.IsChecked;
            FormatCollectibles();
            Settings.Default.Save();
        }

        private void KRoolOrder_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.KRoolOrder = KRoolOrder.IsChecked;
            Settings.Default.Save();
        }

        private void HelmOrder_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.HelmOrder = HelmOrder.IsChecked;
            Settings.Default.Save();
        }

        private void HelmInLevelOrder_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.HelmInLevelOrder = HelmInLevelOrder.IsChecked;
            if (!Settings.Default.HelmInLevelOrder)
            {
                Regions[RegionName.HIDEOUT_HELM].SetLevelOrderNumber(8);
            }
            Settings.Default.Save();
        }

        private void ColoredBarrelPadMoves_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ColoredBarrelPadMoves = ColorBarrelPadMoves.IsChecked;
            Settings.Default.Save();
            ((App)Application.Current).UpdatePadBarrelImages();
        }

        private void HelmDoors_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.HelmDoors = HelmDoors.IsChecked;
            Settings.Default.Save();
        }

        private void SetProgHintTypeAmount(object sender, RoutedEventArgs e)
        {
            var dialog = new ProgHintDialog();
            var mousePosition = Mouse.GetPosition(this);
            mousePosition = PointToScreen(mousePosition);
            UIUtils.MoveWindowAndEnsureVisibile(dialog, mousePosition.X - dialog.Width/2, mousePosition.Y-dialog.Height/2);
            dialog.ShowDialog();
            UpdateProgHintImage();
        }
    }
}
