using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Hints;

namespace TrackOMatic.HintsUI;

public partial class BLockerHint : UserControl
{
    public static readonly DependencyProperty RegionNameProperty = DependencyProperty.Register("RegionName", typeof(RegionName), typeof(BLockerHint));
    public RegionName RegionName
    {
        get { return (RegionName)GetValue(RegionNameProperty); }
        set { SetValue(RegionNameProperty, value); }
    }

    public BLockerHint()
    {
        InitializeComponent();
    }

    private void OnTextLeftClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not BLockerHintViewModel viewModel)
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
            TextBox_LostFocus(sender, null!);
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (DataContext is not BLockerHintViewModel viewModel)
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
