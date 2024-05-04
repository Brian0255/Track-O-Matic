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
        public List<ItemName> SelectedItems { get; private set; } = new();
        public HintItemSelectionDialog(List<ItemName> itemsToTurnOn)
        {
            InitializeComponent();
            DataContext = this;
            foreach (var child in ItemGrid.Children)
            {
                if (child is SelectableHintItem hintItem)
                {
                    hintItem.MouseDoubleClick += TextBlock_MouseDown;
                    ItemName itemName = (ItemName)hintItem.Tag;
                    if (itemsToTurnOn.Contains(itemName))
                    {
                        hintItem.TurnOn();
                    }
                    else
                    {
                        hintItem.Reset();
                    }
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
                    SelectedItems.Add((ItemName)hintItem.Tag);
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
    }
}
