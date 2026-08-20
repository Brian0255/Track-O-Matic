using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

/// <summary>
/// Interaction logic for FoundHintItemList.xaml
/// </summary>
public partial class FoundHintItemList : UserControl
{
    public FoundHintItemList()
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

        e.Handled = true;
        if (DataContext is not FoundHintItemListViewModel viewModel)
        {
            return;
        }

        var mousePos = Mouse.GetPosition(this);
        var screenMousePosition = PointToScreen(mousePos);
        var parameter = (screenMousePosition.X, screenMousePosition.Y);

        if (viewModel.SelectFoundItemsCommand.CanExecute(parameter))
        {
            viewModel.SelectFoundItemsCommand.Execute(parameter);
        }
    }
}
