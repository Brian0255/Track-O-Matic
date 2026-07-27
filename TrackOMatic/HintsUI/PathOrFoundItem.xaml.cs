using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TrackOMatic
{
    public partial class PathOrFoundItem : ContentControl
    {
        public bool IsChecked { get; set; }
        public ItemName ItemName { get; private set; }
        private HintItemList HintItemList { get; init; }

        public static readonly DependencyProperty PathItemImageProperty =
        DependencyProperty.Register("PathItemImage", typeof(ImageSource), typeof(PathOrFoundItem));

        public ImageSource PathItemImage
        {
            get { return (ImageSource)GetValue(PathItemImageProperty); }
            set { SetValue(PathItemImageProperty, value); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public PathOrFoundItem(ItemName itemName, HintItemList hintItemList, bool isChecked = false)
        {
            InitializeComponent();
            SetResourceReference(PathItemImageProperty, itemName.ToString().ToLower());
            IsChecked = isChecked;
            ItemName = itemName;
            HintItemList = hintItemList;
            Checkmark.Visibility = IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateCheckmark()
        {
            Checkmark.Visibility = IsChecked ? Visibility.Visible : Visibility.Hidden;
            HintItemList.UpdateCheckmark(ItemName, IsChecked);
        }

        public void Toggle()
        {
            if (ItemName == ItemName.NONE)
            {
                return;
            }

            IsChecked = !IsChecked;
            UpdateCheckmark();
        }

    }
}
