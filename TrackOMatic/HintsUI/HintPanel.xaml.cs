using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TrackOMatic.Properties;

namespace TrackOMatic
{
    public partial class HintPanel : UserControl
    {

        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(HintPanel));
        public static readonly DependencyProperty LineColorProperty = DependencyProperty.Register("LineColor", typeof(Color), typeof(HintPanel));
        public static readonly DependencyProperty HintTypeProperty = DependencyProperty.Register("HintType", typeof(HintType), typeof(HintPanel));
        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.Register("RegionName", typeof(RegionName), typeof(HintPanel));


        public List<ItemName> ItemsToFilterBy { get; private set; } = new();

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public HintType HintType
        {
            get { return (HintType)GetValue(HintTypeProperty); }
            set { SetValue(HintTypeProperty, value); }
        }

        public RegionName RegionName
        {
            get { return (RegionName)GetValue(RegionNameProperty); }
            set { SetValue(RegionNameProperty, value); }
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            var hintSettings = HintTypeSettingsList.SETTINGS[HintType];
            SortButton.Visibility = hintSettings.HintSorterVisibility ? Visibility.Visible : Visibility.Collapsed;
            FilterButton.Visibility = hintSettings.HintSorterVisibility ? Visibility.Visible : Visibility.Collapsed;
        }
        public HintPanel()
        {
            Console.WriteLine(HintType);
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
        }

        public void Reset()
        {
            foreach (var child in HintList.Children.OfType<HintInfo>().ToList())
            {
                HintList.Children.Remove(child);
            }
            ItemsToFilterBy = new();
            UpdateFilterImage();
        }

        private void RemoveHint(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                HintInfo hint = (HintInfo)sender;
                hint.OnRemove();
                HintList.Children.Remove(hint);
            }
        }

        private HintInfo CreateNewHint(bool isSavedHint = false)
        {
            var newHint = new HintInfo(HintType, Name, isSavedHint, RegionName);
            var insertionPoint = HintList.Children.Count - 1;
            HintList.Children.Insert(insertionPoint, newHint);
            newHint.MouseDown += RemoveHint;
            newHint.ItemsOnPath.OnItemsSelected += OnPathItemsSelected;
            return newHint;
        }

        public void OnAddHint(object sender, MouseEventArgs e)
        {
            CreateNewHint();
        }

        public void AddSavedHint(SavedHint savedHint)
        {
            var newHint = CreateNewHint(true);
            newHint.SetUpFromSavedHint(savedHint);
        }

        public List<SavedHint> GetSavedHints()
        {
            List<SavedHint> hints = new();
            foreach (var hint in HintList.Children)
            {
                if (hint is HintInfo hintInfo)
                {
                    hints.Add(hintInfo.SavedHint);
                }
            }
            return hints;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private List<HintInfo> FetchCurrentHintList()
        {
            return HintList.Children.OfType<HintInfo>().ToList();
        }

        private void BuildHintPanelFromHintList(List<HintInfo> hints)
        {
            var addNewHint = HintList.Children[HintList.Children.Count - 1];
            HintList.Children.Clear();
            foreach (var hint in hints)
            {
                HintList.Children.Add(hint);
            }
            HintList.Children.Add(addNewHint);
        }

        private void UpdateFilterImage()
        {
            string resource = ItemsToFilterBy.Count > 0 ? "filter_on" : "filter_empty";
            FilterButton.Source = (ImageSource)FindResource(resource);
        }

        private void ApplyFilter()
        {
            var filteredHints = new List<HintInfo>();
            var rejectedHints = new List<HintInfo>();
            foreach (var child in HintList.Children)
            {
                if (child is HintInfo hintInfo)
                {
                    var selectedItems = hintInfo.ItemsOnPath.SelectedItems;
                    if (ItemsToFilterBy.All(filterItem => selectedItems.ContainsKey(filterItem)))
                    {
                        filteredHints.Add(hintInfo);
                    }
                    else
                    {
                        rejectedHints.Add(hintInfo);
                    }
                }
            }
            var combined = filteredHints.Concat(rejectedHints).ToList();
            BuildHintPanelFromHintList(combined);
            UpdateFilterImage();
        }

        private void Sort()
        {
            var hints = FetchCurrentHintList();
            hints.Sort((hint1, hint2) => hint2.ItemsOnPath.SelectedItems.Count - hint1.ItemsOnPath.SelectedItems.Count);
            BuildHintPanelFromHintList(hints);
        }

        private void OnPathItemsSelected()
        {
            if (Settings.Default.AutoSortPathHints)
            {
                Sort();
            }

            ApplyFilter();
        }

        private void FilterButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new HintItemSelectionDialog(ItemsToFilterBy.ToList())
            {
                Title = "Select Items to Prioritize"
            };
            var mousePosition = Mouse.GetPosition(this);
            mousePosition = PointToScreen(mousePosition);
            UIUtils.MoveWindowAndEnsureVisible(dialog, mousePosition.X - 20, mousePosition.Y - 10);
            dialog.ShowDialog();
            ItemsToFilterBy = dialog.SelectedItems.Keys.ToList();
            ApplyFilter();
        }

        public void SortButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Sort();
            ApplyFilter();
        }
    }
}
