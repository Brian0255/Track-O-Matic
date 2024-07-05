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
    public partial class ItemBackground : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty BackgroundItemImageProperty =
        DependencyProperty.Register("BackgroundItemImage", typeof(Image), typeof(ItemBackground));

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public Image BackgroundItemImage
        {
            get { return (Image)GetValue(BackgroundItemImageProperty); }
            set { SetValue(BackgroundItemImageProperty, value); }
        }

        public ItemBackground()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ChangeOpacity(double newOpacity)
        {
            BackgroundItemImage.Opacity = newOpacity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetStarVisibility(Visibility newVisibility)
        {
            Star.Visibility = newVisibility;
        }

        public void ToggleStar()
        {
            var newVisibility = (Star.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            SetStarVisibility(newVisibility);
            mainWindow.BACKGROUND_IMAGE_TO_ITEM[this].SetStarVisibility(newVisibility);
        }

        public void ItemBackground_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.MiddleButton == MouseButtonState.Pressed) ToggleStar();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = mainWindow.BACKGROUND_IMAGE_TO_ITEM[this];
                if (item.Parent != mainWindow.ItemGrid)
                {
                    item.HandleItemReturn();
                    item.DoDragDrop(e.RightButton == MouseButtonState.Pressed);
                }
            }
        }

        public void ItemBackground_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0) ToggleStar();
        }
    }
}