using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels;

namespace TrackOMatic;
/// <summary>
/// Interaction logic for HelmSlot.xaml
/// </summary>
public partial class HelmSlot : UserControl
{
    public HelmSlot()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not UiHelmSlotViewModel viewModel)
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
}
