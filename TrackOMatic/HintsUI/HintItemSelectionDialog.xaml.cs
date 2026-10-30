using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for HintItemSelectionDialog.xaml
    /// </summary>
    public partial class HintItemSelectionDialog : Window
    {
        public Dictionary<ItemName, bool> SelectedItems { get; private set; } = new();
        private Dictionary<ItemName, SelectableHintItem> ITEM_TO_SELECTABLE = new();
        public HintItemSelectionDialog(List<ItemName> itemsToTurnOn)
        {
            InitializeComponent();
            InitializeMap();
            DataContext = this;
            foreach(var item in itemsToTurnOn)
            {
                if (ITEM_TO_SELECTABLE.ContainsKey(item))
                {
                    ITEM_TO_SELECTABLE[item].TurnOn();
                }
            }
            var randoVersion = ((MainWindow)Application.Current.MainWindow).RandomizerVersion;
            AdjustLayoutV6(randoVersion >= 6);
        }

        private void InitializeMap()
        {
            foreach (var child in ItemGrid.Children)
            {
                if (child is SelectableHintItem hintItem)
                {
                    hintItem.MouseDoubleClick += TextBlock_MouseDown;
                    ItemName itemName = (ItemName)hintItem.Tag;
                    ITEM_TO_SELECTABLE[itemName] = hintItem;
                    hintItem.Reset();
                }
            }
        }

        private void ProcessItems()
        {
            SelectedItems = new();
            foreach (var child in ItemGrid.Children)
            {
                if (child is SelectableHintItem hintItem && hintItem.On)
                {
                    SelectedItems.Add((ItemName)hintItem.Tag, false);
                }
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                ProcessItems();
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ProcessItems();
        }

        private void ItemGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessItems();
                Close();
            }
        }

        private void PlaceItem(ItemName itemName, int row, int column, Visibility visibility)
        {
            var item = ITEM_TO_SELECTABLE[itemName];
            Grid.SetRow(item, row);
            Grid.SetColumn(item, column);
            item.Visibility = visibility;
        }

        private void AdjustLayoutV6(bool useNewer)
        {
            Dictionary<ItemName, (int row, int column, Visibility visibility)> v5_mapping = new()
            {
                {ItemName.FUNKY, (2,12, Visibility.Visible) },
                {ItemName.SNIDE, (3,12, Visibility.Visible) },
                {ItemName.DAY, (0,0,Visibility.Hidden) },
                {ItemName.NIGHT, (0,0,Visibility.Hidden) },
                {ItemName.CANNONS, (0,0,Visibility.Hidden) },
            };

            Dictionary<ItemName, (int row, int column, Visibility visibility)> v6_mapping = new()
            {
                {ItemName.FUNKY, (0,13, Visibility.Visible) },
                {ItemName.SNIDE, (1,13, Visibility.Visible) },
                {ItemName.DAY, (2,12,Visibility.Visible) },
                {ItemName.NIGHT, (2,13,Visibility.Visible) },
                {ItemName.CANNONS, (3,12,Visibility.Visible) },
            };

            var mapping = useNewer ? v6_mapping : v5_mapping;
            foreach (var entry in mapping)
            {
                var itemName = entry.Key;
                var placementInfo = entry.Value;
                PlaceItem(itemName, placementInfo.row, placementInfo.column, placementInfo.visibility);
            }
            var v6ColumnWidth = useNewer ? 1.0 : 0;
            v6_Column.Width = new GridLength(v6ColumnWidth, GridUnitType.Star);
        }
    }
}
