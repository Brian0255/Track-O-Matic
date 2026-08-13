using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

using TrackOMatic.Logic;
using TrackOMatic.Logic.Enums;

[assembly: InternalsVisibleTo("TrackOMatic.Services.Test")]

namespace TrackOMatic.Services;

/// <summary>
/// Manages user-facing settings that persist across application sessions.
/// Settings are stored in a JSON file in the platform-specific application data folder.
/// Defaults are hardcoded in UserSettingsData and used when the file doesn't exist.
/// </summary>
public class UserSettingsService : IUserSettingsService
{
    private const string SettingsFileName = "user-settings.json";
    private readonly string _settingsFilePath;
    private UserSettingsData _settingsData;
    private readonly JsonSerializerOptions _serializerOptions;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Creates a UserSettingsService with default app data folder location.
    /// </summary>
    public UserSettingsService() : this(null)
    {
    }

    /// <summary>
    /// Creates a UserSettingsService with optional custom settings file path.
    /// Used primarily for testing with custom file locations.
    /// </summary>
    /// <param name="settingsFilePath">Optional custom path for settings file. If null, uses default app data folder.</param>
    internal UserSettingsService(string? settingsFilePath)
    {
        // Determine platform-specific app data folder or use provided path
        if (string.IsNullOrEmpty(settingsFilePath))
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataPath, "Track-O-Matic");
            _settingsFilePath = Path.Combine(appFolder, SettingsFileName);
        }
        else
        {
            _settingsFilePath = settingsFilePath;
        }

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        }
        ;

        // Initialize data with defaults
        _settingsData = new UserSettingsData();

        // Try to load from file, use defaults if file doesn't exist
        if (File.Exists(_settingsFilePath))
        {
            LoadFromFile();
        }
    }

    /// <summary>
    /// Loads settings from the JSON file.
    /// </summary>
    private void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return;
            }

            string json = File.ReadAllText(_settingsFilePath);
            var loaded = JsonSerializer.Deserialize<UserSettingsData>(json, _serializerOptions);

            if (loaded != null)
            {
                _settingsData = loaded;
            }
        }
        catch (Exception ex)
        {
            // Log or handle error - for now, silently fall back to defaults
            System.Diagnostics.Debug.WriteLine($"Failed to load settings from {_settingsFilePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves settings to the JSON file.
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            // Ensure directory exists
            string? directory = Path.GetDirectoryName(_settingsFilePath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(_settingsData, _serializerOptions);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings to {_settingsFilePath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Converts HintDisplayMode enum value to its string representation for JSON storage.
    /// Uses the enum name by default, which is human-readable.
    /// </summary>
    private static string HintDisplayModeToString(HintDisplayMode mode) => mode.ToString();

    #region IUserSettingsService Implementation

    public bool TopMost
    {
        get => _settingsData.TopMost;
        set => SetProperty(nameof(TopMost), () => _settingsData.TopMost, v => _settingsData.TopMost = v, value);
    }

    public bool SongDisplay
    {
        get => _settingsData.SongDisplay;
        set => SetProperty(nameof(SongDisplay), () => _settingsData.SongDisplay, v => _settingsData.SongDisplay = v, value);
    }

    public bool CompactMode
    {
        get => _settingsData.CompactMode;
        set => SetProperty(nameof(CompactMode), () => _settingsData.CompactMode, v => _settingsData.CompactMode = v, value);
    }

    public bool HelmInLevelOrder
    {
        get => _settingsData.HelmInLevelOrder;
        set => SetProperty(nameof(HelmInLevelOrder), () => _settingsData.HelmInLevelOrder, v => _settingsData.HelmInLevelOrder = v, value);
    }

    public bool HelmDoors
    {
        get => _settingsData.HelmDoors;
        set => SetProperty(nameof(HelmDoors), () => _settingsData.HelmDoors, v => _settingsData.HelmDoors = v, value);
    }

    public bool AutoSortPathHints
    {
        get => _settingsData.AutoSortPathHints;
        set => SetProperty(nameof(AutoSortPathHints), () => _settingsData.AutoSortPathHints, v => _settingsData.AutoSortPathHints = v, value);
    }

    public bool EnemiesInAutofill
    {
        get => _settingsData.EnemiesInAutofill;
        set => SetProperty(nameof(EnemiesInAutofill), () => _settingsData.EnemiesInAutofill, v => _settingsData.EnemiesInAutofill = v, value);
    }

    public bool ColoredBarrelPadMoves
    {
        get => _settingsData.ColoredBarrelPadMoves;
        set => SetProperty(nameof(ColoredBarrelPadMoves), () => _settingsData.ColoredBarrelPadMoves, v => _settingsData.ColoredBarrelPadMoves = v, value);
    }

    public bool Autotracking
    {
        get => _settingsData.Autotracking;
        set => SetProperty(nameof(Autotracking), () => _settingsData.Autotracking, v => _settingsData.Autotracking = v, value);
    }

    public bool ShowTotalBPs
    {
        get => _settingsData.ShowTotalBPs;
        set => SetProperty(nameof(ShowTotalBPs), () => _settingsData.ShowTotalBPs, v => _settingsData.ShowTotalBPs = v, value);
    }

    public bool ShowCompanyCoins
    {
        get => _settingsData.ShowCompanyCoins;
        set => SetProperty(nameof(ShowCompanyCoins), () => _settingsData.ShowCompanyCoins, v => _settingsData.ShowCompanyCoins = v, value);
    }

    public bool ShowKRoolOrder
    {
        get => _settingsData.ShowKRoolOrder;
        set => SetProperty(nameof(ShowKRoolOrder), () => _settingsData.ShowKRoolOrder, v => _settingsData.ShowKRoolOrder = v, value);
    }

    public bool ShowHelmOrder
    {
        get => _settingsData.ShowHelmOrder;
        set => SetProperty(nameof(ShowHelmOrder), () => _settingsData.ShowHelmOrder, v => _settingsData.ShowHelmOrder = v, value);
    }

    public bool BroadcastHelmKRool
    {
        get => _settingsData.BroadcastHelmKRool;
        set => SetProperty(nameof(BroadcastHelmKRool), () => _settingsData.BroadcastHelmKRool, v => _settingsData.BroadcastHelmKRool = v, value);
    }

    public bool BroadcastShopkeepers
    {
        get => _settingsData.BroadcastShopkeepers;
        set => SetProperty(nameof(BroadcastShopkeepers), () => _settingsData.BroadcastShopkeepers, v => _settingsData.BroadcastShopkeepers = v, value);
    }

    public bool BroadcastSongDisplay
    {
        get => _settingsData.BroadcastSongDisplay;
        set => SetProperty(nameof(BroadcastSongDisplay), () => _settingsData.BroadcastSongDisplay, v => _settingsData.BroadcastSongDisplay = v, value);
    }

    public BroadcastNumberLabel BroadcastNumberLabel
    {
        get => _settingsData.BroadcastNumberLabel.FromLegacyToEnum<BroadcastNumberLabel>();
        set
        {
            string stringValue = value.ToString();
            SetProperty(nameof(BroadcastNumberLabel), () => _settingsData.BroadcastNumberLabel, v => _settingsData.BroadcastNumberLabel = v, stringValue);
        }
    }

    public HintDisplayMode HintDisplay
    {
        get => _settingsData.HintDisplay.FromLegacyToEnum<HintDisplayMode>();
        set
        {
            string stringValue = HintDisplayModeToString(value);
            SetProperty(nameof(HintDisplay), () => _settingsData.HintDisplay, v => _settingsData.HintDisplay = v, stringValue);
        }
    }

    public bool ShowAmountForHints
    {
        get => _settingsData.ShowAmountForHints;
        set => SetProperty(nameof(ShowAmountForHints), () => _settingsData.ShowAmountForHints, v => _settingsData.ShowAmountForHints = v, value);
    }

    public bool BroadcastViewActive
    {
        get => _settingsData.BroadcastViewActive;
        set => SetProperty(nameof(BroadcastViewActive), () => _settingsData.BroadcastViewActive, v => _settingsData.BroadcastViewActive = v, value);
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
            SaveToFile();
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
