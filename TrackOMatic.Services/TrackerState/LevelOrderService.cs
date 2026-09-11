using System.ComponentModel;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service for managing which region belongs to which level in the tracker state.
/// This also wraps access to the <see cref="SavedProgress"/> state in case it changes via reset or load.
/// </summary>
public class LevelOrderService: ILevelOrderService
{
    private readonly ISavedProgressProvider _progressProvider;
    private readonly IUserSettingsService _userSettingsService;
    private SavedProgress _savedProgress;

    public event EventHandler<LevelOrderStateChangedEventArgs>? LevelOrderChanged;

    public LevelOrderService(ISavedProgressProvider progressProvider, IUserSettingsService userSettingsService)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _savedProgress = _progressProvider.CurrentProgress;
        // Set up listener for SavedProgress changes
        _progressProvider.ProgressChanged += OnProgressChanged;
        _userSettingsService.PropertyChanged += OnUserSettingsChanged;
    }

    private void InitializeCollections()
    {
        _savedProgress.LevelOrder ??= new(8);
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        _savedProgress = e.NewProgress;
        InitializeCollections();
    }

    private void OnUserSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(_userSettingsService.HelmInLevelOrder))
        {
            return;
        }

        if (!_userSettingsService.HelmInLevelOrder)
        {
            var levelOrder = GetLevelOrder();
            levelOrder[7] = 8;
            SetLevelOrder(levelOrder);
        }
    }

    #region ILevelOrderService Implementation

    public IList<int> GetLevelOrder()
    {
        var list = _savedProgress.LevelOrder;
        if (list is null || list.Count < 7)
        {
            return [0, 0, 0, 0, 0, 0, 0, 0];
        }
        return _savedProgress.LevelOrder;
    }

    public void SetLevelOrder(IEnumerable<int> levelOrder)
    {
        var previous = GetLevelOrder();
        var newList = levelOrder.ToList();
        _savedProgress.LevelOrder = newList;
        LevelOrderChanged?.Invoke(this, new LevelOrderStateChangedEventArgs([.. previous], newList, ChangeReason.UserModified));
    }

    public void LoadLevelOrderSilently(IEnumerable<int> levelOrder)
    {
        _savedProgress.LevelOrder = levelOrder.ToList();
        _progressProvider.CurrentProgress.LevelOrder = levelOrder.ToList();
    }

    #endregion
}
