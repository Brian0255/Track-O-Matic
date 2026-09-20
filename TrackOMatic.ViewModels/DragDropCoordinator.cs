using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.ViewModels;

public class DragDropCoordinator : IDragDropCoordinator
{
    public bool TryExecuteDrop(ItemName itemName, RegionViewModel viewModel, MouseDragType dragType)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        IItemListViewModel? listModel = viewModel.HasVials ? viewModel.VialItemListViewModel : viewModel.RegionItemListViewModel;
        return listModel is not null && listModel.TryAcceptDrop(itemName, dragType);
    }
}
