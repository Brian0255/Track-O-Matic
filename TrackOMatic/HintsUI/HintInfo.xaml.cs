using AutoCompleteTextBox.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TrackOMatic
{
    public partial class HintInfo : UserControl, INotifyPropertyChanged
    {
        private bool UserInitialized = false;

        public static readonly DependencyProperty HintTypeSettingsProperty = DependencyProperty.Register("HintTypeSettings", typeof(HintTypeSettings), typeof(HintInfo));
        public HintTypeSettings HintTypeSettings
        {
            get { return (HintTypeSettings)GetValue(HintTypeSettingsProperty); }
            set { SetValue(HintTypeSettingsProperty, value); }
        }

        public HintType HintType { get; private set; }

        private ObservableCollection<string> suggestions;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HintInfo(HintType hintType)
        {
            InitializeComponent();
            DataContext = this;
            Location.Loaded += (sender, e) => Location.Focus();
            HintTypeSettings = HintTypeSettingsList.SETTINGS[hintType];
            OnPropertyChanged(nameof(HintTypeSettings));
            SuggestionBox.ItemsSource = new List<string>(HintData.SortedRegions);
            HintType = hintType;
        }

        private void Location_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SuggestionBox.Items.Count > 0)
                {
                    Location.Text = SuggestionBox.Items[0] as string;
                }
                ProcessSelection();
                Keyboard.ClearFocus();
                if (HintType == HintType.REGION_POTION_COUNT)
                {
                    PotionCount.Focus();
                }
            }
            if (e.Key == Key.Down)
            {
                SuggestionBox.SelectedIndex = 0;
                SuggestionBox.UpdateLayout();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var listBoxItem = (ListBoxItem)SuggestionBox
                    .ItemContainerGenerator
                    .ContainerFromItem(SuggestionBox.SelectedItem);
                    if (listBoxItem != null) { listBoxItem.Focus(); }
                }));
            }
        }

        private void CheckForShortcuts(List<string> matches)
        {
            foreach (var entry in HintData.UserShortcuts["Hint Regions"])
            {
                var shortcut = entry.Key;
                var matchingEnumString = entry.Value;
                if (shortcut.Contains(Location.Text))
                {
                    Enum.TryParse(matchingEnumString, out HintRegion region);
                    var hintRegion = HintData.HINT_REGION_TO_STRING[region];
                    matches.Add(hintRegion);
                }
            }
        }

        private void SetUpSuggestions()
        {
            var matches = new List<string>();
            if(HintTypeSettings.HintSuggestion == HintSuggestion.LOCATION) 
            {
                CheckForShortcuts(matches);
            }
            if (matches.Count > 0)
            {
                matches.Sort();
                SuggestionBox.ItemsSource = matches;
            }
            else
            {
                List<string> sortBy = (HintTypeSettings.HintSuggestion == HintSuggestion.LOCATION) ? HintData.SortedRegions : HintData.SortedMoves;
                SuggestionBox.ItemsSource = sortBy.Where(item => item.ToLower().Contains(Location.Text.ToLower()));
            }
        }

        private void Location_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Location.Text == "") return;
            SetUpSuggestions();
            BottomRow.IsOpen = (SuggestionBox.Items.Count > 0) && HintTypeSettings.HintSuggestion != HintSuggestion.NONE;
        }

        private void ProcessSelection()
        {
            if (SuggestionBox.SelectedItem != null)
            {
                Location.Text = SuggestionBox.SelectedItem as string;
            }
            BottomRow.IsOpen = false;

            if (HintTypeSettings.PathItemsVisible == Visibility.Visible && !UserInitialized)
            {
                ItemsOnPath.OpenItemSelectionDialog();
                UserInitialized = true;
            }
            if (HintTypeSettings.PromptForFoundItem && !UserInitialized)
            {
                RightItems.OpenItemSelectionDialog();
                UserInitialized = true;
            }
            SuggestionBox.UnselectAll();
        }

        private void SuggestionBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && SuggestionBox.SelectedItem != null)
            {
                ProcessSelection();
            }
        }

        private void SuggestionBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SuggestionBox.SelectedItem != null)
            {
                ProcessSelection();
            }
        }

        private void HintInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var position = ItemsOnPath.ItemPanel.PointToScreen(new Point(0, 0));
            double[] selectionDialogPosition = { position.X - 10, position.Y };
            ItemsOnPath.SelectionDialogPosition = selectionDialogPosition;
            RightItems.SelectionDialogPosition = selectionDialogPosition;
        }

        private void PotionCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }
    }
}
