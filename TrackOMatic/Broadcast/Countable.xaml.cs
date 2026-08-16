using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;
/// <summary>
/// Interaction logic for Countable.xaml
/// </summary>
public partial class Countable : UserControl
{
    public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register(
        nameof(ItemType),
        typeof(ItemType),
        typeof(Countable),
        new PropertyMetadata(ItemType.MISC, OnItemTypeChanged)
    );

    public ItemType ItemType
    {
        get => (ItemType)GetValue(ItemTypeProperty);
        set => SetValue(ItemTypeProperty, value);
    }

    public Countable()
    {
        InitializeComponent();
    }

    private static void OnItemTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Countable control || e.NewValue is not ItemType itemType)
        {
            return;
        }
        // Create ViewModel with injected services
        var collectiblesService = ServiceLocator.GetService<ICollectiblesService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        if (collectiblesService != null && parsedSpoilerDataService != null)
        {
            // Dispose the old view model if it exists.
            if (control.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            control.DataContext = new CountableViewModel(itemType, collectiblesService, parsedSpoilerDataService);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
        DataContext = null;
    }
}
