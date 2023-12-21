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

namespace TrackOMatic
{
    public class ItemBrightnessChanger 
    {
        private Image ItemImage;
        private ItemName ItemName;
        public ItemBrightnessChanger(Image itemImage, ItemName itemName)
        {
            ItemImage = itemImage;
            ItemName = itemName;
        }
        public void Brighten()
        {
            var resourceName = ItemName.ToString().ToLower();
            var image = (Image)Application.Current.FindResource(resourceName);
            ItemImage.Source = image.Source;
        }

        public void Darken()
        {
            var resourceName = ItemName.ToString().ToLower() + "_bw";
            var image = (Image)Application.Current.FindResource(resourceName);
            ItemImage.Source = image.Source;
        }
    }
}
