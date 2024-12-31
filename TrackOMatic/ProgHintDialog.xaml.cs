using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ProgHintDialog : Window
    {
        private string selectedItemType;
        private string hintCap;
        private readonly Dictionary<ItemType, string> ItemTypeToPlural = new()
        {
            {ItemType.GOLDEN_BANANA, "Golden Bananas" },
            {ItemType.TOTAL_BLUEPRINTS, "Blueprints" },
            {ItemType.KEY, "Keys" },
            {ItemType.BANANA_MEDAL, "Medals" },
            {ItemType.BATTLE_CROWN, "Crowns" },
            {ItemType.FAIRY,"Fairies" },
            {ItemType.RAINBOW_COIN,"Rainbow Coins" },
            {ItemType.PEARL,"Pearls" },
            {ItemType.COLORED_BANANA, "Colored Bananas" }
        };
        private readonly Dictionary<string, ItemType> PluralToItemType = new()
        {
            {"Golden Bananas", ItemType.GOLDEN_BANANA },
            {"Blueprints", ItemType.TOTAL_BLUEPRINTS },
            {"Keys" , ItemType.KEY },
            {"Medals",ItemType.BANANA_MEDAL },
            {"Crowns",ItemType.BATTLE_CROWN },
            {"Fairies",ItemType.FAIRY },
            {"Rainbow Coins",ItemType.RAINBOW_COIN },
            {"Pearls", ItemType.PEARL },
            {"Colored Bananas",ItemType.COLORED_BANANA }
        };
        public ProgHintDialog()
        {
            InitializeComponent();
            DataContext = this;
            var itemType = (ItemType)Properties.Settings.Default.ProgressiveHintItem;
            SelectedItemType = ItemTypeToPlural[itemType];
            var itemTypes = new ItemType[] { ItemType.GOLDEN_BANANA, ItemType.TOTAL_BLUEPRINTS, ItemType.KEY, ItemType.BANANA_MEDAL, ItemType.BATTLE_CROWN, ItemType.FAIRY, ItemType.RAINBOW_COIN, ItemType.PEARL, ItemType.COLORED_BANANA };
            var stringTypes = new List<string>();
            foreach(var item in itemTypes)
            {
                stringTypes.Add(ItemTypeToPlural[item]);
            }
            itemDropdown.ItemsSource = stringTypes;
            HintCap = Properties.Settings.Default.ProgressiveHintCap.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ItemType GetActualItemType()
        {
            var actualItem = (ItemType)PluralToItemType[SelectedItemType];
            return actualItem;
        }

        public string SelectedItemType
        {
            get => selectedItemType;
            set
            {
                if (selectedItemType != value)
                {
                    selectedItemType = value;
                    OnPropertyChanged(nameof(SelectedItemType));
                    var actualItem = (ItemType)PluralToItemType[SelectedItemType];
                    Properties.Settings.Default.ProgressiveHintItem = (int)actualItem;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string HintCap
        {
            get => hintCap;
            set
            {
                if (hintCap != value)
                {
                    hintCap = value;
                    OnPropertyChanged(nameof(hintCap));
                    if (int.TryParse(hintCap, out int hintCapInt))
                    {
                        Properties.Settings.Default.ProgressiveHintCap = hintCapInt;
                        Properties.Settings.Default.Save();
                    }
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void HintCap_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            HintHelper.GenerateThresholds();
        }
    }
}
