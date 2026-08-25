using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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

        // PlacementTarget must be set so the popup knows where to position itself
        popup.PlacementTarget = locationBox;

        // Subscribe to ViewModel changes
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;

        // Subscribe to dialog opening events
        _viewModel.PathItemsSelectionRequested += ViewModel_PathItemsSelectionRequested;
        _viewModel.FoundItemsSelectionRequested += ViewModel_FoundItemsSelectionRequested;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // The popup IsOpen is now handled by binding, so we don't need to manually sync it here.
        // This method can be removed if no other properties need special handling.
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _viewModel.PathItemsSelectionRequested -= ViewModel_PathItemsSelectionRequested;
            _viewModel.FoundItemsSelectionRequested -= ViewModel_FoundItemsSelectionRequested;
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

            // Trigger the appropriate action based on hint panel type
            // This will focus the appropriate control (potion count, path items, or found items)
            viewModel.TriggerLocationConfirmationAction();

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
            // Confirm the selection and trigger appropriate action
            viewModel.SelectSuggestionCommand.Execute(selectedSuggestion);
        }
    }

    /// <summary>
    /// Confirms the currently selected suggestion and triggers the appropriate action.
    /// Used by both keyboard (Enter) and mouse (double-click) selection paths.
    /// </summary>
    private void ConfirmSuggestedLocation(HintInfoViewModel viewModel, RoutedEventArgs e)
    {
        int selectedIndex = viewModel.SelectedSuggestionIndex;
        if (selectedIndex >= 0 && selectedIndex < viewModel.Suggestions.Count)
        {
            string selectedSuggestion = viewModel.Suggestions[selectedIndex];

            // Update location and close popup
            viewModel.LocationText = selectedSuggestion;
            viewModel.SuggestionBoxIsOpen = false;

            // Trigger the appropriate action based on hint panel type
            viewModel.TriggerLocationConfirmationAction();

            // Reset the index
            viewModel.SelectedSuggestionIndex = -1;

            e.Handled = true;
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
            ConfirmSuggestedLocation(viewModel, e);
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
    /// Handles PreviewMouseDoubleClick event for the Suggestion ListBox.
    /// Double-clicking an item (with the left mouse button) has the same behavior as pressing Enter/Return.
    /// </summary>
    private void SuggestionBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Only process left mouse button double-clicks
        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        var viewModel = DataContext as HintInfoViewModel;
        if (viewModel is null)
        {
            return;
        }

        ConfirmSuggestedLocation(viewModel, e);
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

    /// <summary>
    /// Handles the PathItemsSelectionRequested event from the ViewModel.
    /// Opens the Path Items selection dialog.
    /// </summary>
    private void ViewModel_PathItemsSelectionRequested(object? sender, EventArgs e)
    {
        if (_viewModel is HintInfoViewModel hintInfoVM)
        {
            var pathViewModel = hintInfoVM.PathHintItemListViewModel;

            if (pathViewModel is not null)
            {
                // Get the current mouse position for dialog placement
                var mousePos = Mouse.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePos);
                var parameter = (screenMousePosition.X, screenMousePosition.Y);

                if (pathViewModel.SelectPathItemsCommand.CanExecute(parameter))
                {
                    pathViewModel.SelectPathItemsCommand.Execute(parameter);
                }
            }
        }
    }

    /// <summary>
    /// Handles the FoundItemsSelectionRequested event from the ViewModel.
    /// Opens the Found Items selection dialog.
    /// </summary>
    private void ViewModel_FoundItemsSelectionRequested(object? sender, EventArgs e)
    {
        if (_viewModel is HintInfoViewModel hintInfoVM)
        {
            var foundViewModel = hintInfoVM.FoundHintItemListViewModel;

            if (foundViewModel is not null)
            {
                // Get the current mouse position for dialog placement
                var mousePos = Mouse.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePos);
                var parameter = (screenMousePosition.X, screenMousePosition.Y);

                if (foundViewModel.SelectFoundItemsCommand.CanExecute(parameter))
                {
                    foundViewModel.SelectFoundItemsCommand.Execute(parameter);
                }
            }
        }
    }
}
