using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Data;
using DK64PointsTracker.Properties;

namespace DK64PointsTracker
{
    public partial class MainWindow : Window
    {
        private void HandleItemToggle(bool toggle, Item button, bool init)
        {
        }

        private void HandleRegionToggle(bool toggle, Button button, UniformGrid grid)
        {
            /*
            if (toggle && button.IsEnabled == false)
            {
                var outerGrid = (((button.Parent as Grid).Parent as Grid).Parent as Grid);
                int row = (int)((button.Parent as Grid).Parent as Grid).GetValue(Grid.RowProperty);
                outerGrid.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);
                button.IsEnabled = true;
                button.Visibility = Visibility.Visible;
            }
            else if (toggle == false && button.IsEnabled == true)
            {
                if (data.selected == button)
                {
                    data.RegionsData[button.Name].selectedBar.Source = new BitmapImage(new Uri("Images\\VerticalBarWhite.png", UriKind.Relative));
                    data.selected = null;
                }

                for (int i = grid.Children.Count - 1; i >= 0; --i)
                {
                    Item item = grid.Children[i] as Item;
                    item.HandleItemReturn();
                }

                var outerGrid = (((button.Parent as Grid).Parent as Grid).Parent as Grid);
                int row = (int)((button.Parent as Grid).Parent as Grid).GetValue(Grid.RowProperty);
                outerGrid.RowDefinitions[row].Height = new GridLength(0, GridUnitType.Star);
                button.IsEnabled = false;
                button.Visibility = Visibility.Collapsed;
            }
            */
        }

        private void DragDropToggle(object sender, RoutedEventArgs e)
        {
            foreach (var contentControl in ItemGrid.Children)
            {
                if (contentControl is Item item)
                {
                    item.MouseMove -= item.Item_MouseMove;
                    item.MouseMove += item.Item_MouseMove;
                }
            }
            /*
            Properties.Settings.Default.DragDrop = DragAndDropOption.IsChecked;
            data.dragDrop = DragAndDropOption.IsChecked;
            foreach (Item item in data.Items)
            {
                if (itemPools.Contains(item.Parent))
                {
                    if (data.dragDrop == false)
                    {
                        item.MouseDoubleClick -= item.Item_Click;
                        item.MouseMove -= item.Item_MouseMove;

                        item.MouseDown -= item.Item_MouseDown;
                        item.MouseDown += item.Item_MouseDown;
                        item.MouseUp -= item.Item_MouseUp;
                        item.MouseUp += item.Item_MouseUp;
                    }
                    else
                    {
                        item.MouseDoubleClick -= item.Item_Click;
                        item.MouseDoubleClick += item.Item_Click;
                        item.MouseMove -= item.Item_MouseMove;
                        item.MouseMove += item.Item_MouseMove;

                        item.MouseDown -= item.Item_MouseDown;
                        item.MouseUp -= item.Item_MouseUp;
                    }
                }
            }
            */
        }

        private void TopMostToggle(object sender, RoutedEventArgs e)
        {
            Settings.Default.TopMost = TopMostOption.IsChecked;
            Topmost = TopMostOption.IsChecked;
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
                }
            }
            else
            {
                Autotracker.ResetChecks();
            }
            Settings.Default.Save();
        }

        //private void BroadcastStartupToggle(object sender, RoutedEventArgs e)
        //{
        //    Properties.Settings.Default.BroadcastStartup = BroadcastStartupOption.IsChecked;
        //    if (BroadcastStartupOption.IsChecked)
        //        broadcast.Show();
        //}
    }
}
