using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HintInfo : UserControl, INotifyPropertyChanged
{
    private bool UserInitialized = false;
    private MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
    private Dictionary<HintSuggestion, HintSuggestionConfig> SuggestionToShortcutInfo { get; } = new()
    {
        {HintSuggestion.REGION, new HintSuggestionConfig(["Hint Regions"], HintRegion.All) },
        {HintSuggestion.CHECK, new HintSuggestionConfig(["Item Locations"], HintLocation.All) },
        {HintSuggestion.MOVE, new HintSuggestionConfig(["Moves"], HintMove.All) },
        {HintSuggestion.REGION_OR_MOVE, new HintSuggestionConfig(["Hint Regions","Moves"], HintRegion.All.Concat(HintMove.All).ToList()) }
    };
    private HintSuggestionConfig? HintSuggestionConfig { get; init; }

    public static readonly DependencyProperty HintTypeSettingsProperty = DependencyProperty.Register("HintTypeSettings", typeof(HintTypeSettings), typeof(HintInfo));
    public HintTypeSettings HintTypeSettings
    {
        get { return (HintTypeSettings)GetValue(HintTypeSettingsProperty); }
        set { SetValue(HintTypeSettingsProperty, value); }
    }

    public HintType HintType { get; private set; }

    private HintPanelType HintPanelType { get; init; }

    public bool DoSuggestions { get; set; } = true;

    public RegionName RegionName { get; }

    public SavedHint SavedHint { get; private set; }

    private IUserSettingsService UserSettingsService { get; init; }

    private HintInfoViewModel _viewModel;

    /// <summary>
    /// Exposes the ViewModel for XAML binding (follows MVVM pattern).
    /// Notifies when changed so Grid's DataContext binding picks it up.
    /// </summary>
    public HintInfoViewModel ViewModel
    {
        get => _viewModel;
        private set
        {
            if (_viewModel != value)
            {
                _viewModel = value;
                OnPropertyChanged(nameof(ViewModel));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public HintInfo(HintType hintType, HintPanelType hintPanelType, string panelName, bool isSavedHint = false, RegionName regionName = RegionName.UNKNOWN)
    {
        InitializeComponent();
        UserSettingsService = ServiceLocator.GetService<IUserSettingsService>();

        // Initialize properties from constructor parameters
        HintPanelType = hintPanelType;
        RegionName = regionName;

        if (!isSavedHint)
        {
            Location.Loaded += (sender, e) => Location.Focus();
        }

        HintTypeSettings = HintTypeSettingsList.SETTINGS[hintType];
        OnPropertyChanged(nameof(HintTypeSettings));
        HintType = hintType;
        SavedHint = new SavedHint(hintPanelType, Location.Text, "", [], []);
        ItemsOnPath.HintInfo = this;
        RightItems.HintInfo = this;
        if (!HintTypeSettings.PathItemsVisible)
        {
            RightItems.BottomRow.Height = new GridLength(0);
        }
        if (SuggestionToShortcutInfo.ContainsKey(HintTypeSettings.HintSuggestion))
        {
            HintSuggestionConfig = SuggestionToShortcutInfo[HintTypeSettings.HintSuggestion];
        }

        // Temporarily declared in code behind.
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            return; // Already loaded
        }

        var viewModel = new HintInfoViewModel(
            HintType,
            HintPanelType,
            RegionName,
            SavedHint,
            ServiceLocator.GetService<IShortcutService>(),
            ServiceLocator.GetService<IHintItemSelectionService>(),
            ServiceLocator.GetService<ISavedProgressProvider>(),
            ServiceLocator.GetService<IItemTrackingService>(),
            UserSettingsService
        );

        // Setting this property triggers OnPropertyChanged, which the Grid's binding listens to
        ViewModel = viewModel;

        // Wire up dialog-opening notification from ViewModel
        viewModel.PathItemsSelectionRequested += (s, e) => ItemsOnPath.OpenItemSelectionDialog();
        viewModel.FoundItemsSelectionRequested += (s, e) => RightItems.OpenItemSelectionDialog();

        // Reset focused suggestion index when suggestions change
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(HintInfoViewModel.Suggestions))
            {
                _focusedSuggestionIndex = -1;
            }
        };
    }

    public void UpdateSelectedItems()
    {
        SavedHint.PathItems = ItemsOnPath.SelectedItems;
        SavedHint.FoundItems = RightItems.SelectedItems;
        mainWindow.DataSaver.Save();
    }

    private void Location_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            string locationText = Location.Text;

            // Check if focus is in the SuggestionBox (dropdown)
            if (SuggestionBox.IsKeyboardFocusWithin && SuggestionBox.SelectedItem is string selectedSuggestion)
            {
                // Use the selected item from the dropdown
                locationText = selectedSuggestion;
            }
            else if (_viewModel?.Suggestions.Count > 0 && Location.IsFocused)
            {
                // Focus is in the TextBox and there are suggestions - use first suggestion
                locationText = _viewModel.Suggestions[0];
            }
            // else: use what's already in Location.Text

            // Update the Location TextBox
            Location.Text = locationText;

            // Close the suggestion dropdown
            if (_viewModel != null)
            {
                _viewModel.SuggestionBoxIsOpen = false;
            }

            // Extract screen coordinates and invoke ViewModel command
            var mousePos = Mouse.GetPosition(this);
            var screenMousePosition = PointToScreen(mousePos);
            var parameter = (Location.Text, screenMousePosition.X, screenMousePosition.Y);
            if (_viewModel?.ConfirmLocationAndItemsCommand.CanExecute(parameter) == true)
            {
                _viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
            }

            // Clear focus
            Keyboard.ClearFocus();

            // TODO: Phase 2 - Uncomment when persistence layer is ready
            // mainWindow.DataSaver.Save();


            if (_viewModel?.FocusAndSelectPotionCountCommand.CanExecute(null) == true)
            {
                _viewModel.FocusAndSelectPotionCountCommand.Execute(null);
            }

            e.Handled = true;
        }
        else if (e.Key == Key.Down)
        {
            // Allow keyboard navigation into suggestions without auto-selecting
            if (_viewModel?.Suggestions.Count > 0 && _viewModel.SuggestionBoxIsOpen)
            {
                // Move focus directly to the first item in the ListBox
                SuggestionBox.UpdateLayout();
                var firstItem = (ListBoxItem)SuggestionBox.ItemContainerGenerator.ContainerFromIndex(0);
                if (firstItem != null)
                {
                    _focusedSuggestionIndex = 0;
                    firstItem.Focus();
                }
                else
                {
                    // Fall back to focusing the ListBox if item container isn't ready
                    _focusedSuggestionIndex = 0;
                    SuggestionBox.Focus();
                }
                e.Handled = true;
            }
        }
    }

    private void SuggestionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Only process selection if it came from user action (Enter or mouse click)
        // Not from keyboard navigation (Down/Up arrows)
        if (Keyboard.IsKeyDown(Key.Enter) || Keyboard.IsKeyDown(Key.Return) || _isMouseSelecting)
        {
            // This is from the Enter key
            if (SuggestionBox.SelectedItem is string selectedSuggestion && _viewModel != null)
            {
                Location.Text = selectedSuggestion;
                _viewModel.SuggestionBoxIsOpen = false;

                var mousePos = Mouse.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePos);
                var parameter = (Location.Text, screenMousePosition.X, screenMousePosition.Y);
                if (_viewModel.ConfirmLocationAndItemsCommand.CanExecute(parameter))
                {
                    _viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
                }

                Keyboard.ClearFocus();

                if (_viewModel?.FocusAndSelectPotionCountCommand.CanExecute(null) == true)
                {
                    _viewModel.FocusAndSelectPotionCountCommand.Execute(null);
                }
            }
        }
        else
        {
            // This is from keyboard navigation (Down/Up) - clear the selection
            SuggestionBox.SelectedIndex = -1;
        }
    }

    private bool _isMouseSelecting = false;
    private int _focusedSuggestionIndex = -1;


    private void SuggestionBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            // Handle Enter key when focus is in the SuggestionBox
            // Use the tracked focused index instead of SelectedItem (which we clear during navigation)
            if (_focusedSuggestionIndex >= 0 && _focusedSuggestionIndex < _viewModel?.Suggestions.Count && _viewModel != null)
            {
                string selectedSuggestion = _viewModel.Suggestions[_focusedSuggestionIndex];

                // Use the selected item from the dropdown
                Location.Text = selectedSuggestion;

                // Close the suggestion dropdown
                _viewModel.SuggestionBoxIsOpen = false;

                // Extract screen coordinates and invoke ViewModel command
                var mousePos = Mouse.GetPosition(this);
                var screenMousePosition = PointToScreen(mousePos);
                var parameter = (Location.Text, screenMousePosition.X, screenMousePosition.Y);
                if (_viewModel.ConfirmLocationAndItemsCommand.CanExecute(parameter))
                {
                    _viewModel.ConfirmLocationAndItemsCommand.Execute(parameter);
                }

                // Clear focus and reset the index
                Keyboard.ClearFocus();
                _focusedSuggestionIndex = -1;

                e.Handled = true;
            }
        }
        else if (e.Key == Key.Down)
        {
            // Track Down navigation
            if (_focusedSuggestionIndex >= 0 && _focusedSuggestionIndex < _viewModel?.Suggestions.Count - 1)
            {
                _focusedSuggestionIndex++;
            }
        }
        else if (e.Key == Key.Up)
        {
            // Track Up navigation
            if (_focusedSuggestionIndex > 0)
            {
                _focusedSuggestionIndex--;
            }
        }
    }

    private void SuggestionBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isMouseSelecting = true;
    }

    /// <summary>
    /// Handles Preview KeyDown event for the PotionCount TextBox.
    /// When Enter/Return is pressed, confirms the potion count and prepares to save the hint.
    /// </summary>
    private void PotionCount_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            // Invoke the command to confirm potion count and prepare to save
            if (_viewModel?.ConfirmPotionCountCommand.CanExecute(null) == true)
            {
                _viewModel.ConfirmPotionCountCommand.Execute(null);
            }

            // Clear focus from the potion count box
            Keyboard.ClearFocus();
            e.Handled = true;
        }
    }

    public void OnRemove()
    {
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        if (HintType != HintType.DIRECT_ITEM_HINT)
        {
            return;
        }

        foreach (var entry in RightItems.SelectedItems)
        {
            var itemName = entry.Key;
            if (!mainWindow.ITEM_NAME_TO_ITEM.ContainsKey(itemName))
            {
                continue;
            }

            var matchingItem = mainWindow.ITEM_NAME_TO_ITEM[itemName];
            if (matchingItem.ItemImage.ToString().Contains("bw") && matchingItem.Parent != mainWindow.ItemGrid)
            {
                matchingItem.HandleItemReturn();
            }
        }
    }
    public void SetUpFromSavedHint(SavedHint savedHint)
    {
        DoSuggestions = false;
        SavedHint = savedHint;
        Location.Text = savedHint.LocationText;
        // PotionCountText binding is updated automatically via ViewModel since SavedHint is set above
        if (_viewModel != null)
        {
            _viewModel.PotionCountText = savedHint.PotionCountText;
        }
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
        RaiseEvent(args);
        e.Handled = true;
    }
}
