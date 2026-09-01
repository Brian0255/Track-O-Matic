using System.Windows;
using System.Windows.Controls.Primitives;

using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Regions;
/// <summary>
/// Interaction logic for RegionItems.xaml
/// </summary>
public partial class RegionItems : UniformGrid
{
    public RegionItems()
    {
        InitializeComponent();
    }

    public RegionName RegionName => (DataContext as RegionItemListViewModel)?.RegionName ?? RegionName.UNKNOWN;

    protected override Size MeasureOverride(Size constraint)
    {
        Rows = Math.Max(1, (InternalChildren.Count + Columns - 1) / Columns);
        return base.MeasureOverride(constraint);
    }
}
