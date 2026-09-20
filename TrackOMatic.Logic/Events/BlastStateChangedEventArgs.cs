using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for changing which Kong handles disabling the Blast-O-Matic in the correct order.
/// </summary>
/// <param name="oldValue"></param>
/// <param name="newValue"></param>
/// <param name="changeReason"></param>
public class BlastStateChangedEventArgs(List<int> oldValue, List<int> newValue, ChangeReason changeReason = ChangeReason.Unknown) : ValueChangedEventArgs<List<int>>(oldValue, newValue, changeReason)
{
}
