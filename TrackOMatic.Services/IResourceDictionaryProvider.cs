namespace TrackOMatic.Services;

/// <summary>
/// Provides theme-aware resource dictionary management for the application.
/// Handles loading and switching resource dictionaries based on theme settings.
/// This is an optional service that WPF applications can implement to manage their
/// application-level resource dictionaries. Avalonia apps would have their own implementation.
/// </summary>
public interface IResourceDictionaryProvider
{
    /// <summary>
    /// Updates the application's resource dictionaries based on the current theme.
    /// Called when the theme changes or during application startup.
    /// </summary>
    void UpdateResourceDictionaries();
}
