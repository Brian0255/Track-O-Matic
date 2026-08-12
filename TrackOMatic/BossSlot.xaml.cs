using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for BossSlot.xaml
/// </summary>
public partial class BossSlot : UserControl
{
    public static readonly DependencyProperty SlotNumberProperty = DependencyProperty.Register(
        nameof(SlotNumber),
        typeof(int),
        typeof(BossSlot),
        new PropertyMetadata(-1, OnBossSlotChanged)
    );

    public int SlotNumber
    {
        get => (int)GetValue(SlotNumberProperty);
        set => SetValue(SlotNumberProperty, value);
    }
    public BossSlot()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
    }

    private static void OnBossSlotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not BossSlot control || e.NewValue is not int newSlotNumber)
        {
            return;
        }
        SetUpContext(control);
    }

    private static void SetUpContext(BossSlot control)
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

            control.DataContext = new UiBossSlotViewModel(
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
        if (DataContext is not UiBossSlotViewModel viewModel)
        {
            return;
        }

        var mousePos = Mouse.GetPosition(this);
        var screenMousePosition = PointToScreen(mousePos);
        var parameter = (screenMousePosition.X, screenMousePosition.Y);

        if (viewModel.SelectItemCommand.CanExecute(parameter))
        {
            viewModel.SelectItemCommand.Execute(parameter);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiBossSlotViewModel viewModel)
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
