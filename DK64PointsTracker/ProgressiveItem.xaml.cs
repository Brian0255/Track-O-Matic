using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DK64PointsTracker
{
    public partial class ProgressiveItem : UserControl
    {
        public static readonly DependencyProperty ImageSourcesProperty =
            DependencyProperty.Register("ImageSources", typeof(List<BitmapImage>), typeof(ProgressiveItem), new PropertyMetadata(new List<BitmapImage>()));

        private int currentIndex = 0;

        public List<BitmapImage> ImageSources
        {
            get { return (List<BitmapImage>)GetValue(ImageSourcesProperty); }
            set { SetValue(ImageSourcesProperty, value); }
        }

        public ProgressiveItem()
        {
            InitializeComponent();
            DataContext = this;
            if (ImageSources != null && ImageSources.Count > 0)
            {
                image.Source = ImageSources[currentIndex];
            }
        }

        private void ImageButton_LeftPress(object sender, RoutedEventArgs e)
        {
            if (ImageSources != null && ImageSources.Count > 0)
            {
                currentIndex++;
                if (currentIndex >= ImageSources.Count)
                    currentIndex = 0;

                image.Source = ImageSources[currentIndex];
            }
        }

        private void ImageButton_RightPress(object sender, RoutedEventArgs e)
        {
            if (ImageSources != null && ImageSources.Count > 0)
            {
                currentIndex--;
                if (currentIndex == -1)
                    currentIndex = ImageSources.Count - 1;

                image.Source = ImageSources[currentIndex];
            }
        }
    }
}
