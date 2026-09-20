using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for KongBlueprint.xaml
/// </summary>
public partial class KongBlueprint : UserControl
{
    public KongBlueprint()
    {
        InitializeComponent();
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiKongBlueprintViewModel viewModel && viewModel.IncrementCountCommand.CanExecute(null))
        {
            viewModel.IncrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UiKongBlueprintViewModel viewModel && viewModel.DecrementCountCommand.CanExecute(null))
        {
            viewModel.DecrementCountCommand.Execute(null);
        }
    }

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not UiKongBlueprintViewModel viewModel)
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
}
