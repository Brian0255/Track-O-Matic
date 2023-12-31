using AutoCompleteTextBox.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private Dictionary<HintSuggestion, HintShortcutInfo> suggestionToShortcutInfo = new()
        {
            {HintSuggestion.LOCATION, new HintShortcutInfo("Hint Regions", HintData.SortedRegions) },
            {HintSuggestion.CHECK, new HintShortcutInfo("Item Locations", HintData.SortedChecks) },
            {HintSuggestion.MOVE, new HintShortcutInfo("Moves", HintData.SortedMoves) }
        };
        private HintShortcutInfo hintShortcutInfo;

        public static readonly DependencyProperty HintTypeSettingsProperty = DependencyProperty.Register("HintTypeSettings", typeof(HintTypeSettings), typeof(HintInfo));
        public HintTypeSettings HintTypeSettings
        {
            get { return (HintTypeSettings)GetValue(HintTypeSettingsProperty); }
            set { SetValue(HintTypeSettingsProperty, value); }
        }

        public HintType HintType { get; private set; }
        public bool DoSuggestions { get; set; } = true;

        public SavedHint SavedHint { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HintInfo(HintType hintType, string panelName, bool isSavedHint = false)
        {
            InitializeComponent();
            DataContext = this;
            if (!isSavedHint) Location.Loaded += (sender, e) => Location.Focus();
            HintTypeSettings = HintTypeSettingsList.SETTINGS[hintType];
            OnPropertyChanged(nameof(HintTypeSettings));
            SuggestionBox.ItemsSource = new List<string>(HintData.SortedRegions);
            HintType = hintType;
            SavedHint = new SavedHint(panelName, Location.Text, PotionCount.Text, new(), new());
            ItemsOnPath.HintInfo = this;
            RightItems.HintInfo = this;
            if (suggestionToShortcutInfo.ContainsKey(HintTypeSettings.HintSuggestion))
            {
                hintShortcutInfo = suggestionToShortcutInfo[HintTypeSettings.HintSuggestion];
            }
        }

        public void UpdateSelectedItems()
        {
            SavedHint.PathItems = ItemsOnPath.SelectedItems;
            SavedHint.FoundItems = RightItems.SelectedItems;
            mainWindow.DataSaver.Save();
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
                mainWindow.DataSaver.Save();
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
            foreach (var entry in HintData.UserShortcuts[hintShortcutInfo.JSONShortcutsKey])
            {
                var shortcut = entry.Key;
                if (shortcut.Contains(Location.Text.ToLower()))
                {
                    matches.Add(entry.Value);
                }
            }
        }

        private void SetUpSuggestions()
        {
            var matches = new List<string>();
            if (HintTypeSettings.HintSuggestion == HintSuggestion.NONE)
            {
                SuggestionBox.ItemsSource = null;
                return;
            }
            CheckForShortcuts(matches);
            if (matches.Count > 0)
            {
                matches.Sort();
                SuggestionBox.ItemsSource = matches;
            }
            else
            {
                List<string> sortBy = hintShortcutInfo.DefaultSortedList;
                SuggestionBox.ItemsSource = sortBy.Where(item => item.ToLower().Contains(Location.Text.ToLower()));
            }
        }

        private void Location_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DoSuggestions) return;
            SetUpSuggestions();
            BottomRow.IsOpen = Location.Text != "" && (SuggestionBox.Items.Count > 0) && HintTypeSettings.HintSuggestion != HintSuggestion.NONE;
            SavedHint.LocationText = Location.Text;
        }

        private bool TryProcessKongHint()
        {
            var hintWordList = Location.Text.Split(' ');
            if (hintWordList.Length != 3) return false;
            if (HintData.UserShortcuts["Kong Hint Shorthand"] == null) return false;
            var shorthands = HintData.UserShortcuts["Kong Hint Shorthand"];
            for (int i = 0; i < hintWordList.Length; ++i)
            {
                hintWordList[i] = hintWordList[i].ToLower();
                if (!shorthands.ContainsKey(hintWordList[i])) return false;
            }
            var foundKongString = shorthands[hintWordList[1]];
            if(!Enum.TryParse(foundKongString, out ItemName kongThatIsFound)) return false;
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var region = shorthands[hintWordList[2]];
            var thingThatFinds = shorthands[hintWordList[0]];
            thingThatFinds = textInfo.ToTitleCase(thingThatFinds.ToLower());
            var newText = thingThatFinds + " in " + region;
            if(thingThatFinds == "Boss")
            {
                newText = region + " Boss Defeated";
            }
            RightItems.SelectedItems = new() { kongThatIsFound };
            RightItems.ProcessSelectedItems();
            Location.Text = newText;
            return true;
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
            if (HintType == HintType.KONGS)
            {
                UserInitialized = TryProcessKongHint();
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
                mainWindow.DataSaver.Save();
            }
        }

        private void SuggestionBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SuggestionBox.SelectedItem != null)
            {
                ProcessSelection();
                mainWindow.DataSaver.Save();
            }
        }

        private void HintInfoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsOnPath != null && RightItems != null)
            {
                // Execute the code only when the visual tree is fully loaded
                Dispatcher.BeginInvoke(new Action(() =>
                {

                    var position = ItemsOnPath.ItemPanel.PointToScreen(new Point(0, 0));
                    double[] selectionDialogPosition = { position.X - 10, position.Y };
                    ItemsOnPath.SelectionDialogPosition = selectionDialogPosition;
                    RightItems.SelectionDialogPosition = selectionDialogPosition;
                }
                ), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
            }
        }
        private void PotionCount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                SavedHint.PotionCountText = PotionCount.Text;
            }
        }
        public void SetUpFromSavedHint(SavedHint savedHint)
        {
            DoSuggestions = false;
            SavedHint = savedHint;
            Location.Text = savedHint.LocationText;
            PotionCount.Text = savedHint.PotionCountText;
            ItemsOnPath.SelectedItems = savedHint.PathItems;
            ItemsOnPath.ProcessSelectedItems();
            RightItems.SelectedItems = savedHint.FoundItems;
            RightItems.ProcessSelectedItems();
            UserInitialized = true;
            DoSuggestions = true;
        }

        private void Location_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right)
            {
                RoutedEvent = MouseDownEvent,
                Source = this
            };
            this.RaiseEvent(args);
            e.Handled = true;
        }
    }
}
