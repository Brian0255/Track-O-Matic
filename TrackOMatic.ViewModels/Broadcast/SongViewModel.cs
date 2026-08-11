using System.ComponentModel;
using System.Runtime.CompilerServices;

using TrackOMatic.Logic.Events.Autotracking;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic.ViewModels.Broadcast;

/// <summary>
/// View model for displaying song information.
/// Can be injected into MainWindow or BroadcastView independently.
/// Handles synchronization context marshaling for UI updates from background threads.
/// </summary>
public class SongViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly ISongService _songService;
    private readonly SynchronizationContext? _uiContext;
    private string _songGame = string.Empty;
    private string _songName = string.Empty;
    private string _resourceKey = string.Empty;

    public SongViewModel(ISongService songService)
    {
        _songService = songService ?? throw new ArgumentNullException(nameof(songService));
        _uiContext = SynchronizationContext.Current; // Capture UI context at creation

        // Initialize from current state
        InitializeState();
        ResourceKey = "headphones";

        // Subscribe to changes
        _songService.SongChanged += OnSongChanged;
    }

    public string SongGame
    {
        get => _songGame;
        private set
        {
            if (_songGame != value)
            {
                _songGame = value;
                OnPropertyChanged();
            }
        }
    }

    public string SongName
    {
        get => _songName;
        private set
        {
            if (_songName != value)
            {
                _songName = value;
                OnPropertyChanged();
            }
        }
    }

    public string ResourceKey
    {
        get => _resourceKey;
        private set
        {
            if (_resourceKey != value)
            {
                _resourceKey = value;
                OnPropertyChanged();
            }
        }
    }

    private void InitializeState()
    {
        SongGame = _songService.SongGame;
        SongName = _songService.SongName;
    }

    private void OnSongChanged(object? sender, SongChangedEventArgs e)
    {
        // Marshal to UI thread if needed (AutotrackerService runs on background thread)
        if (_uiContext is not null && SynchronizationContext.Current != _uiContext)
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

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

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
