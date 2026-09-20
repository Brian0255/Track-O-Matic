using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events.Hints;
using TrackOMatic.Logic.Models;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Hints;

public partial class PathHintItemViewModel : ObservableObject, IEquatable<PathHintItemViewModel>, IDisposable
{
    private readonly HintId _hintId;
    private readonly IHintService _hintService;

    #region Proxy Properties

    [ObservableProperty]
    public partial ItemName ItemName { get; set; }

    [ObservableProperty]
    public partial bool IsFound { get; set; }

    [ObservableProperty]
    public partial string ImageResourceKey { get; set; } = "";

    [ObservableProperty]
    public partial string CheckmarkKey { get; set; } = "";

    #endregion

    public PathHintItemViewModel(HintId hintId, ItemName itemName, bool isFound, IHintService hintService)
    {
        _hintId = hintId;
        ItemName = itemName;
        IsFound = isFound;
        _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
        _hintService.HintPathItemToggled += OnHintPathItemToggled;
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
        _hintService.TogglePathItem(_hintId, ItemName);
    }

    private bool CanToggleFound() => ItemName != ItemName.NONE;

    private void OnHintPathItemToggled(object? sender, HintItemToggleEventArgs e)
    {
        if (e.HintId == _hintId && e.ItemName == ItemName)
        {
            IsFound = e.IsFound;
        }
    }

    #region Object Overrides

    public override bool Equals(object? obj) => Equals(obj as PathHintItemViewModel);

    public bool Equals(PathHintItemViewModel? other) => other is not null && ItemName.Equals(other.ItemName);

    public override int GetHashCode() => ItemName.GetHashCode();

    public override string ToString() => $"{ItemName} on Path: {(IsFound ? "✓" : "✗")}";

    #endregion

    #region Operator Overloads

    public static bool operator ==(PathHintItemViewModel? left, PathHintItemViewModel? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(PathHintItemViewModel? left, PathHintItemViewModel? right) => !(left == right);

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
                _hintService.HintPathItemToggled -= OnHintPathItemToggled;
            }
            _disposed = true;
        }
    }

    ~PathHintItemViewModel()
    {
        Dispose(false);
    }

    #endregion
}
