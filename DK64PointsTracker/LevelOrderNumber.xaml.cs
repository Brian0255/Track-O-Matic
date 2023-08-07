using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DK64PointsTracker
{
    public partial class LevelOrderNumber : UserControl
    {

        private int currentNumber = 0;

        public LevelOrderNumber()
        {
            InitializeComponent();
            DataContext = this;
            currentNumber = 0;
        }

        private void UpdateLabel()
        {
            NumberLabel.Text = (currentNumber != 0) ? currentNumber.ToString() : "?";
        }

        public void Reset()
        {
            currentNumber = 0;
            UpdateLabel();
        }

        private void LevelOrder_LeftPress(object sender, RoutedEventArgs e)
        {
            currentNumber = (currentNumber + 1) % 8;
            UpdateLabel();
        }

        private void LevelOrder_RightPress(object sender, RoutedEventArgs e)
        {
            currentNumber = (currentNumber + 7) % 8;
            UpdateLabel();
        }

        private void LevelOrder_MouseWheel(object sender, MouseWheelEventArgs e)
        {
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
