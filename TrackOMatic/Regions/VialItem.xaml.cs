using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for VialItem.xaml
/// </summary>
public partial class VialItem : UserControl
{
    public VialItem()
    {
        InitializeComponent();
    }

    private void VialItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not VialItemViewModel viewModel)
        {
            return;
        }
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (isShiftPressed && viewModel.ToggleVialStarCommand.CanExecute(null))
            {
                viewModel.ToggleVialStarCommand.Execute(null);
            }
            else if (viewModel.RemoveVialFromRegionCommand.CanExecute(null))
            {
                viewModel.RemoveVialFromRegionCommand.Execute(null);
            }
            e.Handled = true;
        }
        else if (e.MiddleButton == MouseButtonState.Pressed)
        {
            if (viewModel.ToggleVialStarCommand.CanExecute(null))
            {
                viewModel.ToggleVialStarCommand.Execute(null);
            }
            e.Handled = true;
        }
    }

    private void VialItem_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not VialItemViewModel viewModel)
        {
            return;
        }
        if (e.Delta != 0 && viewModel.ToggleVialStarCommand.CanExecute(null))
        {
            viewModel.ToggleVialStarCommand.Execute(null);
            e.Handled = true;
        }
    }
}
