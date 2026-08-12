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
                UIUtils.MoveWindowAndEnsureVisible(dialog, x, y);
            }
            else if (mainWindow?.IsVisible == true)
            {
                // If no preferred location, center on main window
                var x = mainWindow.Left + (mainWindow.Width - dialog.ActualWidth) / 2;
                var y = mainWindow.Top + (mainWindow.Height - dialog.ActualHeight) / 2;
                UIUtils.MoveWindowAndEnsureVisible(dialog, x, y);
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
