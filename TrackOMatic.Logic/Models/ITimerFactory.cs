namespace TrackOMatic.Logic.Models;

/// <summary>
/// Factory interface for creating ITimer instances.
/// This allows tests to inject mock timers and production code to use real timers.
/// </summary>
public interface ITimerFactory
{
    /// <summary>
    /// Creates a new timer instance with the specified interval.
    /// </summary>
    /// <param name="intervalMs">The interval in milliseconds.</param>
    /// <returns>A new ITimer instance.</returns>
    ITimer CreateTimer(double intervalMs);
}
