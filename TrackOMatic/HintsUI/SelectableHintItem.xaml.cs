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
    public partial class SelectableHintItem : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty HintItemImageProperty =
        DependencyProperty.Register("HintItemImage", typeof(Image), typeof(SelectableHintItem));
        private ItemName ItemName { get; set; }
        private ItemBrightnessChanger ItemBrightnessChanger { get; set; }

        private void HintItem_OnLoaded(object sender, RoutedEventArgs e)
        {
            ItemName = (ItemName)Tag;
            ItemBrightnessChanger = new ItemBrightnessChanger(HintItemImage, ItemName);
            UpdateImageFromState();
        }

        public bool On { get; private set; } = false;

        public Image HintItemImage
        {
            get { return (Image)GetValue(HintItemImageProperty); }
            set { SetValue(HintItemImageProperty, value); }
        }

        public SelectableHintItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void UpdateImageFromState()
        {
            if (ItemBrightnessChanger == null) return;
            if (On) ItemBrightnessChanger.Brighten();
            else ItemBrightnessChanger.Darken();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            On = false;
            UpdateImageFromState();
        }

        public void TurnOn()
        {
            On = true;
            UpdateImageFromState();
        }

        public void ToggleState()
        {
            On = !On;
            UpdateImageFromState();
        }

        public void SelectableHintItem_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) ToggleState();
        }

        public void SelectableHintItem_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0) ToggleState();
        }
    }
}