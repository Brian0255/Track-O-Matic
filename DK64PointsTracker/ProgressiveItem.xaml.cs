using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            ImageSources = new List<BitmapImage>();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SetInitialSource();
        }

        private void SetInitialSource()
        {
            if (ImageSources != null && ImageSources.Count > 0)
            {
                SetSource(currentIndex);
            }
        }

        public void Reset()
        {
            currentIndex = 0;
            SetSource(currentIndex);
        }

        private void SetSource(int newIndex)
        {
            image.Source = ImageSources[newIndex];
        }

        public void SetImage(BitmapImage newImage)
        {
            image.Source = newImage;
        }

        private void ImageButton_LeftPress(object sender, RoutedEventArgs e)
        {
            if (ImageSources != null && ImageSources.Count > 0)
            {
                currentIndex++;
                if (currentIndex >= ImageSources.Count)
                    currentIndex = 0;

                SetSource(currentIndex);
            }
        }

        private void ImageButton_RightPress(object sender, RoutedEventArgs e)
        {
            if (ImageSources != null && ImageSources.Count > 0)
            {
                currentIndex--;
                if (currentIndex == -1)
                    currentIndex = ImageSources.Count - 1;

                SetSource(currentIndex);
            }
        }

        private void ImageButton_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ImageButton_LeftPress(sender, e);
            }
            else if (e.Delta < 0)
            {
                ImageButton_RightPress(sender, e);
            }

            e.Handled = true;
        }
    }
}
