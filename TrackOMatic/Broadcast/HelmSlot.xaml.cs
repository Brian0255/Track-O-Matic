using System.Windows;
using System.Windows.Controls;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels.Broadcast;

namespace TrackOMatic.Broadcast;
/// <summary>
/// Interaction logic for HelmSlot.xaml
/// </summary>
public partial class HelmSlot : UserControl
{
    public static readonly DependencyProperty SlotNumberProperty = DependencyProperty.Register(
        nameof(SlotNumber),
        typeof(int),
        typeof(HelmSlot),
        new PropertyMetadata(-1, OnHelmSlotChanged)
    );

    public int SlotNumber
    {
        get => (int)GetValue(SlotNumberProperty);
        set => SetValue(SlotNumberProperty, value);
    }

    public HelmSlot()
    {
        InitializeComponent();
    }

    private static void OnHelmSlotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not HelmSlot control || e.NewValue is not int newSlotNumber)
        {
            return;
        }
        SetUpContext(control);
    }

    private static void SetUpContext(HelmSlot control)
    {
        if (control.SlotNumber < 0 || control.SlotNumber > 4)
        {
            return;
        }

        var endGamaeProgressionService = ServiceLocator.GetService<IEndGameProgressionService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        if (endGamaeProgressionService is not null && parsedSpoilerDataService is not null && savedProgressProvider is not null)
        {
            // Dispose the old view model if it exists.
            if (control.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            control.DataContext = new HelmSlotViewModel(
                control.SlotNumber,
                endGamaeProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider
            );
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
