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

        // Compute positioning parameters before creating event handlers
        double centerX, centerY;

        if (preferredX.HasValue && preferredY.HasValue)
        {
            // Position relative to preferred location (centered above it)
            centerX = preferredX.Value;
            centerY = preferredY.Value;
        }
        else if (mainWindow?.IsVisible == true)
        {
            // Center on main window
            centerX = mainWindow.Left + (mainWindow.Width / 2);
            centerY = mainWindow.Top + (mainWindow.Height / 2);
        }
        else
        {
            // Fallback: no positioning
            centerX = 0;
            centerY = 0;
        }

        // Position dialog - this will subscribe to SourceInitialized internally
        WindowPositioning.PositionDialogCenteredOn(dialog, centerX, centerY);

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
