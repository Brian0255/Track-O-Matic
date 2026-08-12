using System.ComponentModel;

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

    public async Task<ItemGridCell?> SelectItemAsync(IReadOnlyList<ItemGridCell> items, string title)
    {
        if (items == null || items.Count == 0)
        {
            return null;
        }

        // Create the dialog window
        var dialog = new FlexibleGridItemSelector(items, title)
        {
            Topmost = _userSettingsService.TopMost,
        };

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
