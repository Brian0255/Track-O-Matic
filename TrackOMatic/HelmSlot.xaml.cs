using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

namespace TrackOMatic;
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
        Unloaded += OnUnloaded;
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
        var itemSelectorService = ServiceLocator.GetService<IItemSelectorService>();
        if (endGamaeProgressionService is not null && parsedSpoilerDataService is not null && savedProgressProvider is not null)
        {
            // Dispose the old view model if it exists.
            if (control.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            control.DataContext = new UiHelmSlotViewModel(
                control.SlotNumber,
                endGamaeProgressionService,
                parsedSpoilerDataService,
                savedProgressProvider,
                itemSelectorService
            );
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not UiHelmSlotViewModel viewModel)
        {
            return;
        }

        if (viewModel.SelectItemCommand.CanExecute(null))
        {
            viewModel.SelectItemCommand.Execute(null);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiHelmSlotViewModel viewModel)
        {
            return;
        }
        if (e.Delta > 0 && viewModel.CycleNextCommand.CanExecute(null))
        {
            viewModel.CycleNextCommand.Execute(null);
        }
        else if (e.Delta < 0 && viewModel.CyclePreviousCommand.CanExecute(null))
        {
            viewModel.CyclePreviousCommand.Execute(null);
        }

        e.Handled = true;
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
