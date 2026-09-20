using Microsoft.Extensions.DependencyInjection;

using TrackOMatic.Logic.Models.Autotracking;

namespace TrackOMatic.AutoTracking.Windows;

/// <summary>
/// Dependency injection extension for Windows-specific autotracking services.
/// Registers Windows implementations of IProcessMemoryReader and IEmulatorAttacher.
/// </summary>
public static class WindowsAutotrackerServiceRegistration
{
    /// <summary>
    /// Adds Windows-specific autotracking services to the dependency injection container.
    /// Call this from your platform-specific application startup (e.g., WPF, Avalonia.Windows).
    /// </summary>
    public static IServiceCollection AddWindowsAutotracking(this IServiceCollection services)
    {
        services.AddSingleton<IProcessMemoryReader, WindowsProcessMemoryReader>();
        services.AddSingleton<IEmulatorAttacher, WindowsEmulatorAttacher>();
        return services;
    }
}
