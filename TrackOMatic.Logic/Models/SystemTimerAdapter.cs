using System.Timers;

using Timer = System.Timers.Timer;

namespace TrackOMatic.Logic.Models;

/// <summary>
/// Adapter that wraps System.Timers.Timer to implement ITimer.
/// This allows the timer to be injected and mocked in tests.
/// </summary>
/// <remarks>
/// Creates a new SystemTimerAdapter with the specified interval.
/// </remarks>
/// <param name="intervalMs">The interval in milliseconds.</param>
public class SystemTimerAdapter(double intervalMs) : ITimer
{
    private readonly Timer _timer = new(intervalMs);
    private bool _disposed;

    /// <summary>
    /// Gets or sets the interval in milliseconds.
    /// </summary>
    public double Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    /// <summary>
    /// Gets or sets whether the timer automatically restarts after each tick.
    /// </summary>
    public bool AutoReset
    {
        get => _timer.AutoReset;
        set => _timer.AutoReset = value;
    }

    /// <summary>
    /// Raised when the timer interval elapses.
    /// </summary>
    public event ElapsedEventHandler? Elapsed
    {
        add => _timer.Elapsed += value;
        remove => _timer.Elapsed -= value;
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void Start()
    {
        if (!_disposed)
        {
            _timer.Start();
        }
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop()
    {
        if (!_disposed)
        {
            _timer.Stop();
        }
    }

    /// <summary>
    /// Releases all resources used by the adapter and underlying timer.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _timer?.Dispose();
        _disposed = true;
    }
}
