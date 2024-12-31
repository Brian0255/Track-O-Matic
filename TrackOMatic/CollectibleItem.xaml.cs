﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TrackOMatic.Properties;

namespace TrackOMatic
{
    public partial class CollectibleItem : UserControl, INotifyPropertyChanged
    {
        private int text;

        public static readonly DependencyProperty ImageSourceProperty =
       DependencyProperty.Register("ImageSource", typeof(string), typeof(CollectibleItem), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(int), typeof(CollectibleItem), new PropertyMetadata(null));

        public static readonly DependencyProperty InteractibleProperty =
        DependencyProperty.Register("Interactible", typeof(bool), typeof(CollectibleItem), new PropertyMetadata(true));

        public static readonly DependencyProperty NumberDisplayProperty =
        DependencyProperty.Register("NumberDisplay", typeof(bool), typeof(CollectibleItem), new PropertyMetadata(true));

        public static readonly DependencyProperty BGColorProperty =
        DependencyProperty.Register("BGColor", typeof(Brush), typeof(CollectibleItem));

        public static readonly DependencyProperty RowHeightProperty =
        DependencyProperty.Register("RowHeight", typeof(GridLength), typeof(CollectibleItem), new PropertyMetadata(new GridLength(1.2, GridUnitType.Star)));

        public bool Interactible
        {
            get { return (bool)GetValue(InteractibleProperty); }
            set { SetValue(InteractibleProperty, value); }
        }

        public bool NumberDisplay
        {
            get { return (bool)GetValue(NumberDisplayProperty); }
            set { SetValue(NumberDisplayProperty, value); }
        }

        private string baseSourcePath = "";
        private string BWSourcePath = "";

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public Brush BGColor
        {
            get { return (Brush)GetValue(BGColorProperty); }
            set { SetValue(BGColorProperty, value); }
        }

        public GridLength RowHeight
        {
            get { return (GridLength)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        private void Darken()
        {
            ImageSource = BWSourcePath;
        }

        private void LightUp()
        {
            ImageSource = baseSourcePath;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BWSourcePath = ImageSource;
            //hacky, but remove the .png, add '_bw', readd the .png
            baseSourcePath = BWSourcePath.Replace("bw", "dk64");
        }

        public int Text
        {
            get => text;
            set
            {
                if (text != value)
                {
                    text = value;
                    SetValue(TextProperty, value);
                    if(value == 0)
                    {
                        //NumberGrid.Visibility = Visibility.Hidden;
                        Darken();
                        CountViewbox.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        int columnWidth = (text >= 10) ? 25 : 17;
                        //NumberGridColumn.Width = new GridLength(columnWidth);
                        //NumberGrid.Visibility = Visibility.Visible;
                        LightUp();
                        CountViewbox.Visibility = (NumberDisplay) ? Visibility.Visible : Visibility.Hidden;
                    }
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.OnCollectibleTextChanged();
                }
            }
        }

        private void UpdateBroadcast()
        {
            //kinda janky way to update broadcast correctly on user input
            var window = (MainWindow)Application.Current.MainWindow;
            ItemType collectibleType = 0;
            foreach(var entry in window.Collectibles)
            {
                if(entry.Value == this)
                {
                    collectibleType = entry.Key;
                    break;
                }
            }
            if (window.BroadcastView != null) window.BroadcastView.UpdateCollectible(collectibleType, Text);
        }

        public void SetAmount(int newAmount)
        {
            Text = newAmount;
        }

        public CollectibleItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.Autotracking || !Interactible) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Text++;
                UpdateBroadcast();
            }
        }

        private void Image_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.Autotracking || !Interactible) return;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Text = Math.Max(Text - 1, 0);
                UpdateBroadcast();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
