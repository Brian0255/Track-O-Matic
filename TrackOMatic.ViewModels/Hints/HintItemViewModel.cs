using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.ViewModels.Hints;

public partial class HintItemViewModel : ObservableObject
{
    #region Proxy Properties

    private ItemName _itemName;
    public ItemName ItemName
    {
        get => _itemName;
        private set => SetProperty(ref _itemName, value);
    }

    private bool _isFound;
    public bool IsFound
    {
        get => _isFound;
        private set => SetProperty(ref _isFound, value);
    }

    private string _imageResourceKey = "";
    public string ImageResourceKey
    {
        get => _imageResourceKey;
        private set => SetProperty(ref _imageResourceKey, value);
    }

    private string _checkmarkKey = "";
    public string CheckmarkKey
    {
        get => _checkmarkKey;
        private set => SetProperty(ref _checkmarkKey, value);
    }

    #endregion

    public HintItemViewModel(ItemName itemName, bool isFound)
    {
        _itemName = itemName;
        _isFound = isFound;
        UpdateResourceKey();
    }

    private void UpdateResourceKey()
    {
        ImageResourceKey = ItemName.ToString().ToLowerInvariant();
        CheckmarkKey = "checkmark";
    }

    [RelayCommand]
    private void ToggleFound()
    {
        IsFound = !IsFound;
    }
}
