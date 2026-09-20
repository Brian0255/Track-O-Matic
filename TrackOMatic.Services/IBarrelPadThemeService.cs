namespace TrackOMatic.Services;

/// <summary>
/// Service for managing theme variants (e.g., colored vs base barrel/pad images).
/// Fires events when theme properties change so subscribers can update their state.
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Fired when the barrel/pad image theme changes (colored vs base).
    /// </summary>
    event EventHandler? BarrelPadThemeChanged;

    /// <summary>
    /// Gets whether colored barrel/pad images should be used.
    /// </summary>
    bool UseColoredBarrelPadMoves { get; }

    /// <summary>
    /// Updates the barrel/pad image theme setting.
    /// Raises BarrelPadThemeChanged if the value changes.
    /// </summary>
    void SetBarrelPadTheme(bool useColored);
}
