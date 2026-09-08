using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.ViewModels.Hints;

public partial class HintDialogItemViewModel : ObservableObject
{
    #region Proxy Properties

    [ObservableProperty]
    public partial HintGridCell Cell { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; } = false;

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    #endregion

    public HintDialogItemViewModel(HintGridCell cell, bool isSelected)
    {
        Cell = cell;
        IsSelected = isSelected;
        UpdateResourceKey();
    }

    [RelayCommand]
    private void ToggleSelection()
    {
        IsSelected = !IsSelected;
        UpdateResourceKey();
    }

    private void UpdateResourceKey()
    {
        var key = Cell.ItemName.ToString().ToLowerInvariant();
        if (!IsSelected)
        {
            key += "_bw";
        }
        ImageResourceKey = key;
    }
}
