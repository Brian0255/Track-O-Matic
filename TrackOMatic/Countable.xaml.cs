using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for Countable.xaml
/// </summary>
public partial class Countable : UserControl
{
    public Countable()
    {
        InitializeComponent();
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
