using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Events;
using TrackOMatic.Logic.Models;

namespace TrackOMatic.Services.TrackerState;

/// <summary>
/// A service for managing the end game portions of Blast-O-Matic destruction and final boss completion.
/// This also wraps access to the <see cref="SavedProgress"/> state in case it changes via reset or load.
/// </summary>
public class EndGameProgressionService: IEndGameProgressionService
{
    private readonly ISavedProgressProvider _progressProvider;
    private SavedProgress _savedProgress;

    public event EventHandler<BlastStateChangedEventArgs>? BlastStateChanged;
    public event EventHandler<GauntletStateChangedEventArgs>? GauntletStateChanged;

    public EndGameProgressionService(ISavedProgressProvider progressProvider)
    {
        _progressProvider = progressProvider ?? throw new ArgumentNullException(nameof(progressProvider));
        _savedProgress = _progressProvider.CurrentProgress;
        // Set up listener for SavedProgress changes
        _progressProvider.ProgressChanged += OnProgressChanged;
    }

    private void InitializeCollections()
    {
        _savedProgress.HelmKongs ??= new(5);
        _savedProgress.BossKongs ??= new(5);
    }

    private void OnProgressChanged(object? sender, ProgressReplacedEventArgs e)
    {
        _savedProgress = e.NewProgress;
        InitializeCollections();
    }

    #region IEndGameProgressionService Implementation

    public IList<int> GetHelmKongs()
    {
        return _savedProgress.HelmKongs;
    }

    public void SetHelmKongs(IEnumerable<int> kongs)
    {
        var previous = GetHelmKongs();
        var newList = kongs.ToList();
        _savedProgress.HelmKongs = newList;
        BlastStateChanged?.Invoke(this, new BlastStateChangedEventArgs([.. previous], newList, ChangeReason.UserModified));
    }

    public IReadOnlyList<int> GetBossKongs()
    {
        return _savedProgress.BossKongs.AsReadOnly();
    }

    public void SetBossKongs(IEnumerable<int> kongs)
    {
        var previous = GetBossKongs();
        var newList = kongs.ToList();
        _savedProgress.BossKongs = newList;
        GauntletStateChanged?.Invoke(this, new GauntletStateChangedEventArgs([.. previous], newList, ChangeReason.UserModified));
    }

    #endregion
}
