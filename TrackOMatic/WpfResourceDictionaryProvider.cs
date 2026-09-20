using System.Windows;

using TrackOMatic.Services;

namespace TrackOMatic;

/// <summary>
/// WPF implementation of IResourceDictionaryProvider.
/// Manages application-level resource dictionary merging for barrel/pad image variants.
/// This is WPF-specific and lives in the UI project, not in the framework-agnostic Services layer.
/// </summary>
public class WpfResourceDictionaryProvider(IThemeService themeService) : IResourceDictionaryProvider
{
    private readonly IThemeService _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

    public void UpdateResourceDictionaries()
    {
        var dicts = Application.Current.Resources.MergedDictionaries;
        dicts.Clear();

        // Always load the base dictionary first
        dicts.Add(new ResourceDictionary
        {
            Source = new Uri("Dictionary1.xaml", UriKind.Relative)
        });

        // Load the appropriate barrel/pad images dictionary based on theme
        var path = _themeService.UseColoredBarrelPadMoves
            ? "ColoredBarrelPadImages.xaml"
            : "BaseBarrelPadImages.xaml";

        dicts.Add(new ResourceDictionary
        {
            Source = new Uri(path, UriKind.Relative)
        });
    }
}
