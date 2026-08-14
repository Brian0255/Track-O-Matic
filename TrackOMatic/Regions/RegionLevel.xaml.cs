using System.Windows.Controls;
using System.Windows.Input;

using TrackOMatic.ViewModels;

namespace TrackOMatic.Regions;

/// <summary>
/// Interaction logic for RegionLevel.xaml
/// </summary>
public partial class RegionLevel : UserControl
{
    public RegionLevel()
    {
        InitializeComponent();
    }

    private void RegionLevel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not RegionLevelViewModel viewModel)
        {
            return;
        }

        viewModel.IncrementLevelOrder();
    }

    private void RegionLevel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not RegionLevelViewModel viewModel)
        {
            return;
        }

        viewModel.DecrementLevelOrder();
    }

    private void RegionLevel_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not RegionLevelViewModel viewModel)
        {
            return;
        }

        viewModel.HandleMouseWheel(e.Delta);
        e.Handled = true;
    }
}

