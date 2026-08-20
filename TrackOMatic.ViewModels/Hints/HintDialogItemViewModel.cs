using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Models.Hints;

namespace TrackOMatic.ViewModels.Hints;

public partial class HintDialogItemViewModel : ObservableObject
{
    #region Proxy Properties

    private HintGridCell _cell;
    public HintGridCell Cell
    {
        get => _cell;
        set => SetProperty(ref _cell, value);
    }

    private bool _isSelected = false;
    public bool IsSelected
    {
        get => _isSelected;
        internal set => SetProperty(ref _isSelected, value);
    }

    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        private set => SetProperty(ref _imageResourceKey, value);
    }

    #endregion

    public HintDialogItemViewModel(HintGridCell cell, bool isSelected)
    {
        _cell = cell;
        _isSelected = isSelected;
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
            key = key + "_bw";
        }
        ImageResourceKey = key;
    }
}
