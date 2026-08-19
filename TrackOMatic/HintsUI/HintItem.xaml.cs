using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for HintItem.xaml
/// </summary>
public partial class HintItem : ContentControl
{
    public HintItem()
    {
        InitializeComponent();
    }

    private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        {
            ToggleCheck();
        }
        else if (e.MiddleButton == MouseButtonState.Pressed)
        {
            ToggleCheck();
        }
    }

    private void ToggleCheck()
    {
        if (DataContext is not HintItemViewModel viewModel)
        {
            return;
        }

        if (viewModel.ToggleFoundCommand.CanExecute(null))
        {
            viewModel.ToggleFoundCommand.Execute(null);
        }
    }
}
