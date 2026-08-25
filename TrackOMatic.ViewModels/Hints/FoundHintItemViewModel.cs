using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class FoundHintItemViewModel : ObservableObject, IEquatable<FoundHintItemViewModel>, IDisposable
{
    private readonly HintId _hintId;
    private readonly IHintService _hintService;

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

    public FoundHintItemViewModel(HintId hintId, ItemName itemName, bool isFound, IHintService hintService)
    {
        _hintId = hintId;
        _itemName = itemName;
        _isFound = isFound;
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _hintService.HintFoundItemToggled += OnHintFoundItemToggled;
        UpdateResourceKey();
    }

    private void UpdateResourceKey()
    {
        ImageResourceKey = ItemName.ToString().ToLowerInvariant();
        CheckmarkKey = "checkmark";
    }

    [RelayCommand(CanExecute = nameof(CanToggleFound))]
    private void ToggleFound()
    {
        _hintService.ToggleFoundItem(_hintId, ItemName);
    }

    private bool CanToggleFound() => ItemName != ItemName.NONE;

    private void OnHintFoundItemToggled(object? sender, HintItemToggleEventArgs e)
    {
        if (e.HintId == _hintId && e.ItemName == _itemName)
        {
            IsFound = e.IsFound;
        }
    }

    #region Object Overrides

    public override bool Equals(object? obj) => Equals(obj as FoundHintItemViewModel);

    public bool Equals(FoundHintItemViewModel? other) => other is not null && ItemName.Equals(other.ItemName);

    public override int GetHashCode() => ItemName.GetHashCode();

    public override string ToString() => $"{ItemName} Found: {(IsFound ? "✓" : "✗")}";

    #endregion

    #region Operator Overloads

    public static bool operator ==(FoundHintItemViewModel? left, FoundHintItemViewModel? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(FoundHintItemViewModel? left, FoundHintItemViewModel? right) => !(left == right);

    #endregion

    #region IDisposable Support

    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _hintService.HintFoundItemToggled -= OnHintFoundItemToggled;
            }
            _disposed = true;
        }
    }

    ~FoundHintItemViewModel()
    {
        Dispose(false);
    }

    #endregion
}
