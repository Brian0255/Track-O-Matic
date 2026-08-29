using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;

/// <summary>
/// Interaction logic for Broadcast Item display (read-only).
/// </summary>
public partial class Item : UserControl
{
    public static readonly DependencyProperty ItemNameProperty = DependencyProperty.Register(
        nameof(ItemName),
        typeof(ItemName),
        typeof(Item),
        new PropertyMetadata(ItemName.BASIC_KEY, OnItemNameChanged));

    public ItemName ItemName
    {
        get => (ItemName)GetValue(ItemNameProperty);
        set => SetValue(ItemNameProperty, value);
    }

    public Item()
    {
        InitializeComponent();
    }

    private static void OnItemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Item control && e.NewValue is ItemName itemName)
        {
            // Create ViewModel with injected services
            var itemTrackingService = ServiceLocator.GetService<IItemTrackingService>();
            var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
            var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
            var themeService = ServiceLocator.GetService<IThemeService>();

            if (itemTrackingService != null && parsedSpoilerDataService != null && savedProgressProvider != null)
            {
                control.DataContext = new BroadcastItemViewModel(itemName, itemTrackingService, parsedSpoilerDataService, savedProgressProvider, themeService);
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        (DataContext as IDisposable)?.Dispose();
        DataContext = null;
    }
}
