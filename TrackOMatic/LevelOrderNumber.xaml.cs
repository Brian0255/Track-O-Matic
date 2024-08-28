using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrackOMatic
{
    public partial class LevelOrderNumber : UserControl
    {

        private int currentNumber = 0;
        private bool disabled = false;
        public RegionName RegionName { get; private set; }

        public LevelOrderNumber()
        {
            InitializeComponent();
            DataContext = this;
            currentNumber = 0;
        }

        public void UpdateLabel()
        {
            NumberLabel.Text = (currentNumber != 0) ? currentNumber.ToString() : "?";
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.BroadcastView != null && currentNumber != -1)
            {
                mainWindow.BroadcastView.UpdateLevelNumber(RegionName, currentNumber);
            }
        }

        public void SetRegion(RegionName newRegion)
        {
            RegionName = newRegion;
        }

        public void Reset()
        {
            currentNumber = 0;
            UpdateLabel();
        }

        public void SetNumber(int newNumber)
        {
            disabled = (newNumber != -1);
            if (disabled) currentNumber = newNumber;
            UpdateLabel();
        }

        private void LevelOrder_LeftPress(object sender, RoutedEventArgs e)
        {
            if(disabled) return;
            currentNumber = (currentNumber + 1) % 8;
            UpdateLabel();
        }

        private void LevelOrder_RightPress(object sender, RoutedEventArgs e)
        {
            if (disabled) return;
            currentNumber = (currentNumber + 7) % 8;
            UpdateLabel();
        }

        private void LevelOrder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (disabled) return;
            if (e.Delta > 0)
            {
                LevelOrder_LeftPress(sender, e);
            }
            else if (e.Delta < 0)
            {
                LevelOrder_RightPress(sender, e);
            }

            e.Handled = true;
        }
    }
}
