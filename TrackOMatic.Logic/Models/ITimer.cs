using System.Timers;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// Platform-agnostic abstraction for a repeating timer.
/// This interface allows for easy mocking and testing of timer-based behavior.
/// </summary>
public interface ITimer : IDisposable
{
    /// <summary>
    /// Gets or sets the interval in milliseconds between timer ticks.
    /// </summary>
    double Interval { get; set; }

    /// <summary>
    /// Gets or sets whether the timer should automatically restart after each tick.
    /// </summary>
    bool AutoReset { get; set; }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the timer.
    /// </summary>
    void Stop();

    /// <summary>
    /// Raised when the timer interval elapses.
    /// </summary>
    event ElapsedEventHandler? Elapsed;
}
