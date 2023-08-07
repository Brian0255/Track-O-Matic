using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TrackOMatic
{
    public partial class ToggleableItem : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty ImageSourceProperty =
       DependencyProperty.Register("ImageSource", typeof(string), typeof(ToggleableItem), new PropertyMetadata(null));

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private string onImageSource;
        private string offImageSource;

        public string OnImageSource
        {
            get => onImageSource;
            set
            {
                onImageSource = value;
                UpdateImage();
            }
        }

        public string OffImageSource
        {
            get => offImageSource; set
            {
                offImageSource = value;
                UpdateImage();
            }
        }

        public bool On;

        public event PropertyChangedEventHandler PropertyChanged;

        public ToggleableItem()
        {
            InitializeComponent();
            DataContext = this;
            On = false;
            UpdateImage();
        }

        private void UpdateImage()
        {
            ImageSource = (On) ? onImageSource : offImageSource;
            Opacity = (On) ? 1.00 : 0.25;
            NotifyPropertyChanged();
        }

        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                On = !On;
                UpdateImage();
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
