using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;
/// <summary>
/// Interaction logic for FoundHintItem.xaml
/// </summary>
public partial class FoundHintItem : ContentControl
{
    public FoundHintItem()
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
        if (DataContext is not FoundHintItemViewModel viewModel)
        {
            return;
        }

        if (viewModel.ToggleFoundCommand.CanExecute(null))
        {
            viewModel.ToggleFoundCommand.Execute(null);
        }
    }
}
