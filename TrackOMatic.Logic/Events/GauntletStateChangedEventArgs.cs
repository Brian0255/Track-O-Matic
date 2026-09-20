using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for which boss is fought in the end game and in what order.
/// </summary>
/// <param name="oldValue"></param>
/// <param name="newValue"></param>
/// <param name="changeReason"></param>
public class GauntletStateChangedEventArgs(List<int> oldValue, List<int> newValue, ChangeReason changeReason = ChangeReason.Unknown) : ValueChangedEventArgs<List<int>>(oldValue, newValue, changeReason)
{
}
