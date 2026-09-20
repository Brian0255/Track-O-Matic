using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for HelmDoorSlot.xaml
/// </summary>
public partial class HelmDoorSlot : UserControl
{
    public HelmDoorSlot()
    {
        InitializeComponent();
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HelmDoorSlotViewModel viewModel)
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

    private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not HelmDoorSlotViewModel viewModel)
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
    }
}
