using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TrackOMatic
{
    public partial class HitListItem : UserControl
    {
        private int currentIndex;
        private List<string> backgrounds = new() { "RegionImageBG", "HitListAvailable", "HitListDone" };
        public HitListItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Reset()
        {
            image.Source = new BitmapImage(new Uri("Images/dk64/question_mark.png", UriKind.Relative));
            currentIndex = 0;
            ReadCurrentIndex();
        }
        public void SetImage(BitmapImage newImage)
        {
            image.Source = newImage;
        }

        private void ReadCurrentIndex()
        {
            var resourceName = backgrounds[currentIndex];
            InsideBorder.SetResourceReference(Border.BackgroundProperty, resourceName);
        }

        private void IncreaseIndex()
        {
            currentIndex = (currentIndex + 1) % backgrounds.Count;
            ReadCurrentIndex();
        }

        private void DecreaseIndex()
        {
            currentIndex = (currentIndex + backgrounds.Count - 1) % backgrounds.Count;
            ReadCurrentIndex();
        }

        private void HitListItem_LeftPress(object sender, RoutedEventArgs e)
        {
            IncreaseIndex();
        }

        private void HitListItem_RightPress(object sender, RoutedEventArgs e)
        {
            DecreaseIndex();
        }

        private void HitListItem_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                IncreaseIndex();
            }
            else if(e.Delta < 0)
            {
                DecreaseIndex();
            }
            e.Handled = true;
        }
    }
}
