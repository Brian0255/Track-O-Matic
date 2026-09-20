using System.ComponentModel;

namespace TrackOMatic.Services;

/// <summary>
/// Implementation of IThemeService that manages barrel/pad image variants.
/// </summary>
public class BarrelPadThemeService : IThemeService
{
    private readonly IUserSettingsService _settingsService;
    private bool _useColoredBarrelPadMoves;

    public event EventHandler? BarrelPadThemeChanged;

    public bool UseColoredBarrelPadMoves
    {
        get => _useColoredBarrelPadMoves;
        private set
        {
            if (_useColoredBarrelPadMoves != value)
            {
                _useColoredBarrelPadMoves = value;
                BarrelPadThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public BarrelPadThemeService(IUserSettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        _useColoredBarrelPadMoves = settingsService.ColoredBarrelPadMoves;

        // Subscribe to settings changes so we stay in sync
        if (_settingsService is INotifyPropertyChanged settingsNotifier)
        {
            settingsNotifier.PropertyChanged += OnSettingsPropertyChanged;
        }
    }

    public void SetBarrelPadTheme(bool useColored)
    {
        // Update the underlying settings (this will also trigger our property changed handler)
        _settingsService.ColoredBarrelPadMoves = useColored;
    }

    private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IUserSettingsService.ColoredBarrelPadMoves))
        {
            UseColoredBarrelPadMoves = _settingsService.ColoredBarrelPadMoves;
        }
    }
}
