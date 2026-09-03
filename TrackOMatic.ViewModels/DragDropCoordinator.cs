using TrackOMatic.Logic.Enums;
using TrackOMatic.ViewModels;
using TrackOMatic.ViewModels.Regions;

namespace TrackOMatic.Ui;

public class DragDropCoordinator : IDragDropCoordinator
{
    public bool TryExecuteDrop(ItemName itemName, RegionViewModel viewModel, MouseDragType dragType)
    {
        IItemListViewModel? listModel = viewModel.HasVials ? viewModel.VialItemListViewModel : viewModel.RegionItemListViewModel;
        return listModel is not null && listModel.TryAcceptDrop(itemName, dragType);
    }
}
