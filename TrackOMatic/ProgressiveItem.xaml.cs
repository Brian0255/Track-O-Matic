using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private int currentIndex;
        private List<BitmapImage> images;

        public List<List<BitmapImage>> ImageSources
        {
            get { return (List<List<BitmapImage>>)GetValue(ImageSourcesProperty); }
            set 
            {
                SetValue(ImageSourcesProperty, value);
                if(value.Count > 0 && value[0].Count > 0){
                    SetImage(value[0][0]);
                }
                images = value.SelectMany(list => list).ToList();
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
            SetIndex(0);
        }

        public void SetIndex(int index)
        {
            currentIndex = index;
            ReadCurrentIndex();
        }

        public void SetImage(BitmapImage newImage)
        {
            image.Source = newImage;
        }

        private void ReadCurrentIndex()
        {
            image.Source = images[currentIndex];
        }

        private void ImageButton_LeftPress(object sender, RoutedEventArgs e)
        {
            if (!Enabled || ImageSources.Count == 0) return;
            var itemSelector = new BasicItemSelector(ImageSources);
            var mousePosition = Mouse.GetPosition(this);
            mousePosition = PointToScreen(mousePosition);
            UIUtils.MoveWindowAndEnsureVisibile(itemSelector, mousePosition.X - itemSelector.Width / 2, mousePosition.Y - itemSelector.Height);
            itemSelector.ShowDialog();
            var index = itemSelector.SelectedImageIndex;
            if (index == -1) return;
            currentIndex = index;
            ReadCurrentIndex();
        }

        private void ImageButton_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Enabled) return;
            if(e.Delta > 0)
            {
                currentIndex = (currentIndex + 1) % images.Count;
            }
            else if(e.Delta < 0)
            {
                currentIndex = (currentIndex + images.Count - 1) % images.Count;
            }
            ReadCurrentIndex();
        }
    }
}
