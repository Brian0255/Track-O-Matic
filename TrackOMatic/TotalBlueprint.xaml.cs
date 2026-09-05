using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;
using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for TotalBlueprint.xaml
/// </summary>
public partial class TotalBlueprint : UserControl
{
    public TotalBlueprint()
    {
        InitializeComponent();

        // There's no depenency properties, so just initialize the data context now.
        var collectiblesService = ServiceLocator.GetService<ICollectiblesService>();
        var parsedSpoilerDataService = ServiceLocator.GetService<IParsedSpoilerDataService>();
        var savedProgressProvider = ServiceLocator.GetService<ISavedProgressProvider>();
        var userSettingsService = ServiceLocator.GetService<IUserSettingsService>();

        if (collectiblesService != null && parsedSpoilerDataService != null && savedProgressProvider != null && userSettingsService != null)
        {
            DataContext = new UiTotalBlueprintViewModel(collectiblesService, parsedSpoilerDataService, savedProgressProvider, userSettingsService);
        }
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiTotalBlueprintViewModel viewModel && viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiTotalBlueprintViewModel viewModel && viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiTotalBlueprintViewModel viewModel)
        {
            return;
        }
        if (e.Delta > 0 && viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
        else if (e.Delta < 0 && viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
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
