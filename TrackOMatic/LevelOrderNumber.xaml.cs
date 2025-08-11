using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TrackOMatic.Properties;

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
            currentNumber = newNumber;
            UpdateLabel();
        }

        public int GetNumber()
        {
            return currentNumber;
        }

        private void LevelOrder_LeftPress(object sender, RoutedEventArgs e)
        {
            if (RegionName == RegionName.HIDEOUT_HELM && currentNumber == 8 && !Settings.Default.HelmInLevelOrder) return;
            int max = (Settings.Default.HelmInLevelOrder) ? 8 : 7;
            currentNumber = (currentNumber + 1) % (max + 1);
            UpdateLabel();
        }

        private void LevelOrder_RightPress(object sender, RoutedEventArgs e)
        {
            if (RegionName == RegionName.HIDEOUT_HELM && currentNumber == 8 && !Settings.Default.HelmInLevelOrder) return;
            int max = (Settings.Default.HelmInLevelOrder) ? 8 : 7;
            currentNumber = (currentNumber + (max)) % (max + 1);
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
