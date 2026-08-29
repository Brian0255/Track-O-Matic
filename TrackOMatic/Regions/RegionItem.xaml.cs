using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for RegionItem.xaml
/// </summary>
public partial class RegionItem : UserControl
{
    public RegionItem()
    {
        InitializeComponent();
    }

    private void RegionItem_PreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not RegionItemViewModel viewModel)
        {
            return;
        }
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (isShiftPressed) {
                viewModel.ToggleStar();
            }
            else
            {
                viewModel.RemoveFromRegion();
            }
            e.Handled = true;
        }
        else if (e.MiddleButton == MouseButtonState.Pressed)
        {
            viewModel.ToggleStar();
            e.Handled = true;
        }
    }

    private void RegionItem_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not RegionItemViewModel viewModel)
        {
            return;
        }
        if ( e.Delta != 0)
        {
            viewModel.ToggleStar();
            e.Handled = true;
        }
    }
}
