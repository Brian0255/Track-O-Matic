using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for PathHintItemList.xaml
/// </summary>
public partial class PathHintItemList : UserControl
{
    public PathHintItemList()
    {
        InitializeComponent();
    }

    private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var pressedShift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        if (pressedShift)
        {
            return;
        }
        // Handle the mouse down event here
        e.Handled = true;
        if (DataContext is not PathHintItemListViewModel viewModel)
        {
            return;
        }

        var mousePos = Mouse.GetPosition(this);
        var screenMousePosition = PointToScreen(mousePos);
        var parameter = (screenMousePosition.X, screenMousePosition.Y);
        if (viewModel.SelectPathItemsCommand.CanExecute(parameter))
        {
            viewModel.SelectPathItemsCommand.Execute(parameter);
        }
    }
}
