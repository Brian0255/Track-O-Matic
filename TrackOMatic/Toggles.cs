using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Data;
using TrackOMatic.Properties;

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

        private void HitListToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.HitList = HitListOption.IsChecked;
            ResetWidthHeight();
            Settings.Default.Save();
        }

        private void AutotrackToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.Autotracking = AutotrackOption.IsChecked;
            if (!AutotrackOption.IsChecked) 
            {
                foreach (var item in DraggableItems.Cast<Item>())
                {
                    item.IsEnabled = true;
                    item.CanLeftClick = true;
                }
                DataSaver.TurnOffAutotrackingField();
                foreach (var entry in Regions) SetRegionLighting(entry.Key, false);
            }
            else
            {
                Autotracker.ResetChecks();
            }
            Settings.Default.Save();
        }
    }
}
