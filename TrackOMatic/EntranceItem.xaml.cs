using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TrackOMatic
{
    /// <summary>
    /// Interaction logic for EntranceItem.xaml
    /// </summary>
    public partial class EntranceItem : UserControl
    {
        public string DisplayName { get; private set; }
        public string InternalName { get; private set; }
        private List<string> allEntranceNames;
        private bool isSelectingFromList = false;

        public EntranceItem(string displayName, string internalName)
        {
            InitializeComponent();
            DisplayName = displayName;
            InternalName = internalName;
            EntranceName.Text = displayName;
        }

        public void SetEntranceNamesList(List<string> entranceNames)
        {
            allEntranceNames = entranceNames;
        }

        public void SetDestination(string destination)
        {
            DestinationTextBox.Text = destination;
        }

        public string GetDestination()
        {
            return DestinationTextBox.Text?.Trim() ?? string.Empty;
        }

        public void ClearDestination()
        {
            DestinationTextBox.Text = string.Empty;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDestination();
        }

        private void DestinationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isSelectingFromList || allEntranceNames == null)
                return;

            string searchText = DestinationTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(searchText))
            {
                AutocompletePopup.IsOpen = false;
                return;
            }

            // Filter entrance names that contain the search text (case-insensitive)
            var filteredEntrances = allEntranceNames
                .Where(name => name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(name => 
                {
                    // Prioritize matches that start with the search text
                    if (name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                        return 0;
                    return 1;
                })
                .ThenBy(name => name)
                .ToList();

            if (filteredEntrances.Count > 0)
            {
                AutocompleteListBox.ItemsSource = filteredEntrances;
                AutocompletePopup.IsOpen = true;
                AutocompletePopup.Width = DestinationTextBox.ActualWidth;
            }
            else
            {
                AutocompletePopup.IsOpen = false;
            }
        }

        private void DestinationTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (AutocompletePopup.IsOpen)
            {
                if (e.Key == Key.Down)
                {
                    AutocompleteListBox.Focus();
                    if (AutocompleteListBox.Items.Count > 0)
                    {
                        AutocompleteListBox.SelectedIndex = 0;
                    }
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    AutocompletePopup.IsOpen = false;
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter || e.Key == Key.Tab)
                {
                    if (AutocompleteListBox.Items.Count > 0)
                    {
                        SelectItem(0);
                    }
                    AutocompletePopup.IsOpen = false;
                    e.Handled = (e.Key == Key.Enter);
                }
            }
        }

        private void AutocompleteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AutocompleteListBox.SelectedItem != null)
            {
                SelectItem(AutocompleteListBox.SelectedIndex);
            }
        }

        private void AutocompleteListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (AutocompleteListBox.SelectedItem != null)
                {
                    SelectItem(AutocompleteListBox.SelectedIndex);
                    AutocompletePopup.IsOpen = false;
                    DestinationTextBox.Focus();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                AutocompletePopup.IsOpen = false;
                DestinationTextBox.Focus();
                e.Handled = true;
            }
        }

        private void SelectItem(int index)
        {
            if (AutocompleteListBox.Items.Count > index)
            {
                isSelectingFromList = true;
                DestinationTextBox.Text = AutocompleteListBox.Items[index].ToString();
                DestinationTextBox.CaretIndex = DestinationTextBox.Text.Length;
                isSelectingFromList = false;
                AutocompletePopup.IsOpen = false;
            }
        }

        private void DestinationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Delay closing the popup to allow clicking on list items
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!AutocompleteListBox.IsKeyboardFocusWithin)
                {
                    AutocompletePopup.IsOpen = false;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void AutocompletePopup_Closed(object sender, EventArgs e)
        {
            // Clean up when popup closes
        }
    }
}
