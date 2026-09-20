using System.ComponentModel;
using System.Runtime.CompilerServices;

using TrackOMatic.Logic.Enums;

[assembly: InternalsVisibleTo("TrackOMatic.Services.Test")]

namespace TrackOMatic.Services;

public class ApplicationStateService : IApplicationStateService
{
    private const string StateFileName = "app-state.json";
    private readonly string _stateFilePath;
    private readonly ApplicationStateData _stateData;
    private readonly IDataPersistenceService _dataPersistenceService;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Creates an ApplicationStateService with default app data folder location.
    /// </summary>
    public ApplicationStateService(IDataPersistenceService dataPersistenceService) : this(null, dataPersistenceService)
    {
    }

    /// <summary>
    /// Creates an ApplicationStateService with optional custom state file path.
    /// Used primarily for testing with custom file locations.
    /// </summary>
    /// <param name="stateFilePath">Optional custom path for state file. If null, uses default app data folder.</param>
    internal ApplicationStateService(string? stateFilePath, IDataPersistenceService dataPersistenceService)
    {
        _dataPersistenceService = dataPersistenceService;

        // Determine platform-specific app data folder or use provided path
        if (string.IsNullOrEmpty(stateFilePath))
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "Track-O-Matic");
            _stateFilePath = Path.Combine(appFolder, StateFileName);
        }
        else
        {
            _stateFilePath = stateFilePath;
        }

        // Initialize data with defaults
        _stateData = new ApplicationStateData();

        // Try to load from file, use defaults if file doesn't exist
        try
        {
            var data = _dataPersistenceService.LoadAsync<ApplicationStateData>(_stateFilePath)
                .GetAwaiter()
                .GetResult();

            _stateData = data;
        }
        catch (Exception ex)
        {
            // Log or handle error - for now, silently fall back to defaults
            System.Diagnostics.Debug.WriteLine($"Failed to load settings from {_stateFilePath}: {ex.Message}");
            _stateData = new();
        }
    }

    /// <summary>
    /// Saves settings to the JSON file.
    /// </summary>
    internal async Task SaveToFileAsync()
    {
        try
        {
            await _dataPersistenceService.SaveAsync(_stateData, _stateFilePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings to {_stateFilePath}: {ex.Message}");
        }
    }

    #region IApplicationStateService Implementation

    public double WindowX
    {
        get => _stateData.WindowX;
        set => SetProperty(nameof(WindowX), () => _stateData.WindowX, v => _stateData.WindowX = v, value);
    }

    public double WindowY
    {
        get => _stateData.WindowY;
        set => SetProperty(nameof(WindowY), () => _stateData.WindowY, v => _stateData.WindowY = v, value);
    }

    public double DesiredWidth
    {
        get => _stateData.DesiredWidth;
        set => SetProperty(nameof(DesiredWidth), () => _stateData.DesiredWidth, v => _stateData.DesiredWidth = v, value);
    }

    public double DesiredHeight
    {
        get => _stateData.DesiredHeight;
        set => SetProperty(nameof(DesiredHeight), () => _stateData.DesiredHeight, v => _stateData.DesiredHeight = v, value);
    }

    public string LastFolderPath
    {
        get => _stateData.LastFolderPath;
        set => SetProperty(nameof(LastFolderPath), () => _stateData.LastFolderPath, v => _stateData.LastFolderPath = v, value);
    }

    public ItemType ProgressiveHintItem
    {
        get => (ItemType)_stateData.ProgressiveHintItem;
        set => SetProperty(nameof(ProgressiveHintItem), () => _stateData.ProgressiveHintItem, v => _stateData.ProgressiveHintItem = v, (int)value);
    }

    public int ProgressiveHintCap
    {
        get => _stateData.ProgressiveHintCap;
        set => SetProperty(nameof(ProgressiveHintCap), () => _stateData.ProgressiveHintCap, v => _stateData.ProgressiveHintCap = v, value);
    }

    #endregion

    #region INotifyPropertyChanged Support

    /// <summary>
    /// Sets a property value and notifies if changed.
    /// </summary>
    private void SetProperty<T>(string propertyName, Func<T> getter, Action<T> setter, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(getter(), value))
        {
            setter(value);
            OnPropertyChanged(propertyName);
            _ = SaveToFileAsync();
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
