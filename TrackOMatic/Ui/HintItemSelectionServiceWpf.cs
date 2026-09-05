using System.ComponentModel;
using System.Windows;

using TrackOMatic.HintsUI;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models.Hints;
using TrackOMatic.Services;

namespace TrackOMatic.Ui;

public class HintItemSelectionServiceWpf(IUserSettingsService userSettingsService) : IHintItemSelectionService
{
    private readonly IUserSettingsService _userSettingsService = userSettingsService
        ?? throw new ArgumentNullException(nameof(userSettingsService));

    public async Task<IList<HintGridCell>> SelectItemsAsync(
        IReadOnlyList<HintGridCell> items,
        IList<ItemName> itemsFoundAlready,
        string title,
        double? preferredX = null,
        double? preferredY = null
    )
    {
        if (items is null || items.Count == 0)
        {
            return [];
        }

        // Necessary evil: need MainWindow for the owner.
        var mainWindow = Application.Current.MainWindow;

        var dialog = new HintMultiSelectionDialog(items, itemsFoundAlready, title)
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
            // Show dialog modally and wait for it to close
            dialog.ShowDialog();

            // Get the selected items from the dialog (works regardless of how it was closed)
            var selectedItems = dialog.GetSelectedItems();

            return selectedItems;
        }
        finally
        {
            _userSettingsService.PropertyChanged -= settingsChangedHandler;
        }
    }
}
