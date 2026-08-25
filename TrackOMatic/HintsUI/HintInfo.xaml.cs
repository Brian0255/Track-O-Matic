using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Diagnostics;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HintInfo : UserControl
{
    private HintInfoViewModel? _viewModel;
    private bool _isInitialized = false;
    private Popup? _cachedPopup = null;

    public HintInfo()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Guard against multiple Loaded events
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        _viewModel = DataContext as HintInfoViewModel;
        if (_viewModel is null || FindName("SuggestionPopup") is not Popup popup || FindName("Location") is not TextBox locationBox)
        {
            return;
        }

        _cachedPopup = popup;

        // CRITICAL: Set PlacementTarget BEFORE subscribing to any property changes
        popup.PlacementTarget = locationBox;
        popup.Placement = PlacementMode.Bottom;
        popup.AllowsTransparency = false; // Disable transparency to avoid rendering artifacts

        // Handle popup closing to sync state back
        popup.Closed += (s, e) => 
        {
            if (_viewModel.SuggestionBoxIsOpen)
            {
                _viewModel.SuggestionBoxIsOpen = false;
            }
        };

        // Subscribe to ViewModel changes AFTER placement is configured
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        // Set initial state
        popup.IsOpen = _viewModel.SuggestionBoxIsOpen;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HintInfoViewModel.SuggestionBoxIsOpen))
        {
            if (_cachedPopup is not null && _viewModel is not null)
            {
                bool shouldBeOpen = _viewModel.SuggestionBoxIsOpen;
                if (_cachedPopup.IsOpen != shouldBeOpen)
                {
                    _cachedPopup.IsOpen = shouldBeOpen;
                }
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
        _cachedPopup = null;
    }

    private void Location_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var viewModel = DataContext as HintInfoViewModel;
        if (viewModel is null)
        {
            return;
        }

        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            string locationText = viewModel.LocationText ?? "";

            // Check if focus is in the SuggestionBox (dropdown) and an item is selected
            if (viewModel.IsSuggestionBoxFocused == true && viewModel.SelectedSuggestion is string selectedSuggestion)
            {
                // Use the selected item from the dropdown
                locationText = selectedSuggestion;
            }
            else if (viewModel.Suggestions.Count > 0 && (sender as TextBox)?.IsFocused == true)
            {
                // Focus is in the TextBox and there are suggestions - use first suggestion
                locationText = viewModel.Suggestions[0];
            }
            // else: use what's already in LocationText

            // Update the Location TextBox via ViewModel
            viewModel.LocationText = locationText;

            // Close the suggestion dropdown
            viewModel.SuggestionBoxIsOpen = false;

            // Extract screen coordinates and invoke ViewModel command
            var mousePos = Mouse.GetPosition(this);
            var screenMousePosition = PointToScreen(mousePos);
            var parameter = (locationText, screenMousePosition.X, screenMousePosition.Y);
            if (viewModel.ConfirmLocationAndItemsCommand.CanExecute(parameter) == true)
            {
                viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
            }

            // Clear focus
            Keyboard.ClearFocus();

            // TODO: Phase 2 - Uncomment when persistence layer is ready
            // mainWindow.DataSaver.Save();


            if (viewModel.FocusAndSelectPotionCountCommand.CanExecute(null) == true)
            {
                viewModel.FocusAndSelectPotionCountCommand.Execute(null);
            }

            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            // Allow keyboard navigation into suggestions without auto-selecting
            if (viewModel.Suggestions.Count > 0 && viewModel.SuggestionBoxIsOpen)
            {
                // Signal the behavior to focus the first item in the ListBox
                viewModel.ShouldFocusSuggestionsFirst = true;
                // Initialize the focus index in ViewModel for navigation tracking
                viewModel.SelectedSuggestionIndex = 0;
                e.Handled = true;
            }
        }
    }

    private void SuggestionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox || DataContext is not HintInfoViewModel viewModel)
        {
            return;
        }
        if (!listBox.IsLoaded)
        {
            return;
        }

        // Only process if user explicitly selected via mouse or Enter key
        // Keyboard navigation (arrow keys) should not trigger this flow
        if (listBox.SelectedItem is string selectedSuggestion &&
            (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Return)))
        {
            // Confirm the selection and advance to potion count
            viewModel.LocationText = selectedSuggestion;
            viewModel.SuggestionBoxIsOpen = false;

            var mousePos = Mouse.GetPosition(this);
            var screenMousePosition = PointToScreen(mousePos);
            var parameter = (selectedSuggestion, screenMousePosition.X, screenMousePosition.Y);
            if (viewModel.ConfirmLocationAndItemsCommand.CanExecute(parameter))
            {
                viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
            }

            Keyboard.ClearFocus();

            if (viewModel.FocusAndSelectPotionCountCommand.CanExecute(null) == true)
            {
                viewModel.FocusAndSelectPotionCountCommand.Execute(null);
            }
        }
    }

    private void SuggestionBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var viewModel = DataContext as HintInfoViewModel;
        if (viewModel is null)
        {
            return;
        }

        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            // Handle Enter key when focus is in the SuggestionBox
            // Use the ViewModel's tracked focus index to get the highlighted item
            int selectedIndex = viewModel.SelectedSuggestionIndex;
            if (selectedIndex >= 0 && selectedIndex < viewModel.Suggestions.Count)
            {
                string selectedSuggestion = viewModel.Suggestions[selectedIndex];

                // Use the highlighted item from the dropdown
                viewModel.LocationText = selectedSuggestion;
                viewModel.SuggestionBoxIsOpen = false;

                // Extract screen coordinates and invoke ViewModel command
                var mousePos = Mouse.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePos);
                var parameter = (selectedSuggestion, screenMousePosition.X, screenMousePosition.Y);
                if (viewModel.ConfirmLocationAndItemsCommand.CanExecute(parameter))
                {
                    viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
                }

                // Clear focus and reset the index
                Keyboard.ClearFocus();
                viewModel.SelectedSuggestionIndex = -1;

                e.Handled = true;
            }
        }
        else if (e.Key == Key.Down)
        {
            // Track Down navigation in ViewModel
            int currentIndex = viewModel.SelectedSuggestionIndex;
            if (currentIndex >= 0 && currentIndex < viewModel.Suggestions.Count - 1)
            {
                viewModel.SelectedSuggestionIndex++;
            }
            else if (currentIndex < 0)
            {
                // First Down key press: start at index 0
                viewModel.SelectedSuggestionIndex = 0;
            }
        }
        else if (e.Key == Key.Up)
        {
            // Track Up navigation in ViewModel
            int currentIndex = viewModel.SelectedSuggestionIndex;
            if (currentIndex > 0)
            {
                viewModel.SelectedSuggestionIndex--;
            }
        }
    }

    /// <summary>
    /// Handles Preview KeyDown event for the PotionCount TextBox.
    /// When Enter/Return is pressed, confirms the potion count and prepares to save the hint.
    /// </summary>
    private void PotionCount_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var viewModel = DataContext as HintInfoViewModel;
        if (viewModel is null)
        {
            return;
        }
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            // Invoke the command to confirm potion count and prepare to save
            if (viewModel?.ConfirmPotionCountCommand.CanExecute(null) == true)
            {
                viewModel.ConfirmPotionCountCommand.Execute(null);
            }

            // Clear focus from the potion count box
            Keyboard.ClearFocus();
            e.Handled = true;
        }
    }

    private void Location_PreviewMouseRightButtonDown(object sender, MouseEventArgs e)
    {
        var viewModel = DataContext as HintInfoViewModel;
        if (viewModel?.RemoveHintCommand.CanExecute(null) == true)
        {
            viewModel.RemoveHintCommand.Execute(null);
        }
        e.Handled = true;
    }
}
