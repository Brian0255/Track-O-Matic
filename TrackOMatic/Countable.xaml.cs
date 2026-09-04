using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

namespace TrackOMatic;
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
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();
        if (collectiblesService != null && parsedSpoilerDataService != null)
        {
            // Dispose the old view model if it exists.
            if (control.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            control.DataContext = new UiCountableViewModel(itemType, collectiblesService, parsedSpoilerDataService, userSettingsService);
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

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not UiCountableViewModel viewModel)
        {
            return;
        }

        if (viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not UiCountableViewModel viewModel)
        {
            return;
        }

        if (viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiCountableViewModel viewModel)
        {
            return;
        }
        if (e.Delta > 0)
        {
            if (viewModel.IncrementCountCommand.CanExecute(null))
            {
                viewModel.IncrementCountCommand.Execute(null);
            }
        }
        else if (e.Delta < 0)
        {
            if (viewModel.DecrementCountCommand.CanExecute(null))
            {
                viewModel.DecrementCountCommand.Execute(null);
            }

        }
        e.Handled = true;
    }
}
