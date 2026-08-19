using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for HintDialogItem.xaml
/// </summary>
public partial class HintDialogItem : ContentControl
{
    public HintDialogItem()
    {
        InitializeComponent();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            ToggleState();
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta != 0)
        {
            ToggleState();
        }
    }

    private void ToggleState()
    {
        if (DataContext is not HintDialogItemViewModel viewModel)
        {
            return;
        }

        if (viewModel.ToggleSelectionCommand.CanExecute(null))
        {
            viewModel.ToggleSelectionCommand.Execute(null);
        }
    }
}
