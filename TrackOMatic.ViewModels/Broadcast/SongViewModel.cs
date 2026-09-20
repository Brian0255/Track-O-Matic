using CommunityToolkit.Mvvm.ComponentModel;

using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

/// <summary>
/// View model for displaying song information.
/// Can be injected into MainWindow or BroadcastView independently.
/// Handles synchronization context marshaling for UI updates from background threads.
/// </summary>
public partial class SongViewModel : ObservableObject, IDisposable
{
    private readonly ISongService _songService;
    private readonly SynchronizationContext _uiContext;

    [ObservableProperty]
    public partial string SongGame { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SongName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ResourceKey { get; set; } = string.Empty;

    public SongViewModel(ISongService songService)
    {
        _songService = songService ?? throw new ArgumentNullException(nameof(songService));
        _uiContext = SynchronizationContext.Current ?? throw new InvalidOperationException("SongViewModel must be created on a thread with an active SynchronizationContext.");

        // Initialize from current state
        InitializeState();
        ResourceKey = "headphones";

        // Subscribe to changes
        _songService.SongChanged += OnSongChanged;
    }

    private void InitializeState()
    {
        SongGame = _songService.SongGame;
        SongName = _songService.SongName;
    }

    private void OnSongChanged(object? sender, SongChangedEventArgs e)
    {
        // Marshal to UI thread if needed (AutotrackerService runs on background thread)
        if (SynchronizationContext.Current != _uiContext)
        {
            _uiContext.Post(_ => UpdateFromService(), null);
        }
        else
        {
            UpdateFromService();
        }
    }

    private void UpdateFromService()
    {
        SongGame = _songService.SongGame;
        SongName = _songService.SongName;
    }

    #region IDisposable Implementation

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
                _songService.SongChanged -= OnSongChanged;
            }
            _disposed = true;
        }
    }

    ~SongViewModel()
    {
        Dispose(false);
    }

    #endregion
}
