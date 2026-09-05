using System.ComponentModel;
using System.Windows;

using TrackOMatic.Services;

namespace TrackOMatic.Ui;

/// <summary>
/// WPF implementation of the item selector service.
/// Creates and manages the lifecycle of FlexibleGridItemSelector dialogs.
/// </summary>
internal class ItemSelectorServiceWpf(IUserSettingsService userSettingsService) : IItemSelectorService
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService
        ?? throw new ArgumentNullException(nameof(userSettingsService));

    /// <summary>
    /// Selects an item from a grid of items, displaying a dialog to the user.
    /// </summary>
    /// <remarks>
    /// This method is one of the few explicitly allowed to refer to <see cref="MainWindow"/> in code.
    /// We need the ability to spawn a window where the mouse is located, and this is one of the
    /// slightly better approaches we can take.
    /// </remarks>
    /// <param name="items">The list of items to display along with their coordinates.</param>
    /// <param name="title">The title of the dialog window.</param>
    /// <param name="preferredX">The preferred X coordinate for the dialog window.</param>
    /// <param name="preferredY">The preferred Y coordinate for the dialog window.</param>
    /// <returns>The selected item, or null if no item was selected.</returns>
    public async Task<ItemGridCell?> SelectItemAsync(IReadOnlyList<ItemGridCell> items, string title, double? preferredX = null, double? preferredY = null)
    {
        if (items == null || items.Count == 0)
        {
            return null;
        }

        var mainWindow = Application.Current.MainWindow;

        // Create the dialog window
        var dialog = new FlexibleGridItemSelector(items, title)
        {
            Topmost = _userSettingsService.TopMost,
            Owner = mainWindow,
            WindowStartupLocation = WindowStartupLocation.Manual
        };

        // Position dialog relative to preferred location, or center on main window
        void PositionDialog()
        {
            if (preferredX.HasValue && preferredY.HasValue)
            {
                // Center dialog on the preferred location
                var x = preferredX.Value - (dialog.ActualWidth / 2);
                var y = preferredY.Value - dialog.ActualHeight;
                WindowPositioning.PositionDialogCenteredOn(dialog, x + (dialog.ActualWidth / 2), y + dialog.ActualHeight);
            }
            else if (mainWindow?.IsVisible == true)
            {
                // If no preferred location, center on main window
                var x = mainWindow.Left + (mainWindow.Width - dialog.ActualWidth) / 2;
                var y = mainWindow.Top + (mainWindow.Height - dialog.ActualHeight) / 2;
                WindowPositioning.PositionDialogCenteredOn(dialog, x + (dialog.ActualWidth / 2), y + (dialog.ActualHeight / 2));
            }
        }

        // Position after window is measured (SourceInitialized)
        void SourceInitializedHandler(object? s, EventArgs e)
        {
            dialog.SourceInitialized -= SourceInitializedHandler;
            PositionDialog();
        }

        dialog.SourceInitialized += SourceInitializedHandler;

        // Keep dialog TopMost in sync with user settings while it's open
        var settingsChangedHandler = new PropertyChangedEventHandler((s, e) =>
        {
            if (e.PropertyName == nameof(IUserSettingsService.TopMost))
            {
                dialog.Topmost = _userSettingsService.TopMost;
            }
        });

        _userSettingsService.PropertyChanged += settingsChangedHandler;

        try
        {
            // Show dialog and wait for result
            dialog.ShowDialog();
            return dialog.SelectedItem;
        }
        finally
        {
            _userSettingsService.PropertyChanged -= settingsChangedHandler;
        }
    }
}
