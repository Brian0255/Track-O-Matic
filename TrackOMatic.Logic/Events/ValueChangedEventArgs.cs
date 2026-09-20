using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for a value change event.
/// </summary>
/// <typeparam name="T">The type of the value that changed.</typeparam>
public class ValueChangedEventArgs<T> : EventChangeData
{
    /// <summary>
    /// Gets the old value before the change.
    /// </summary>
    public T OldValue { get; init; }

    /// <summary>
    /// Gets the new value after the change.
    /// </summary>
    public T NewValue { get; init; }

    public ValueChangedEventArgs(T oldValue, T newValue, ChangeReason changeReason = ChangeReason.Unknown)
    {
        OldValue = oldValue;
        NewValue = newValue;
        ChangeReason = changeReason;
    }
}
