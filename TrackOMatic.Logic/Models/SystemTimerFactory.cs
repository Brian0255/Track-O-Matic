namespace TrackOMatic.Logic.Models;

/// <summary>
/// Factory for creating SystemTimerAdapter instances.
/// </summary>
public class SystemTimerFactory : ITimerFactory
{
    /// <summary>
    /// Creates a new SystemTimerAdapter with the specified interval.
    /// </summary>
    /// <param name="intervalMs">The interval in milliseconds.</param>
    /// <returns>A new SystemTimerAdapter instance.</returns>
    public ITimer CreateTimer(double intervalMs)
    {
        return new SystemTimerAdapter(intervalMs);
    }
}
