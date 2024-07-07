using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrackOMatic
{
    public partial class ProgressiveItem : UserControl
    {
        public static readonly DependencyProperty ImageSourcesProperty =
            DependencyProperty.Register("ImageSources", typeof(List<List<BitmapImage>>), typeof(ProgressiveItem), new PropertyMetadata(new List<List<BitmapImage>>()));

        public bool Enabled { get; set; } = true;

        public List<List<BitmapImage>> ImageSources
        {
            get { return (List<List<BitmapImage>>)GetValue(ImageSourcesProperty); }
            set 
            {
                SetValue(ImageSourcesProperty, value);
                if(value.Count > 0 && value[0].Count > 0){
                    SetImage(value[0][0]);
                }
            }
        }

        public ProgressiveItem()
        {
            InitializeComponent();
            DataContext = this;
            ImageSources = new List<List<BitmapImage>>();
        }


        public void Reset()
        {
            Visibility = Visibility.Visible;
            Enabled = true;
        }

        public void SetImage(BitmapImage newImage)
        {
            image.Source = newImage;
        }

        private void ImageButton_LeftPress(object sender, RoutedEventArgs e)
        {
            if (!Enabled && ImageSources.Count == 0) return;
            var itemSelector = new BasicItemSelector(ImageSources);
            var mousePosition = Mouse.GetPosition(this);
            mousePosition = PointToScreen(mousePosition);
            UIUtils.MoveWindowAndEnsureVisibile(itemSelector, mousePosition.X - itemSelector.Width / 2, mousePosition.Y - itemSelector.Height);
            itemSelector.ShowDialog();
            var source = itemSelector.SelectedImageSource;
            if (source == null) return;
            image.Source = itemSelector.SelectedImageSource;
        }
    }
}
