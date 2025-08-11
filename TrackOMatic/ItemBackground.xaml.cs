using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace TrackOMatic
{
    public partial class ItemBackground : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty BackgroundItemImageProperty =
        DependencyProperty.Register("BackgroundItemImage", typeof(ImageSource), typeof(ItemBackground));

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public ImageSource BackgroundItemImage
        {
            get { return (ImageSource)GetValue(BackgroundItemImageProperty); }
            set { SetValue(BackgroundItemImageProperty, value); }
        }

        public static readonly DependencyProperty InteractibleProperty =
        DependencyProperty.Register("Interactible", typeof(bool), typeof(ItemBackground), new PropertyMetadata(true));

        public bool Interactible
        {
            get { return (bool)GetValue(InteractibleProperty); }
            set { SetValue(InteractibleProperty, value); }
        }

        public ItemBackground()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void ChangeOpacity(double newOpacity)
        {
            Image.Opacity = newOpacity;
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
            if (!Interactible) return;
            var shiftClicked = (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
            if (e.MiddleButton == MouseButtonState.Pressed) ToggleStar();
            else if (e.LeftButton == MouseButtonState.Pressed && shiftClicked) ToggleStar();
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = mainWindow.BACKGROUND_IMAGE_TO_ITEM[this];
                if (item.Parent != mainWindow.ItemGrid)
                {
                    item.HandleItemReturn();
                    if(item.Region != null && item.Region.RegionName != RegionName.UNHINTABLE_MOVES) item.DoDragDrop(e.RightButton == MouseButtonState.Pressed);
                }
            }
        }

        public void ItemBackground_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Interactible) return;
            if (e.Delta != 0) ToggleStar();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Tag == null) return;
        }
    }
}