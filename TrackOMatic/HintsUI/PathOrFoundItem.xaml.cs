using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace TrackOMatic
{
    public partial class PathOrFoundItem : ContentControl
    {
        private ItemBrightnessChanger ItemBrightnessChanger;
        public bool IsChecked { get; set; }
        public ItemName ItemName { get; private set; }
        private HintItemList HintItemList;
        public PathOrFoundItem(ItemName itemName, bool isChecked = false, HintItemList hintItemList = null)
        {
            InitializeComponent();
            ItemBrightnessChanger = new ItemBrightnessChanger(Image, itemName);
            ItemBrightnessChanger.Brighten();
            DataContext = this;
            IsChecked = isChecked;
            ItemName = itemName;
            HintItemList = hintItemList;
            Checkmark.Visibility = IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateCheckmark()
        {
            Checkmark.Visibility = IsChecked ? Visibility.Visible : Visibility.Hidden;
            HintItemList.UpdateCheckmark(ItemName, IsChecked);
        }

        public void Toggle()
        {
            if (ItemName == ItemName.NONE) return;
            IsChecked = !IsChecked;
            UpdateCheckmark();
        }


        private void  Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed) Toggle();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0) Toggle();
        }
    }
}