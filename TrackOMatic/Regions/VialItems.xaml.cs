using System.Windows;
using System.Windows.Controls.Primitives;

using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for VialItems.xaml
/// </summary>
public partial class VialItems : UniformGrid
{
    public VialItems()
    {
        InitializeComponent();
    }

    public RegionName RegionName => (DataContext as VialItemListViewModel)?.RegionName ?? RegionName.UNKNOWN;

    public bool TryDropItem(ItemName itemName, MouseDragType dragType)
    {
        if (DataContext is not VialItemListViewModel viewModel)
        {
            return false;
        }
        return viewModel.TryAcceptDrop(itemName, dragType);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Rows = Math.Max(1, (InternalChildren.Count + Columns - 1) / Columns);
        return base.MeasureOverride(constraint);
    }
}
