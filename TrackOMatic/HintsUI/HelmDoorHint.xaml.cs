using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class HelmDoorHint : UserControl
{
    public HelmDoorHint()
    {
        InitializeComponent();
    }

    private void OnTextLeftClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HelmDoorHintViewModel viewModel)
        {
            return;
        }

        if (viewModel.ClearPlaceholderCommand.CanExecute(null))
        {
            viewModel.ClearPlaceholderCommand.Execute(null);
        }
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Return)
        {
            Keyboard.ClearFocus();
            OnLostTextBoxFocus(sender);
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e) => OnLostTextBoxFocus(sender);

    private void OnLostTextBoxFocus(object sender)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (DataContext is not HelmDoorHintViewModel viewModel)
        {
            return;
        }

        // Update the ViewModel's BarrierCount with the current TextBox text before executing the command
        viewModel.BarrierCount = textBox.Text;

        if (viewModel.SetBarrierCountCommand.CanExecute(null))
        {
            viewModel.SetBarrierCountCommand.Execute(null);
        }
    }
}
